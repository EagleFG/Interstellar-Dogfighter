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
    private Image[] _shieldUISprites;

    [SerializeField]
    private Slider _fuelSlider;

    [SerializeField]
    private TextMeshProUGUI _ammoCountUI;

    [SerializeField]
    private TextMeshProUGUI _waveCounter;

    [SerializeField]
    private GameObject _gameOverText;

    private void Start()
    {
        _waveCounter.text = "Wave: 00";
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
                _shieldUISprites[i].gameObject.SetActive(true);
            }
            else
            {
                _shieldUISprites[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateShieldsColorUI(Color newColor)
    {
        for (int i = 0, l = _shieldUISprites.Length; i < l; i++)
        {
            _shieldUISprites[i].color = newColor;
        }
    }

    public void UpdateFuelSlider(float fuelRemaining)
    {
        _fuelSlider.value = fuelRemaining;
    }

    public void UpdateAmmoCountUI(int ammoCount, int maxAmmo)
    {
        _ammoCountUI.text = ammoCount.ToString("D2") + "/" + maxAmmo.ToString();
    }

    public void UpdateWaveCounterUI(int waveNumber)
    {
        _waveCounter.text = "Wave: " + waveNumber.ToString("D2");
    }

    public void EnableGameOverUI()
    {
        _gameOverText.SetActive(true);
    }
}
