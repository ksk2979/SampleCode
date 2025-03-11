using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStayTargets : MonoBehaviour
{
    public string[] m_targetTags;
    protected List<Collider> targets = new List<Collider>();
    protected List<CharacterController> targetsController = new List<CharacterController>();
    private SphereCollider m_colider;
    public void Init(string[] targets, float radius)
    {
        m_targetTags = targets;
        if(m_colider == null)
            m_colider = GetComponent<SphereCollider>();
        m_colider.radius = radius;
    }

    public bool IsHaveDetectTarget()
    {
        return 0 < targets.Count;
    }

    private void OnDisable()
    {
        if (0 < targets.Count)
        {
            targets.Clear();
            targetsController.Clear();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, m_targetTags))
        {
            if (targets.Contains(other) == false)
            {
                targets.Add(other);
                targetsController.Add(other.transform.GetComponent<CharacterController>());
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, m_targetTags))
        {
            if (targets.Contains(other))
            {
                targets.Remove(other);
                targetsController.Remove(other.transform.GetComponent<CharacterController>());
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targetsController[i].IsDie())
            {
                targets.RemoveAt(i);
                targetsController.RemoveAt(i);
            }
        }
    }

    public List<Collider> GetTargetList()
    {
        return targets;
    }
}
