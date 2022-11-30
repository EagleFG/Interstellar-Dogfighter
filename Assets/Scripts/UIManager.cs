using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreUI;

    [SerializeField]
    private Image _livesUI;

    [SerializeField]
    private Sprite[] _livesUISprites;

    [SerializeField]
    private GameObject _gameOverText;

    private void Start()
    {
        _scoreUI.text = "Score: 0";
    }

    public void UpdateScoreText(int score)
    {
        _scoreUI.text = "Score: " + score;
    }

    public void UpdateLives(int lives)
    {
        _livesUI.sprite = _livesUISprites[lives];
    }

    public void EnableGameOverText()
    {
        _gameOverText.SetActive(true);
    }
}
