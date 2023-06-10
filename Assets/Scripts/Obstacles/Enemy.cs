using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public WeaponData weaponData;
    [SerializeField] private Material redOverlayMaterial;

    [SerializeField] private float enemyHealth = 5; // Add this line to define the enemy's health
    [SerializeField] private GameObject deathEffect;

    private float pulseDuration = 0.15f;
    private Renderer characterRenderer;
    private Material[] originalMaterials;

    void Start()
    {
        characterRenderer = transform.GetChild(0).GetComponent<Renderer>();
        originalMaterials = characterRenderer.materials;
        if (weaponData) StartCoroutine(SpawnProjectile());
    }
    public IEnumerator ApplyRedPulse() {
        Material[] redMaterials = new Material[originalMaterials.Length];
        for (int i = 0; i < redMaterials.Length; i++)
        {
            redMaterials[i] = redOverlayMaterial;
        }
        characterRenderer.materials = redMaterials;
        
        yield return new WaitForSeconds(pulseDuration);
        characterRenderer.materials = originalMaterials;
    }

    public void HandleProjectileHit(Projectile projectile, PlayerController playerController) {
        StartCoroutine(ApplyRedPulse());

        TakeDamage(projectile.weaponData);
    }

    public Vector3 SnapXToNearest5(Vector3 vector)
    {
        vector.x = Mathf.Round(vector.x / 5) * 5;
        return vector;
    }
    IEnumerator SpawnProjectile()
    {
        while (true)
        {

            GameObject projectilePrefab = weaponData.projectileModel;
            Quaternion rotation = Quaternion.Euler(weaponData.projectileRotation);
            Vector3 position = SnapXToNearest5(transform.position + weaponData.projectilePosition);
            GameObject spawnedProjectile = Instantiate(projectilePrefab, position, rotation);
            EnemyProjectile enemyProjectileScript = spawnedProjectile.AddComponent<EnemyProjectile>();
            enemyProjectileScript.Initialize(weaponData);

            // spawnedProjectile.transform.parent = transform;
            yield return new WaitForSeconds(weaponData.delayBetweenShots);

        }
    }

    void OnTriggerEnter(Collider otherCollider) {
        PlayerManager.Instance.DamagePlayerIfHit(otherCollider);
    }

    // Add this method to handle taking damage
    public void TakeDamage(WeaponData weaponData)
    {
        enemyHealth -= weaponData.damage * weaponData.multiplierToMobs;
        CoinManager.Instance.AddCoins((int) weaponData.coinsPerHit);

        AudioManager.Instance.PlaySound("Mob Hit");

        if (enemyHealth <= 0)
        {
            Instantiate(deathEffect, transform.position + new Vector3(0f, 4f, 0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
