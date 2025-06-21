using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Init() { }
    public virtual void Init(double damage, double hp, int wave) { }
    public virtual void TakeToDamage(double damage) { }
    public virtual void StayToDamage(double damage, float delayTime, EStayDamageType type) { }
}
