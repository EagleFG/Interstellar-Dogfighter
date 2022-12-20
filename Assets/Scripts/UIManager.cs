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
    private GameObject[] _shieldUISprites;

    [SerializeField]
    private Slider _fuelSlider;

    [SerializeField]
    private TextMeshProUGUI _ammoCountUI;

    [SerializeField]
    private GameObject _gameOverText;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void UpdateScoreUI(int score)
    {
        _scoreUI.text = "Score: " + score;
    }

    public void UpdateLivesUI(int lives)
    {
        _livesUI.sprite = _livesUISprites[lives];
    }

    public void UpdateShieldsUI(int shieldHealth)
    {
        for (int i = 0, l = _shieldUISprites.Length; i < l; i++)
        {
            if (i < shieldHealth)
            {
                _shieldUISprites[i].SetActive(true);
            }
            else
            {
                _shieldUISprites[i].SetActive(false);
            }
        }
    }

    public void UpdateFuelSlider(float fuelRemaining)
    {
        _fuelSlider.value = fuelRemaining;
    }

    public void UpdateAmmoCountUI(int ammoCount)
    {
        _ammoCountUI.text = "Ammo: " + ammoCount.ToString();
    }

    public void EnableGameOverUI()
    {
        _gameOverText.SetActive(true);
    }
}
