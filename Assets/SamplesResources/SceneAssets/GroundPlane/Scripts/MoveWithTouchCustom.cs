using Vuforia;
using UnityEngine;

public class MoveWithTouchCustom : MonoBehaviour
{
    public GameObject Gobj;
    public bool IsPlaced { get; private set; }

    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;
    GameObject cube;
    TouchHandler touchHandler;

    string floorName;
    // Start is called before the first frame update

    void Start()
    {
        this.mainCamera = Camera.main;
        this.touchHandler = FindObjectOfType<TouchHandler>();

        cube = GameObject.Find("Cube");

        SetupFloor();
    }

    // Update is called once per frame
    void Update()
    {

        if (PlaneManager.CurrentPlaneMode == PlaneManager.PlaneMode.GROUND /*&& this.IsPlaced*/)
        {
            if (TouchHandler.IsSingleFingerDragging || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
            {

                this.cameraToPlaneRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(this.cameraToPlaneRay, out this.cameraToPlaneHit))
                {


                    if (this.cameraToPlaneHit.collider.gameObject.name == floorName)
                    {

                        this.Gobj.PositionAt(cameraToPlaneHit.point);
                    }
                    else
                    {
                        Debug.Log("Do Nothing");
                    }
                }
                else
                {
                    Debug.Log("Do Nothing");
                }

            }

        }
        else
        {
            Debug.Log("Do Nothing");
        }
    }


 

    
        
    
        


    void SetupFloor()
    {
        if (VuforiaRuntimeUtilities.IsPlayMode())
        {
            this.floorName = "Emulator Ground Plane";
        }
        else
        {
            this.floorName = "Floor";
            GameObject floor = new GameObject(this.floorName, typeof(BoxCollider));
            floor.transform.SetParent(this.Gobj.transform.parent);
            floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            floor.transform.localScale = Vector3.one;
            floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
        }
    }
}
