using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int _currentWave = 0;

    private bool _isGameOver = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void IncreaseCurrentWaveNumber()
    {
        _currentWave++;
    }

    public int GetCurrentWaveNumber()
    {
        return _currentWave;
    }

    public bool GetGameOverState()
    {
        return _isGameOver;
    }

    public void SetGameOverState(bool newGameOverState)
    {
        _isGameOver = newGameOverState;
    }
}
