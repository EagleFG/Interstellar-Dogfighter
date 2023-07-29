using System.Collections;
using UnityEngine;

public class BossMultiBeamAttack : MonoBehaviour
{
    [SerializeField]
    private BossEnemyBehavior _bossScript;

    [SerializeField]
    private float _turretRotationDisableDuration = 2f;

    private void OnEnable()
    {
        StartCoroutine(FireBeamsAndDisableRotation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        _bossScript.OffensiveAbilityFinished();
    }

    IEnumerator FireBeamsAndDisableRotation()
    {
        for (int i = 0, l = _bossScript.GetBossTurretArray().Length; i < l; i++)
        {
            _bossScript.GetBossTurretArray()[i].DisablePlayerTargeting();
            _bossScript.GetBossTurretArray()[i].FireBeam();
        }

        yield return new WaitForSeconds(_turretRotationDisableDuration);

        for (int i = 0, l = _bossScript.GetBossTurretArray().Length; i < l; i++)
        {
            _bossScript.GetBossTurretArray()[i].EnablePlayerTargeting();
        }

        gameObject.SetActive(false);
    }
}
