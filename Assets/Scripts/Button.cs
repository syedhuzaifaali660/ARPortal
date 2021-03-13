using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    //public GameObject Portal_1B, Portal_2B, Portal_3B;

    [SerializeField] private AudioSource ButtonTap;

    public void PortalFunction()
    {
        StartCoroutine(Wait());
        SceneManager.LoadScene("Portal");

    }

    public void PortalFunction2()
    {
        StartCoroutine(Wait());
        SceneManager.LoadScene("Portal2-World");
    }

    public void PortalFunction3()
    {
        StartCoroutine(Wait());
        SceneManager.LoadScene("Portal3");
    }

    IEnumerator Wait()
    {
        ButtonTap.Play();
        yield return new WaitForSeconds(0.01f);
    }


   
}
