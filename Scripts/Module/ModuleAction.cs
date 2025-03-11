using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 모듈 액션 테스트했던 스크립트 (없애도 됨)
public class ModuleAction : MonoBehaviour
{
    // 페이드 인 아웃 컴포넌트
    FadeInOutAction _uiFade;
    FadeInOutAction _spriteFade;
    FadeInOutAction _cubeFade;
    FadeInOutAction _textFade;

    // 움직임 컴포넌트
    MoveAction _uiMove;
    MoveAction _spriteMove;
    MoveAction _cubeMove;
    MoveAction _textMove;

    // 일반적인 바운스 컴포넌트
    BounceAction _spriteBounce;
    BounceAction _uiBounce;
    BounceAction _cubeBounce;
    BounceAction _textBounce;

    // 컬러 변경
    ColorChangeAction _spriteColor;
    ColorChangeAction _textColor;
    ColorChangeAction _uiColor;

    // 쉐이크
    ShakeAction _cameraShake;
    ShakeAction _spriteShake;
    ShakeAction _uiShake;
    ShakeAction _textShake;
    ShakeAction _cubeShake;

    // 블러
    GameObject _blurObj;

    // 카메라 타겟
    CameraController _cameraController;

    // 장착한(?) 오브젝트
    GameObject _targetObj;

    // 플로팅 오브젝트
    [SerializeField] GameObject _floatingObj;

    // 타이핑 오브젝트
    TypingText _typingTT;

    bool _moveOn = false;
    bool _fadeAPOutOn = false;
    bool _fadeOutOn = false;
    bool _bounceOutOn = false;
    bool _colorOn = false;
    bool _blurOn = false;
    bool _cameraTargetOn = false;

    [SerializeField] float _shakeA;
    [SerializeField] float _shakeT;

    // 터지는 효과 오브젝트 풀링
    PoppingPooling _poppingPooling;
    PoppingPooling _uiPoppingPooling;

    private void Start()
    {
        //_uiFade = GameObject.Find("Canvas").transform.Find("Image").GetComponent<FadeInOutAction>();
        //_spriteFade = GameObject.Find("SpriteImage").GetComponent<FadeInOutAction>();
        //_cubeFade = GameObject.Find("Cube").GetComponent<FadeInOutAction>();
        //_textFade = GameObject.Find("Canvas").transform.Find("Text").GetComponent<FadeInOutAction>();
        //
        //_uiMove = _uiFade.GetComponent<MoveAction>();
        //_spriteMove = _spriteFade.GetComponent<MoveAction>();
        //_cubeMove = _cubeFade.GetComponent<MoveAction>();
        //_textMove = _textFade.GetComponent<MoveAction>();
        //
        //_spriteBounce = _spriteFade.GetComponent<BounceAction>();
        //_uiBounce = _uiFade.GetComponent<BounceAction>();
        //_cubeBounce = _cubeFade.GetComponent<BounceAction>();
        //_textBounce = _textFade.GetComponent<BounceAction>();
        //
        //_spriteColor = _spriteFade.GetComponent<ColorChangeAction>();
        //_textColor = _textFade.GetComponent<ColorChangeAction>();
        //_uiColor = _uiFade.GetComponent<ColorChangeAction>();
        //
        //_cameraShake = Camera.main.GetComponent<ShakeAction>();
        //_spriteShake = _spriteFade.GetComponent<ShakeAction>();
        //_uiShake = _uiFade.GetComponent<ShakeAction>();
        //_textShake = _textFade.GetComponent<ShakeAction>();
        //_cubeShake = _cubeFade.GetComponent<ShakeAction>();
        //
        //_blurObj = GameObject.Find("Blur").transform.Find("BlurCamera").gameObject;
        //
        //_cameraController = _cameraShake.GetComponent<CameraController>();
        //
        //_targetObj = _spriteFade.gameObject;
        //
        //_typingTT = GameObject.Find("Canvas").transform.Find("TypingText").GetComponent<TypingText>();
        //
        //_uiFade.gameObject.SetActive(false);
        //_cubeFade.gameObject.SetActive(false);
        //_textColor.gameObject.SetActive(false);
        //
        //_addText = GameObject.Find("Canvas").transform.Find("TextTest").GetComponent<Text>();

        _poppingPooling = GameObject.Find("Popping").GetComponent<PoppingPooling>();
        _uiPoppingPooling = GameObject.Find("UICanvas").transform.Find("UIPopping").GetComponent<PoppingPooling>();
    }

