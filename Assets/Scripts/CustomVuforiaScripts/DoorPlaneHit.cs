using UnityEngine;
using UnityEngine.Video;
public class DoorPlaneHit : MonoBehaviour
{
    public VideoPlayer TVPlayer;

    private void OnTriggerEnter(Collider other)
    {

        if(other.name == "collider")
        {
            TVPlayer.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.name == "collider")
        {
            TVPlayer.Pause();
        }
    }

}
