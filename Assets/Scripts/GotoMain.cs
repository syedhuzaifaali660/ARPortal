using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GotoMain : MonoBehaviour
{
    [SerializeField] private AudioSource ButtonTap;

    // Start is called before the first frame update
    public void GotoMainScene()
    {
        StartCoroutine(Wait());
        SceneManager.LoadScene("MainUI");
    }

    private void FixedUpdate()
    {

        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(Wait());
            SceneManager.LoadScene("MainUI");
        }
        else
        {
            //do nothing
        }

    }

    IEnumerator Wait()
    {
        ButtonTap.Play();
        yield return new WaitForSeconds(0.01f);
    }
}
