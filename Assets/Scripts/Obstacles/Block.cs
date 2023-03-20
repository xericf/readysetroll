using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    // Block properties
    [SerializeField] private float maxHealth = 7;
    private float currentHealth = 0;

    // Gem Emission
    public GameObject gemPrefab;
    private int gemCount = 2;
    private float gemForce = 5f;

    // Pulsation
    private float pulsateScale = 1.1f;
    private float pulsateSpeed = 0.05f;
    private bool pulsating = false;

    // Block breaking
    [SerializeField] private GameObject effect;
    [SerializeField] List<GameObject> stageModels = new List<GameObject>();
    private GameObject breakingStageBlock = null;
    private int currentStage = -1; // stage of -1 means that it is not using any stageModels game objet right now


    void Start() {
        currentHealth = maxHealth;
    }

    public void HandleProjectileHit(Projectile projectileScript) {

        WeaponData weaponData = projectileScript.GetWeaponData();

        TakeDamageFromProjectile(weaponData);
        AdvanceBreakStageIfApplicable();

        StartCoroutine(Pulsate());
        EmitGems();

    }

    private void TakeDamageFromProjectile(WeaponData weaponData) {
        currentHealth -= weaponData.damage;

        if (currentHealth <= 0) {

            Instantiate(effect, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }
    }

    private void AdvanceBreakStageIfApplicable() {
        float modelCount = stageModels.Count;
        int nextStage = (int) (modelCount - (modelCount * (currentHealth / maxHealth)));
        nextStage = nextStage >= modelCount ? (int) modelCount - 1 : nextStage;
        
        if (nextStage != currentStage) {
            if (breakingStageBlock != null) Destroy(breakingStageBlock);
            
            GameObject breakingModel = Instantiate(stageModels[nextStage]);

            breakingModel.transform.parent = gameObject.transform;
            breakingModel.transform.localPosition = Vector3.zero;
            breakingModel.transform.localEulerAngles = Vector3.zero;

            breakingStageBlock = breakingModel;
        }
    }

    IEnumerator Pulsate()
    {
        if (pulsating) yield break;
        pulsating = true;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * pulsateScale;
        float t = 0;

        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            t += Time.deltaTime / pulsateSpeed;
            yield return null;
        }

        t = 0;

        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            t += Time.deltaTime / pulsateSpeed;
            yield return null;
        }
        transform.localScale = originalScale;
        pulsating = false;

    }

    void EmitGems()
    {
        for (int i = 0; i < gemCount; i++)
        {
            GameObject gem = Instantiate(gemPrefab, transform.position, Quaternion.identity);
            Rigidbody gemRb = gem.GetComponent<Rigidbody>();
            Vector3 forceDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
            gemRb.AddForce(forceDirection * gemForce, ForceMode.Impulse);
        }
    }

}
