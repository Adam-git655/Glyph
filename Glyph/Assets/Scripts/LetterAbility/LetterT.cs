using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LetterT")]
public class LetterT : Ability
{
    [SerializeField] private GameObject tPrefab;
    [SerializeField] private float spawnOffset = 1f; 
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float spinSpeed = 720f;

    public override void Use(Vector2 characterPosition, Vector2 mousePosition)
    {
        Vector2 direction = (mousePosition - characterPosition).normalized;
        Vector2 spawnPosition = characterPosition + direction * spawnOffset;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject projectile = Instantiate(tPrefab, spawnPosition, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
            rb.angularVelocity = (direction.x >= 0f) ? -spinSpeed : spinSpeed;
            rb.angularDamping = 0.5f;
        }
    }
}
