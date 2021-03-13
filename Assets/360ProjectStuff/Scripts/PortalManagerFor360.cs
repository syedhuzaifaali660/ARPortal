using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalManagerFor360 : MonoBehaviour
{
    public GameObject MainCam;
    public GameObject MainObject;
    private Material[] SponzaMaterial;

    private Material PortalPlane2Material;

    // Start is called before the first frame update
    void Start()
    {
        SponzaMaterial = MainObject.GetComponent<Renderer>().sharedMaterials;
        PortalPlane2Material = GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void OnTriggerStay (Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCam.transform.position);


        if(camPositionInPortalSpace.y <= 0.0f)
        {
            for (int i = 0; i < SponzaMaterial.Length; i++)
            {
                SponzaMaterial[i].SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }

            PortalPlane2Material.SetInt("_CullMode", (int)CullMode.Front);
        }

        else if(camPositionInPortalSpace.y < 0.3f)
        {
            for(int i=0; i< SponzaMaterial.Length; i++)
            {
                SponzaMaterial[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }

            PortalPlane2Material.SetInt("_CullMode", (int)CullMode.Off);
        }
        else
        {
            for (int i = 0; i < SponzaMaterial.Length; i++)
            {
                SponzaMaterial[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }

            PortalPlane2Material.SetInt("_CullMode", (int)CullMode.Back);
        }
    }
}
