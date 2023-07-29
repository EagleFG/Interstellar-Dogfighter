using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int _currentWave = 0;

    private bool _isGameOver = false;

    private bool _isVictory = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && (_isGameOver == true || _isVictory == true))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.M) && (_isGameOver == true || _isVictory == true))
        {
            SceneManager.LoadScene(0);
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

    public bool GetVictoryState()
    {
        return _isVictory;
    }

    public void SetVictoryState(bool newVictoryState)
    {
        _isVictory = newVictoryState;
    }
}
