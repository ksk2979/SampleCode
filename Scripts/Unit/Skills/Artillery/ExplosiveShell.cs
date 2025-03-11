using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ź ���������� ���� �� (0: ��, 1: ��, 2: ���� 3: ����)
public class ExplosiveShell : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    float _time = 0f;
    int _number;

    public void Init(UnitDamageData data, int number)
    {
        _obj = this.gameObject;
        _trans = this.transform;
        tdd = data;
        _trans.SetParent(null);
        _number = number;
    }

    public void StartAbility(Vector3 pos)
    {
        _obj.SetActive(true);
        _trans.position = pos;
        _trans.position = new Vector3(pos.x, 0f, pos.z);
        _time = 0f;
    }
    public void EndAbility()
    {
        _obj.SetActive(false);
        _time = 0f;
    }

    private void FixedUpdate()
    {
        _time += Time.deltaTime;
        if (_time > 3f)
        {
            EndAbility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                var unit = other.GetComponent<Interactable>();
                if (unit != null)
                {
                    if (unit._unit != UNIT.Boss)
                    {
                        // �� �Ǵ�
                        if (_number == 0)
                        {
                            if (!unit.GetController().GetStatusEffects.GetDotOn) { unit.GetController().GetStatusEffects.DotOn(tdd.damage); }
                            else { unit.GetController().GetStatusEffects.DotReAttack(); }
                        }
                        // �� ������
                        else if (_number == 1)
                        {
                            if (!unit.GetController().GetStatusEffects.GetDotOn) { unit.GetController().GetStatusEffects.DotOn(tdd.damage); }
                            else { unit.GetController().GetStatusEffects.DotReAttack(); }
                        }
                        // ���� ����
                        else if (_number == 2)
                        {
                            if (unit.GetController()._curState != eCharacterStates.Shock) { unit.GetController().SetState(eCharacterStates.Shock); }
                            else { unit.GetController().GetStatusEffects.ShockAttack(); }
                        }
                        // ���ο� ����
                        else if (_number == 3)
                        {
                            if (!unit.GetController().GetStatusEffects.GetSlowOn) { unit.GetController().GetStatusEffects.SlowOn(); }
                            else { unit.GetController().GetStatusEffects.SlowReAttack(); }
                        }
                    }
                }

                if (other.GetComponent<Interactable>() == null) { other.GetComponent<TentacleScript>().HitDamage(tdd.damage); }
                else { other.GetComponent<Interactable>().TakeToDamage(tdd); }
            }
        }
    }
}
