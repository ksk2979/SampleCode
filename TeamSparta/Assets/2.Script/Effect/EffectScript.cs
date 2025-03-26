using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    public float _time = 2f;
    private void OnEnable()
    {
        Invoke("DoUpdate", _time);
    }

    void DoUpdate()
    {
        this.gameObject.SetActive(false);
    }
}
