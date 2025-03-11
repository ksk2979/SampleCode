using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateHandler : MonoBehaviour
{
    public abstract void OnStart();

    protected virtual void SpawnInit() { }
    protected virtual void SpawnUpdate() { }
    protected virtual void SpawnFinish() { }

    protected virtual void IdleInit() { }
    protected virtual void IdleUpdate() { }
    protected virtual void IdleFinish() { }

    protected virtual void MoveInit() { }
    protected virtual void MoveUpdate() { }
    protected virtual void MoveFinish() { }



    protected virtual void AttackInit() { }
    protected virtual void AttackUpdate() { }
    protected virtual void AttackFinish() { }

    protected virtual void TraceInit() { }
    protected virtual void TraceUpdate() { }
    protected virtual void TraceFinish() { }

    protected virtual void FindTargetInit() { }

    protected virtual void FindTargetUpdate() { }
    protected virtual void FindTargetFinish() { }

    protected virtual void DieInit() { }
    protected virtual void DieUpdate() { }



    protected virtual void DieFinish() { }

    protected virtual void Attack01Init() { }
    protected virtual void Attack01Update() { }
    protected virtual void Attack01Finish() { }


    protected virtual void Attack02Init() { }
    protected virtual void Attack02Update() { }
    protected virtual void Attack02Finish() { }

    protected virtual void SummonInit() { }
    protected virtual void SummonUpdate() { }
    protected virtual void SummonFinish() { }

    protected virtual void KnockBackInit() { }
    protected virtual void KnockBackUpdate() { }
    protected virtual void KnockBackFinish() { }

    protected virtual void KnockDownInit() { }
    protected virtual void KnockDownUpdate() { }
    protected virtual void KnockDownFinish() { }

    protected virtual void SternInit() { }
    protected virtual void SternUpdate() { }
    protected virtual void SternFinish() { }

    protected virtual void ShockInit() { }
    protected virtual void ShockUpdate() { }
    protected virtual void ShockFinish() { }

    protected virtual void DashJumpInit() { }
    protected virtual void DashJumpUpdate() { }
    protected virtual void DashJumpFinish() { }

    protected virtual void ShortIdleInit() { }
    protected virtual void ShortIdleUpdate() { }
    protected virtual void ShortIdleFinish() { }

    protected virtual void HideInit() { }
    protected virtual void HideUpdate() { }
    protected virtual void HideFinish() { }

    protected virtual void RepairInit() { }
    protected virtual void RepairUpdate() { }
    protected virtual void RepairFinish() { }

    protected virtual void DropInit() { }
    protected virtual void DropUpdate() { }
    protected virtual void DropFinish() { }
}