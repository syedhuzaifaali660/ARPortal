using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    
    public Material[] materials;

    public Transform Device;

    bool WasInFront;
    bool InOtherWorld;


    // Start is called before the first frame update
    void Start()
    {
        SetMaterials(false);
    }
    void SetMaterials(bool FullRender)
    {
        var stencilTest = FullRender ? CompareFunction.NotEqual : CompareFunction.Equal;
        Shader.SetGlobalInt("_StencilTest", (int)stencilTest);
        //foreach (var mat in materials)
        //{
        //    mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        //}


    }

    bool GetIsInFront()
    {
        Vector3 pos = transform.InverseTransformPoint(Device.position);
        return pos.z >= 0? true: false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != Device)
            return;

        WasInFront = GetIsInFront();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform !=Device)
        {
            return; 
        }

        bool IsInFront = GetIsInFront();

        if ((IsInFront && !WasInFront) || (WasInFront && !IsInFront))
        {
            InOtherWorld = !InOtherWorld;
            SetMaterials(InOtherWorld);
        }
        WasInFront = IsInFront;
    }

    private void OnDestroy()
    {
        SetMaterials(true);
    }

}
