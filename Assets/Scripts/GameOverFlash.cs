using System.Collections;
using UnityEngine;
using TMPro;

public class GameOverFlash : MonoBehaviour
{
    [SerializeField]
    private float _flashInterval = .5f;

    void OnEnable()
    {
        StartCoroutine(ContinuouslyFlashOnAndOff());
    }

    IEnumerator ContinuouslyFlashOnAndOff()
    {
        if (gameObject.TryGetComponent(out TextMeshProUGUI textComponent))
        {
            while (true)
            {
                textComponent.enabled = true;

                yield return new WaitForSeconds(_flashInterval);

                textComponent.enabled = false;

                yield return new WaitForSeconds(_flashInterval);
            }
        }
    }
}
