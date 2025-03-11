using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 돌진 선공 타입2
/// </summary>

public class EnemyType2Controller : EnemyController
{
    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _oneOnStartInit = true;
        }
        SetState(eCharacterStates.Spawn);
    }
}
