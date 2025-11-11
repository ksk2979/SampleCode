using JetBrains.Annotations;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController, IDayResettable
{
    [Header("View")]
    [SerializeField] Transform _cameraPivot;   // 머리(피치 전용)
    [SerializeField] Transform _yawSource;     // 몸통(Yaw)
    [SerializeField] Transform _handSocket;   // 아이템 붙일 위치
    float _mouseSensitivity = 0.12f; // 민감도 설정 비율
    float _sensitivityScale = 0.012f; // 사용자가 정수를 곱하는 비율
    int _mousePosition = 10;
    float _pitchMin = -80f;
    float _pitchMax = 80f;

    float _pitch;
    float _yawWanted;
    bool _yawIsOnRoot;

    [Header("Interact")]
    [SerializeField] float _interactDistance = 3.0f;      // 최대 거리
    [SerializeField] LayerMask _interactMask = ~0;        // 맞출 레이어(필요 시 전용 레이어만)
    [SerializeField] float _interactRadius = 0.0f;        // 0이면 Raycast(가느다란), >0이면 SphereCast(두께)
    [SerializeField] Key _interactKey = Key.E;            // 상호작용 키

    Inventory _inventory;          // 내 인벤토리
    InventoryUI _inventoryUI;     // 집어넣은 뒤 Refresh 호출
    AimScript _aim;
    bool _inventoryOpen = false; // 인벤이 활성화가 되어있는가? 체크

    GameObject _currentHeld;                  // 현재 손에 든 오브젝트
    int _currentSlot = -1;

    // 입력 캐시
    Vector2 _moveInput;
    
    // 이동속도에 감속 담당
    float _accelTime = 0.07f;
    float _decelTime = 0.12f;

    // 이동 처리에 보간을 주기 위해서
    Vector3 _vel;
    Vector3 _velDamp;

    CapsuleCollider _capsule;

    OptionManager _optionManager;
    CursorManager _cursorManager;
    Coroutine _registerRoutine;
    Vector3 _initPosition;
    Quaternion _initRotation;
    bool _hasInitTransform;
    float _initPitch;

    public override void OnStart()
    {
        _inventory = GetComponent<Inventory>();
        if (_trans == null)
        {
            Init();
            StateStart();
            _animator.applyRootMotion = false;

            //_animEventSender.AddEvent("DieFinishCall", DieFinishCall);
            //_animEventSender.AddEvent("AttackFuntion", AttackFuntion);
            //_animEventSender.AddEvent("SkillFuntion", SkillFuntion);
            //_animEventSender.AddEvent("AttackFinishCall", AttackFinishCall);
        }

        if (_yawSource == null) _yawSource = transform;
        if (_cameraPivot == null && Camera.main != null) _cameraPivot = Camera.main.transform;

        _yawIsOnRoot = (_yawSource == transform);
        var e = _yawSource.rotation.eulerAngles;
        _yawWanted = e.y;
        if (_cameraPivot) _pitch = _cameraPivot.localEulerAngles.x;

        _moveSpeed = 2f; // 달리기 속도 4f

        if (_agent != null)
        {
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.isStopped = true;
            _agent.nextPosition = _rigidbody.position;
        }

        if (_collider != null) { _capsule = _collider as CapsuleCollider; }

        _inventoryUI = UIManager.GetInstance.GetInventoryUI;
        _aim = UIManager.GetInstance.GetAim;
        _optionManager = UIManager.GetInstance.GetOptionManager;
        _cursorManager = UIManager.GetInstance.GetCursorManager;

        if (_inventoryUI != null)
            _inventoryUI.OnInventoryChanged += HandleInventoryChanged;

        CacheInitTransform();
        EnsureRegisteredWithDayCycle();
    }

    void OnDisable()
    {
        if (_inventoryUI != null)
            _inventoryUI.OnInventoryChanged -= HandleInventoryChanged;

        if (_registerRoutine != null)
        {
            StopCoroutine(_registerRoutine);
            _registerRoutine = null;
        }

        if (UIManager.GetInstance.GetDayCycleManager != null)
        {
            UIManager.GetInstance.GetDayCycleManager.UnregisterResettable(this);
        }
    }

    void Update()
    {
        if (_optionManager != null && _optionManager.IsOpen)
        {
            if (_aim.CheckAimActive) { _aim.AimActive(false); }
            return;
        }
        else if (_optionManager != null && !_optionManager.IsOpen)
        {
            if (!_aim.CheckAimActive) { _aim.AimActive(true); }
        }

        // 인벤토리 토글
        InventoryToggleUpdate();
        if (_inventoryOpen)
        {
            _moveInput = Vector2.zero;
            return;
        }

        // 플레이어 움직임
        ReadMoveInput();
        // 대기 상대일때 움직임이 감지되면 무브 애니 실행
        TryRequestMoveState();
        // 달리기
        RunSpeedUpdate();

        // 마우스 민감도 셋팅 []
        MouseSensitivitySettingKey();
        // 플레이어 시야 회전
        LookUpdate();
        // 플레이어 시야에 상호작용할 것이 있는가 체크
        DetectInteractTarget();
        // 플레이어 시야에 상호작용할 것이 있으면 바이딩된 키로 활성화
        InteractUpdate();
        // 단축키 1~6
        HotkeyUpdate();
    }

    protected override void FixedUpdate()
    {
        if (_yawIsOnRoot)
        {
            _rigidbody.MoveRotation(Quaternion.Euler(0f, _yawWanted, 0f));
        }

        base.FixedUpdate();
        if (_agent) { _agent.nextPosition = _rigidbody.position; }
    }
    void LateUpdate()
    {
        if (_cameraPivot) { _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f); }
        if (!_yawIsOnRoot)
        {
            _yawSource.rotation = Quaternion.Euler(0f, _yawWanted, 0f);
        }
    }

    void ReadMoveInput()
    {
        var kb = Keyboard.current;
        if (kb == null) { _moveInput = Vector2.zero; return; }

        float x = 0f, y = 0f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.wKey.isPressed) y += 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;

        var v = new Vector2(x, y);
        if (v.sqrMagnitude > 1f) { v.Normalize(); }
        _moveInput = v;
    }

    void RunSpeedUpdate()
    {
        var kb = Keyboard.current;
        if (kb == null) { return; }

        if (kb.leftShiftKey.isPressed) { _moveSpeed = 4f; }
        else { _moveSpeed = 2f; }
    }

    void TryRequestMoveState()
    {
        if (_curState == eCharacterStates.Idle && _moveInput.sqrMagnitude > 0f)
        {
            SetState(eCharacterStates.Move);
        }
    }

    void MouseSensitivitySettingKey()
    {
        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            _mousePosition -= 1;
            if (_mousePosition <= 0) { _mousePosition = 0; }
            SetMouseSensitivity(_mousePosition);
        }

        if (Keyboard.current.rightBracketKey.wasPressedThisFrame)
        {
            _mousePosition += 1;
            if (_mousePosition >= 30) { _mousePosition = 30; }
            SetMouseSensitivity(_mousePosition);
        }
    }

    protected override void SpawnUpdate()
    {
        SetState(eCharacterStates.Idle);
    }

    protected override void IdleInit()
    {
        VelocitySetting(0f);
    }

    protected override void MoveInit()
    {
        VelocitySetting(1f);
        if (_animator != null) { _animator.SetTrigger(CommonStaticKey.ANIMPARAM_MOVE); }
        //_rigidbody.isKinematic = true;
        _vel = Vector3.zero;
        _velDamp = Vector3.zero;
    }
    protected override void MoveUpdate()
    {
        if (_moveInput.sqrMagnitude <= 0f)
        {
            float smooth = _decelTime;
            _vel = Vector3.SmoothDamp(_vel, Vector3.zero, ref _velDamp, smooth, Mathf.Infinity, Time.fixedDeltaTime);
            Vector3 stepVel = _vel * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + new Vector3(stepVel.x, 0f, stepVel.z));
            if (_vel.sqrMagnitude < 0.0001f)
            {
                _vel = Vector3.zero;
                SetState(eCharacterStates.Idle);
            }
            return;
        }

        // 시야 기준 평면 방향
        Vector3 fwd = Vector3.ProjectOnPlane(_yawSource.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_yawSource.right, Vector3.up).normalized;
        Vector3 dir = (fwd * _moveInput.y + right * _moveInput.x).normalized;

        // 목표 속도
        Vector3 wishVel = dir * _moveSpeed;
        float accelOrDecel = _accelTime;
        // 부드러운 속도 보간
        _vel = Vector3.SmoothDamp(_vel, wishVel, ref _velDamp, accelOrDecel, Mathf.Infinity, Time.fixedDeltaTime);

        // 벽 부분 마찰 줄이기 위해
        float skin = 0.02f;
        Vector3 step = _vel * Time.fixedDeltaTime;
        if (step.sqrMagnitude > 1e-9f)
        {
            Vector3 p1, p2; float radius;
            GetCapsuleAt(_rigidbody.position, out p1, out p2, out radius);
            radius = Mathf.Max(0f, radius - skin);

            if (Physics.CapsuleCast(p1, p2, radius, step.normalized, out RaycastHit hit, step.magnitude + skin,
                                     ~0, QueryTriggerInteraction.Ignore))
            {
                float into = Vector3.Dot(_vel, hit.normal);
                if (into > 0f) { /* Debug.Log("벽에서 멀어짐") */ }
                else { _vel -= hit.normal * into; }
                step = _vel * Time.fixedDeltaTime;
            }
        }

        _rigidbody.MovePosition(_rigidbody.position + new Vector3(step.x, 0f, step.z));

        if (_animator != null)
        {
            float norm = Mathf.Clamp01(_vel.magnitude / Mathf.Max(0.0001f, _moveSpeed));
            _animator.SetFloat("MoveSpeed01", norm, 0.05f, Time.fixedDeltaTime);
        }
    }
    void GetCapsuleAt(Vector3 pos, out Vector3 p1, out Vector3 p2, out float radius)
    {
        var cap = _capsule;
        radius = cap.radius * transform.lossyScale.y;
        float height = Mathf.Max(cap.height * transform.lossyScale.y, radius * 2f);

        Vector3 center = pos + transform.rotation * Vector3.Scale(cap.center, transform.lossyScale);
        Vector3 up = transform.up;
        float half = (height * 0.5f) - radius;

        p1 = center + up * half;
        p2 = center - up * half;
    }
    protected override void MoveFinish()
    {
        _vel = Vector3.zero;
        _velDamp = Vector3.zero;
    }

    protected override void AttackInit()
    {
        VelocitySetting(0f);
        if (_animator != null) { _animator.SetTrigger(CommonStaticKey.ANIMPARAM_ATTACK); }
    }

    protected override void DieInit()
    {
        base.DieInit();
        if (_animator != null)
        {
            _animator.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 0f);
            _animator.SetTrigger(CommonStaticKey.ANIMPARAM_DEAD);
            _animator.SetBool(CommonStaticKey.ANIMPARAM_ISDEAD, true);
        }
    }
    public void SetMouseSensitivity(int userValue)
    {
        _mouseSensitivity = userValue * _sensitivityScale;
        Debug.Log($"마우스 민감도={userValue}"); //, 실제 민감도={_mouseSensitivity}");
    }

    // 보는 시야 업데이트 / 여기서는 방향만 위치한다
    void LookUpdate()
    {
        if (Mouse.current == null) { return; }
        Vector2 d = Mouse.current.delta.ReadValue();
        _yawWanted += d.x * _mouseSensitivity;
        _pitch -= d.y * _mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
    }

    void InteractUpdate()
    {
        if (_cameraPivot == null) { return; }
        if (!Keyboard.current[_interactKey].wasPressedThisFrame) { return; }

        Vector3 origin = _cameraPivot.position;
        Vector3 dir = _cameraPivot.forward;

        bool hasHit = false;
        RaycastHit hit;

        // 에임에 두깨를 주는것이면 이곳으로
        if (_interactRadius > 0f)
        {
            hasHit = Physics.SphereCast(origin, _interactRadius, dir, out hit, _interactDistance, _interactMask, QueryTriggerInteraction.Ignore);
        }
        else
        {
            hasHit = Physics.Raycast(origin, dir, out hit, _interactDistance, _interactMask, QueryTriggerInteraction.Ignore);
        }
        //Debug.DrawRay(origin, dir * _interactDistance, hasHit ? Color.green : Color.red); // 거리 시각화 (디버그용으로 필요시 활성화)

        //if (!Keyboard.current[_interactKey].wasPressedThisFrame) return;
        if (!hasHit) { /*Debug.Log("히트 없음");*/ return; }

        var interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable != null)
        {
            InteractionManager.GetInstance.SetTarget(interactable);
            InteractionManager.GetInstance.TryInteract(this);
            _inventoryUI?.Refresh();
            return;
        }

        Debug.Log($"상호작용 불가 대상: {hit.collider.name}");
    }

    // 거리 시각화 (디버그용으로 필요시 활성화)
    //void OnDrawGizmosSelected()
    //{
    //    if (_cameraPivot == null) return;
    //    Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
    //    if (_interactRadius > 0f)
    //    {
    //        Gizmos.DrawWireSphere(_cameraPivot.position, _interactRadius);
    //        Gizmos.DrawWireSphere(_cameraPivot.position + _cameraPivot.forward * _interactDistance, _interactRadius);
    //    }
    //    else
    //    {
    //        Gizmos.DrawLine(_cameraPivot.position, _cameraPivot.position + _cameraPivot.forward * _interactDistance);
    //    }
    //}

    // 상호작용 대상 알려주는 함수
    void DetectInteractTarget()
    {
        if (_cameraPivot == null) { return; }

        Vector3 origin = _cameraPivot.position;
        Vector3 dir = _cameraPivot.forward;

        bool hasHit = false;
        RaycastHit hit;
        hasHit = Physics.Raycast(origin, dir, out hit, _interactDistance, _interactMask, QueryTriggerInteraction.Ignore);

        if (_aim == null) { return; }

        if (hasHit)
        {
            if (!_aim.CheckTextActive)
            {
                _aim.TextActive(true);
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    InteractionManager.GetInstance.SetTarget(interactable);
                    _aim.TextStrSetting($"{InteractionManager.GetInstance.GetPrompt()}\n[{_interactKey.ToString()}]");
                }
                else
                {
                    _aim.TextStrSetting($"[{_interactKey.ToString()}]");
                }
            }
        }
        else
        {
            if (_aim.CheckTextActive) { _aim.TextActive(false); }
        }
    }

    void HotkeyUpdate()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectSlot(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectSlot(5);
    }

    void SelectSlot(int slotIndex)
    {
        if (_inventory == null) return;

        var slots = _inventory.GetSnapshot();
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        // 기존에 들고 있는 오브젝트 제거
        if (_currentHeld != null)
        {
            Destroy(_currentHeld);
            _currentHeld = null;
        }

        _currentSlot = slotIndex;

        // 슬롯에 아이템 없으면 빈손
        if (slots[slotIndex]._item == null) return;

        var itemData = slots[slotIndex]._item;
        if (itemData._holdPrefab == null) return;

        // 새 아이템 프리팹을 손 소켓에 장착
        _currentHeld = Instantiate(itemData._holdPrefab, _handSocket);
        _currentHeld.transform.localPosition = Vector3.zero;
        _currentHeld.transform.localRotation = Quaternion.identity;

        //Debug.Log($"슬롯 {slotIndex + 1} 선택 → {itemData._itemName} 장착");
    }

    // 인벤토리 열고 닫기 처리
    void InventoryToggleUpdate()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            _inventoryOpen = !_inventoryOpen;
            if (_inventoryUI != null) { _inventoryUI.InventoryRootActive(_inventoryOpen); }

            if (_inventoryOpen)
            {
                // 인벤토리 열림 상태
                UIManager.GetInstance.GetCursorManager.SetMode(ECursorMode.E_Inventory);
            }
            else
            {
                // 인벤토리 닫힘 상태
                UIManager.GetInstance.GetCursorManager.SetMode(ECursorMode.E_GamePlay);
            }
        }
    }
    void HandleInventoryChanged()
    {
        // 현재 손에 든 슬롯(_currentSlot)의 아이템이 사라졌는지 체크
        if (_currentSlot >= 0 && _inventory != null)
        {
            var slots = _inventory.GetSnapshot();

            // 슬롯이 비었거나 아이템이 바뀐 경우 손에서 제거
            if (_currentSlot >= slots.Length || slots[_currentSlot]._item == null)
            {
                //Debug.Log($"인벤토리 변경 감지 - 현재 손에 든 슬롯({_currentSlot})이 비었음 → 오브젝트 제거");
                if (_currentHeld != null)
                {
                    Destroy(_currentHeld);
                    _currentHeld = null;
                }
            }
            else
            {
                // 슬롯 아이템이 바뀐 경우도 교체 필요
                var newItem = slots[_currentSlot]._item;
                var heldName = _currentHeld != null ? _currentHeld.name : "(없음)";
                //Debug.Log($"인벤토리 변경 감지 - 슬롯 {_currentSlot} 내용 변경됨 → 손:{heldName}, 슬롯:{newItem._itemName}");

                // 슬롯 아이템이 현재 들고 있던 것과 다르면 교체
                if (_currentHeld == null || !_currentHeld.name.Contains(newItem._itemName))
                {
                    if (_currentHeld != null) Destroy(_currentHeld);
                    if (newItem._holdPrefab != null)
                    {
                        _currentHeld = Instantiate(newItem._holdPrefab, _handSocket);
                        _currentHeld.transform.localPosition = Vector3.zero;
                        _currentHeld.transform.localRotation = Quaternion.identity;
                        //Debug.Log($"손 아이템 갱신 - 새 오브젝트: {newItem._itemName}");
                    }
                }
            }
        }
    }

    #region 하루 초기화 관련
    void CacheInitTransform()
    {
        if (!_hasInitTransform)
        {
            _initPosition = _trans.position;
            _initRotation = _trans.rotation;
            _hasInitTransform = true;
        }

        if (_cameraPivot != null)
        {
            _initPitch = _cameraPivot.localEulerAngles.x;
        }
    }
    void EnsureRegisteredWithDayCycle()
    {
        if (_registerRoutine != null) { return; }

        if (UIManager.GetInstance.GetDayCycleManager != null)
        {
            UIManager.GetInstance.GetDayCycleManager.RegisterResettable(this);
        }
        else
        {
            _registerRoutine = StartCoroutine(RegisterWhenReady());
        }
    }
    IEnumerator RegisterWhenReady()
    {
        while (UIManager.GetInstance.GetDayCycleManager == null)
        {
            yield return null;
        }

        UIManager.GetInstance.GetDayCycleManager.RegisterResettable(this);
        _registerRoutine = null;
    }
    public void ResetForNewDay()
    {
        if (_inventory != null)
        {
            _inventory.ClearAll();
        }

        if (_inventoryUI != null)
        {
            _inventoryUI.Refresh();
            _inventoryUI.OnInventoryChanged?.Invoke();
        }

        if (_inventoryOpen)
        {
            _inventoryOpen = false;
            if (_inventoryUI != null)
            {
                _inventoryUI.InventoryRootActive(false);
            }
            _cursorManager?.SetMode(ECursorMode.E_GamePlay);
        }

        // 손에 들고 있었다면 오브젝트 없애주기
        if (_currentHeld != null)
        {
            Destroy(_currentHeld);
            _currentHeld = null;
        }
        _currentSlot = -1;

        _moveInput = Vector2.zero;
        _vel = Vector3.zero;
        _velDamp = Vector3.zero;

        if (_agent != null)
        {
            _agent.ResetPath();
            _agent.nextPosition = _initPosition;
        }

        if (_rigidbody != null)
        {
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.position = _initPosition;
            _rigidbody.rotation = _initRotation;
        }

        _trans.SetPositionAndRotation(_initPosition, _initRotation);

        _yawWanted = _initRotation.eulerAngles.y;
        _pitch = Mathf.Clamp(_initPitch, _pitchMin, _pitchMax);
        if (_cameraPivot != null)
        {
            _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        if (_yawIsOnRoot && _rigidbody != null)
        {
            _rigidbody.MoveRotation(_initRotation);
        }
        else if (_yawSource != null)
        {
            _yawSource.rotation = _initRotation;
        }

        if (_curState != eCharacterStates.Idle)
        {
            SetState(eCharacterStates.Idle);
        }
    }
    #endregion

    public bool InventoryCheck { get { return _inventoryOpen; } }
}