    //MyStructData.STTest _stTest;
    public int _addValue = 0;
    public string _addName = "Test";
    public bool _addBool = false;
    public float _addMoney = 10.1f;
    public Text _addText;
    //public void SaveData()
    //{
    //    JsonManager.GetInstance.JsonInit("TestJson", "JsonTest");
    //    _stTest._id = _addValue;
    //    _stTest._name = _addName + _addValue;
    //    _stTest._check = _addBool;
    //    _stTest._money = _addMoney;
    //    JsonManager.GetInstance.JsonInfo(_stTest);
    //    JsonManager.GetInstance.JsonSave();
    //    _addText.text = "저장하였습니다\n" + _stTest._id + "\n" + _stTest._name + "\n" + _stTest._check + "\n" + _stTest._money;
    //}
    //public void LoadData()
    //{
    //    JsonManager.GetInstance.JsonLoad(ref _stTest, "TestJson", "JsonTest");
    //    _addText.text = "로드하였습니다\n" + _stTest._id + "\n" + _stTest._name + "\n" + _stTest._check + "\n" + _stTest._money;
    //}
    public void AddData()
    {
        _addValue++;
        _addBool = _addBool ? false : true;
        _addMoney++;
    }

    public void ResetModule()
    {
        if (_targetObj.GetComponent<RectTransform>() != null)
        {
            _targetObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
            _targetObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            _targetObj.transform.position = new Vector3(0f, 0f, 0f);
            _targetObj.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (_targetObj.GetComponent<Text>() != null)
        {
            _targetObj.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
        }
        else if (_targetObj.GetComponent<Image>() != null)
        {
            _targetObj.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        else if (_targetObj.GetComponent<SpriteRenderer>() != null)
        {
            _targetObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
        // 큐브는 컬러를 안할것이라 없음

        _blurObj.SetActive(false); // 블러 꺼주고
        ObjActiveReset(); // 오브젝트 처음 스프라이트로이미지 리셋시키고
        _targetObj = _spriteFade.gameObject; // 스프라이트이미지 타겟 넣기
        
        _moveOn = false;
        _fadeOutOn = false;
        _bounceOutOn = false;
        _fadeAPOutOn = false;
        _colorOn = false;
        _blurOn = false;
        _cameraTargetOn = false;

        // 메뉴초기화
        DuplicateVariablesObj(true);
        _objSelect.SetActive(false);
        _funtionSelect.SetActive(false);
        _funtion2Button.SetActive(true);
        _funtion2Select.SetActive(false);

        // 타이핑 초기화
        _typingTT.TypingSkip = false;
        _typingTT.gameObject.SetActive(false);
        _typingCount = 0;

        // 카메라 위치 초기화
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
    }

    void ObjActiveReset()
    {
        _spriteFade.gameObject.SetActive(true);
        _cubeFade.gameObject.SetActive(false);
        _textFade.gameObject.SetActive(false);
        _uiFade.gameObject.SetActive(false);
    }

    [SerializeField] GameObject _objButton;
    [SerializeField] GameObject _funtionButton;
    [SerializeField] GameObject _funtion2Button;
    [SerializeField] GameObject _objSelect;
    [SerializeField] GameObject _funtionSelect;
    [SerializeField] GameObject _funtion2Select;
    [SerializeField] GameObject _uiPoppingPrefabs;
    [SerializeField] GameObject _spPoppingPrefabs;
    [SerializeField] GameObject _spPoppingTargetPrefabs;
    public void ObjectSelectButton()
    {
        DuplicateVariablesObj(false);
        _objSelect.SetActive(true);
    }
    public void ObjectSelectCloseButton()
    {
        DuplicateVariablesObj(true);
        _objSelect.SetActive(false);
    }
    public void FuntionSelectButton()
    {
        DuplicateVariablesObj(false);
        _funtionSelect.SetActive(true);
        
        if (_targetObj.GetComponent<ColorChangeAction>() == null)
        {
            _funtionSelect.transform.Find("ColorButton").gameObject.SetActive(false);
        }
        else
        {
            _funtionSelect.transform.Find("ColorButton").gameObject.SetActive(true);
        }
    }
    public void FuntionSelectCloseButton()
    {
        DuplicateVariablesObj(true);
        _funtionSelect.SetActive(false);
    }
    public void FuntionSelect2Button()
    {
        _funtion2Button.gameObject.SetActive(false);
        _funtion2Select.gameObject.SetActive(true);
    }
    public void Funtion2SelectCloseButton()
    {
        _funtion2Button.gameObject.SetActive(true);
        _funtion2Select.gameObject.SetActive(false);
    }

    public void DuplicateVariablesObj(bool active)
    {
        _objButton.gameObject.SetActive(active);
        _funtionButton.gameObject.SetActive(active);
    }

    // 터지는 효과 오브젝트 풀링
    public int _poppingCount = 1;
    public void SpritePoppingPooling()
    {
        for (int i = 0; i < _poppingCount; ++i)
        {
            _poppingPooling.PoppingStart(new Vector3(0, 0, 0));
        }
    }
    public void UIPoppingPooling()
    {
        for (int i = 0; i < _poppingCount; ++i)
        {
            _uiPoppingPooling.PoppingStart(new Vector3(0, 0, 0));
        }
    }

    // 터지는 효과 일반적으로 사용했을 경우
    public void UIPopping()
    {
        GameObject obj = Instantiate(_uiPoppingPrefabs, transform);

        //obj.transform.localPosition = new Vector3(300f, 400f, 0f);
    }
    GameObject _moneyParnt;
    public void SpritePopping()
    {
        if (_moneyParnt == null) { _moneyParnt = GameObject.Find("MoneyObj").gameObject; }
        GameObject obj = Instantiate(_spPoppingPrefabs, _moneyParnt.transform);

        //obj.transform.localPosition = new Vector3(300f, 400f, 0f);
    }
    // 터진후 타겟팅한 곳으로 이동
    public void SpritePoppingTarget()
    {
        if (_moneyParnt == null) { _moneyParnt = GameObject.Find("MoneyObj").gameObject; }
        GameObject obj = Instantiate(_spPoppingTargetPrefabs, _moneyParnt.transform);
        obj.GetComponent<TargetMoveAction>().TargetTr = GameObject.Find("Target").GetComponent<Transform>();
        obj.transform.localPosition = new Vector3(-1.9f, -3.39f, 0f);
    }

    public void TargetObjSelect(int select)
    {
        if (_targetObj != null) { _targetObj.SetActive(false); }
        if (select == 0)
        {
            _targetObj = _spriteFade.gameObject;
        }
        else if (select == 1)
        {
            _targetObj = _uiFade.gameObject;
        }
        else if (select == 2)
        {
            _targetObj = _textFade.gameObject;
        }
        else
        {
            _targetObj = _cubeFade.gameObject;
        }
        _targetObj.SetActive(true);
    }

    public void BounceFadeOutAction()
    {
        if (_bounceOutOn)
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteFade.ScaleUpAndBounceAction(1f));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textFade.ScaleUpAndBounceAction(1f));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiFade.ScaleUpAndBounceAction(1f));
            }
            else
            {
                StartCoroutine(_cubeFade.ScaleUpAndBounceAction(1f));
            }
            _bounceOutOn = false;
        }
        else
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteFade.ScaleDownAndOutActoin(0.3f));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textFade.ScaleDownAndOutActoin(0.3f));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiFade.ScaleDownAndOutActoin(0.3f));
            }
            else
            {
                StartCoroutine(_cubeFade.ScaleDownAndOutActoin(0.3f));
            }
            _bounceOutOn = true;
        }
    }

    public void FadeAPOutAtion()
    {
        if (_fadeAPOutOn)
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteFade.AlphaFadeAction(true));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textFade.AlphaFadeAction(true));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiFade.AlphaFadeAction(true));
            }
            else
            {
                StartCoroutine(_cubeFade.AlphaFadeAction(true));
            }
            _fadeAPOutOn = false;
        }
        else
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteFade.AlphaFadeAction(false));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textFade.AlphaFadeAction(false));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiFade.AlphaFadeAction(false));
            }
            else
            {
                StartCoroutine(_cubeFade.AlphaFadeAction(false));
            }
            _fadeAPOutOn = true;
        }
    }
    public void FadeOutAction()
    {
        if (_fadeOutOn)
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteFade.ScaleFadeAction(true));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textFade.ScaleFadeAction(true));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiFade.ScaleFadeAction(true));
            }
            else
            {
                StartCoroutine(_cubeFade.ScaleFadeAction(true));
            }
            _fadeOutOn = false;
        }
        else
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteFade.ScaleFadeAction(false));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textFade.ScaleFadeAction(false));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiFade.ScaleFadeAction(false));
            }
            else
            {
                StartCoroutine(_cubeFade.ScaleFadeAction(false));
            }
            _fadeOutOn = true;
        }
    }
    public void MoveAction()
    {
        if (_moveOn)
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteMove.RunAction(false));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textMove.RunAction(false));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiMove.RunAction(false));
            }
            else
            {
                StartCoroutine(_cubeMove.RunAction(false));
            }
            _moveOn = false;
        }
        else
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteMove.RunAction(true));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textMove.RunAction(true));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiMove.RunAction(true));
            }
            else
            {
                StartCoroutine(_cubeMove.RunAction(true));
            }
            _moveOn = true;
        }
    }
    public void BounceAction()
    {
        if (_targetObj.GetComponent<SpriteRenderer>() != null)
        {
            StartCoroutine(_spriteBounce.RunAction(1f));
        }
        else if (_targetObj.GetComponent<Text>() != null)
        {
            StartCoroutine(_textBounce.RunAction(1f));
        }
        else if (_targetObj.GetComponent<Image>() != null)
        {
            StartCoroutine(_uiBounce.RunAction(1f));
        }
        else
        {
            StartCoroutine(_cubeBounce.RunAction(1f));
        }
    }
    public void ColorChangeAction()
    {
        if (_colorOn)
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteColor.RunAction(new Color(1f, 1f, 1f, 1f)));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textColor.RunAction(new Color(1f, 1f, 1f, 1f)));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiColor.RunAction(new Color(1f, 1f, 1f, 1f)));
            }
            _colorOn = false;
        }
        else
        {
            if (_targetObj.GetComponent<SpriteRenderer>() != null)
            {
                StartCoroutine(_spriteColor.RunAction(new Color(1f, 0.5f, 1f, 1f)));
            }
            else if (_targetObj.GetComponent<Text>() != null)
            {
                StartCoroutine(_textColor.RunAction(new Color(1f, 0.5f, 1f, 1f)));
            }
            else if (_targetObj.GetComponent<Image>() != null)
            {
                StartCoroutine(_uiColor.RunAction(new Color(1f, 0.5f, 1f, 1f)));
            }
            _colorOn = true;
        }
    }
    public void CameraShakeAction()
    {
        StartCoroutine(_cameraShake.RunAction(_shakeA, _shakeT));
    }
    public void ShakeAction()
    {
        if (_targetObj.GetComponent<SpriteRenderer>() != null)
        {
            StartCoroutine(_spriteShake.RunAction(_shakeA, _shakeT));
        }
        else if (_targetObj.GetComponent<Text>() != null)
        {
            StartCoroutine(_textShake.RunAction(_shakeA, _shakeT));
        }
        else if (_targetObj.GetComponent<Image>() != null)
        {
            StartCoroutine(_uiShake.RunAction(_shakeA + 100f, _shakeT));
        }
        else
        {
            StartCoroutine(_cubeShake.RunAction(_shakeA, _shakeT));
        }
    }
    public void BlurAction()
    {
        if (_blurOn)
        {
            _blurObj.SetActive(false);
            _blurOn = false;
        }
        else
        {
            _blurObj.SetActive(true);
            _blurOn = true;
        }
    }
    public void CameraTargetFixingAction()
    {
        if (_cameraTargetOn)
        {
            _cameraController.CameraUnTargetObj();
            _cameraTargetOn = false;
        }
        else
        {
            _cameraController.CameraTargetObjact(_targetObj);
            _cameraTargetOn = true;
        }
    }
    public void CameraTargetMoveSettingChange()
    {
        _cameraController.ChangeTargetMoveSetting();
    }

    public void DamageMoveAction()
    {
        GameObject obj = Instantiate(_floatingObj, transform);

        obj.transform.localPosition = new Vector3(300f, 400f, 0f);
    }

    int _typingCount = 0;
    public void TypingAction()
    {
        if (_typingCount == 0)
        {
            _typingTT.gameObject.SetActive(true);
            StartCoroutine(_typingTT.Typing(_typingTT.GetComponent<Text>(), "멍멍 멍멍! 멍멍멍!", 0.05f));
            _typingCount++;
        }
        else if (_typingCount == 1)
        {
            if (_typingTT.NowTyping)
            {
                _typingTT.TypingSkip = true;
                _typingCount++;
            }
            else
            {
                _typingTT.gameObject.SetActive(false);
                _typingCount = 0;
            }
        }
        else if (_typingCount == 2)
        {
            _typingTT.gameObject.SetActive(false);
            _typingCount = 0;
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        ResetModule();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        BounceFadeOutAction();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        FadeAPOutAtion();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        FadeOutAction();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        MoveAction();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        BounceAction();   
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        ColorChangeAction();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        CameraShakeAction();
    //    }
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        ShakeAction();
    //    }
    //
    //    // 블러넣어주기
    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        BlurAction();
    //    }
    //
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        CameraTargetFixingAction();
    //    }
    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        CameraTargetMoveSettingChange();
    //    }
    //}


}
