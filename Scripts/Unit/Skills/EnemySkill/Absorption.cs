using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 흡수 스킬
public class Absorption : MonoBehaviour
{
    StageManager _stage;
    List<CharacterController> _playerObj = new List<CharacterController>();
    List<EnemyController> _enemyObj = new List<EnemyController>();
    public float _power = 0.01f; // 0.1 까지 파워업
    float _time = 0;
    float _maxTime = 0.25f;
    List<GameObject> _absorptionObj = new List<GameObject>(); // 흡수된 오브젝트들
    public bool _endLoopSkill = false;
    BossType1Controller _boss;

    private void OnEnable()
    {
        if (_boss == null) { _boss = this.transform.parent.transform.GetComponent<BossType1Controller>(); }
        if (GameManager.GetInstance.TestEditor)
        {
            SpawnTestManager test = SpawnTestManager.GetInstance;
            for (int i = 0; i < test.GetPlayerList().Count; ++i)
            {
                if (!test.GetPlayerList()[i].GetCharacterCtrl().IsDie())
                {
                    _playerObj.Add(test.GetPlayerList()[i].GetCharacterCtrl());
                }
            }
        }
        else
        {
            if (_stage == null) { _stage = StageManager.GetInstance; }
            for (int i = 0; i < _stage.GetPlayersController().Count; ++i)
            {
                if (!_stage.GetPlayersController()[i].IsDie())
                {
                    _playerObj.Add(_stage.GetPlayersController()[i]);
                }
            }
            for (int i = 0; i < _stage._chapter[_stage._cc].GetEnemys().Count; ++i)
            {
                _enemyObj.Add(_stage._chapter[_stage._cc].GetEnemys()[i]);
            }
        }
        
        //Debug.Log("현재 적의 수 : " + _enemyObj.Count);
        _power = 0.01f;
        _time = 0;
        _endLoopSkill = false;
        NotTargeting(true);
    }

    private void OnDisable()
    {
        for (int i = 0; i < _absorptionObj.Count; ++i)
        {
            _absorptionObj[i].SetActive(true);
            // 애니메이션 실행하면서 원래 자리로 되돌려 놓자구
            if (_absorptionObj[i] != null)
            {
                CharacterController character = _absorptionObj[i].GetComponent<CharacterController>();
                if (GameManager.GetInstance.TestEditor) { character.GetMovement.AgentWarp(SpawnTestManager.GetInstance.GetRandomPos()); }
                else { character.GetMovement.AgentWarp(_stage._chapter[_stage._cc].GetRandomPos()); }
                character.AbsorptionAnime(_boss.GetEnemyStat().GetTDD4());
            }
        }
        NotTargeting(false);
    }

    void NotTargeting(bool notTarget)
    {
        if (notTarget)
        {
            for (int i = 0; i < _playerObj.Count; ++i)
            {
                _playerObj[i].tag = CommonStaticDatas.TAG_UNTAGGED;
            }
            for (int i = 0; i < _enemyObj.Count; ++i)
            {
                _enemyObj[i].tag = CommonStaticDatas.TAG_UNTAGGED;
                if (_enemyObj[i].Detecter != null) { _enemyObj[i].Detecter.SetActive(false); }
                _enemyObj[i].SetState(eCharacterStates.Drop);
            }
        }
        else
        {
            for (int i = 0; i < _playerObj.Count; ++i)
            {
                Player player = _playerObj[i].GetComponent<Player>();
                player._PlayerController.TagSetting(CommonStaticDatas.TAG_PLAYER);
                player._playerStats._hpBar.SetActive(true);
            }
            for (int i = 0; i < _enemyObj.Count; ++i)
            {
                Enemy enemy = _enemyObj[i].GetComponent<Enemy>();
                if (!_enemyObj[i].IsDie())
                {
                    enemy._enemyController.TagSetting(CommonStaticDatas.TAG_ENEMY);//Fish_Normal_001 (1)
                    if (_enemyObj[i].Detecter != null) { _enemyObj[i].Detecter.SetActive(true); }
                }
            }

            _absorptionObj.Clear();
            _playerObj.Clear();
            _enemyObj.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(CommonStaticDatas.TAG_FLOOR) && !other.CompareTag(CommonStaticDatas.TAG_BULLET) && !other.CompareTag(CommonStaticDatas.TAG_ABILITY))
        {
            if (other.GetComponent<PlayerStats>() != null) { if (other.transform.GetComponent<PlayerStats>()._hpBar != null) { other.transform.GetComponent<PlayerStats>()._hpBar.SetActive(false); } }
            _absorptionObj.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
    // 빨아 들이는 것에 닿게 된다면?
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.GetComponent<PlayerStats>() != null) { collision.transform.GetComponent<PlayerStats>()._hpBar.SetActive(false); }
    //    _absorptionObj.Add(collision.gameObject);
    //    collision.gameObject.SetActive(false);
    //}

    // 물리 업데이트로 오브젝트 들을 빨아들여보자
    private void FixedUpdate()
    {
        if (_endLoopSkill) { return; }

        if (_power < 0.28f)
        {
            _time += Time.deltaTime;
            if (_time > _maxTime) { _time = 0; _power += 0.02f; }
        }
        
        for (int i = 0; i < _playerObj.Count; ++i)
        {
            _playerObj[i].transform.position = Vector3.MoveTowards(_playerObj[i].transform.position, this.transform.position, _power);
        }
        for (int i = 0; i < _enemyObj.Count; ++i)
        {
            _enemyObj[i].transform.position = Vector3.MoveTowards(_enemyObj[i].transform.position, this.transform.position, _power * 2f);
        }
    }
}
