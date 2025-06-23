using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerProjectile : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;

    [SerializeField] private ParticleSystem collisionParticle;
    [SerializeField] private SpriteRenderer projectileSprite;
    [SerializeField] private TrailRenderer projectileTrail;
    [SerializeField] private PolygonCollider2D projectileCollider;

    [SerializeField] private AudioClip bounceSFX;  // 🔊 New: Bounce sound

    private Vector2 direction;

    public float force = 10f;
    public int maxBounces = 3;
    private int currentBounces = 0;

    public static int globalMaxBounces = 3;

    public string ownerTag; // "Player" or "AI"

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * force;

        maxBounces = globalMaxBounces;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(ownerTag))
        {
            destroyProjectile();
            return;
        }

        if (collision.gameObject.CompareTag("WallBoundary"))
        {
            UpdateRotation();
            currentBounces++;

            // 🔊 Play bounce SFX
            if (bounceSFX != null)
                AudioSource.PlayClipAtPoint(bounceSFX, transform.position);

            if (currentBounces >= maxBounces)
            {
                destroyProjectile();
            }

            return;
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            HealthController hp = collision.gameObject.GetComponent<HealthController>();
            if (hp != null) hp.TakeDamage();

            destroyProjectile();
            return;
        }

        destroyProjectile();
    }

    void destroyProjectile()
    {
        var emission = collisionParticle.emission;
        var duration = collisionParticle.main.duration;

        emission.enabled = true;
        collisionParticle.Play();

        Destroy(projectileSprite);
        Destroy(projectileTrail);
        Destroy(projectileCollider);

        Invoke(nameof(DestroyObj), duration);
    }

    void DestroyObj()
    {
        Destroy(gameObject);
    }

    void UpdateRotation()
    {
        Vector2 velocity = rb.velocity;

        if (velocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
