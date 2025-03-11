using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBuilding : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    private Material[] mats;

    private Material defaultMat;
    public Material changeMat;

    public bool IsShowCamera = false;
    private float flowTime = 0;
    private float changeTime = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        defaultMat = meshRenderer.materials[0];
        mats = meshRenderer.materials;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (IsShowCamera)
        {
            flowTime += Time.deltaTime;
            if(changeTime < flowTime)
            {
                mats[0] = defaultMat;
                meshRenderer.materials = mats;
                flowTime = 0;
            }
        }
    }

    public void OnCamera()
    {
        mats[0] = changeMat;
        meshRenderer.materials = mats;
        IsShowCamera = true;
        flowTime = 0;
    }
}
