using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfObjectisHitThenEnableTouchMove : MonoBehaviour
{

    GameObject cube;
    GameObject capsule;

    private void Start()
    {
        cube =  GameObject.Find("Cube");
        capsule =  GameObject.Find("Capsule");
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Select Stage
                if (hit.transform.name == "Cube")
                {
                    cube.GetComponent<MoveWithTouchCustom>().enabled = true;
                    capsule.GetComponent<MoveWithTouchCustom>().enabled = false;

                }
                else if (hit.transform.name == "Capsule")
                {
                    cube.GetComponent<MoveWithTouchCustom>().enabled = false;
                    capsule.GetComponent<MoveWithTouchCustom>().enabled = true;
                }
                else
                {
                    cube.GetComponent<MoveWithTouchCustom>().enabled = false;
                    capsule.GetComponent<MoveWithTouchCustom>().enabled = false;
                }
            }
        }
        
    }
}
