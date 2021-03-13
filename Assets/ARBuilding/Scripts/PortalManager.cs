using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalManager : MonoBehaviour
{
    public GameObject MainCam;
    public GameObject Sponza;
    private Material[] SponzaMaterial;




    // Start is called before the first frame update
    void Start()
    {
        SponzaMaterial = Sponza.GetComponent<Renderer>().sharedMaterials;

    }

    // Update is called once per frame
    void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCam.transform.position);

        if(camPositionInPortalSpace.y < 0.5f)
        {
            for(int i=0; i< SponzaMaterial.Length; i++)
            {
                SponzaMaterial[i].SetInt("_StencilCompARBuilding", (int)CompareFunction.Always);
            }

        }
        else
        {
            for (int i = 0; i < SponzaMaterial.Length; i++)
            {
                SponzaMaterial[i].SetInt("_StencilCompARBuilding", (int)CompareFunction.Equal);
            }

        }
    }
}
