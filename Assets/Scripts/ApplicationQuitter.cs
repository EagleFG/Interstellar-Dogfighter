using UnityEngine;

public class ApplicationQuitter : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}
