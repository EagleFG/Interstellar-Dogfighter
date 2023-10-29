using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void SetWaveAndLoadGame(int startingWave)
    {
        GameManager.startingWave = startingWave;

        SceneManager.LoadScene(1);
    }
}
