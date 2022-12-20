using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
