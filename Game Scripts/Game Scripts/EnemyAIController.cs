using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyAIController : MonoBehaviour
{
    /* ------------- PUBLIC CONFIG ---------------- */
    public enum AILevel { Easy, Medium, Hard }
    public AILevel difficulty = AILevel.Easy;

    public GameObject bulletPrefab;
    public GameObject shootIndicator;
    public Transform firePoint;

    public float shootInterval = 2f;
    public float bulletSpeed = 10f;   //  ->  needed e2 for time-of-flight estimation

    public TextMeshProUGUI aiDifficultyText;
    public TextMeshProUGUI aiShotsText;

    /* ------------- PRIVATE STATE ---------------- */
    private float shootTimer;
    private Transform playerTarget;
    private int aiShotsFired = 0;

    private const float wallTolerance = 0.1f; // used to improve wall hit prediction

    /* ------------- UNITY LIFECYCLE -------------- */
    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;

        if (aiDifficultyText != null)
            aiDifficultyText.text = "AI-" + difficulty.ToString().ToUpper();
    }

    void Update()
    {
        if (playerTarget == null) return;

        // Handle shoot indicator status
        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f && shootIndicator != null)
                shootIndicator.SetActive(true); // Ready to shoot
        }

        switch (difficulty)
        {
            case AILevel.Easy:
                EasyBehavior();         // rule-based lang. aim straight as player and mindless shooting
                break;

            case AILevel.Medium:
                MediumBehavior();      // Light Montecarlo Search Tree
                                       // 1. Simulate multiple random angles toward the player
                                       // 2. Predict where the shot might go (raycast-style logic)
                                       // 3. Choose the best angle that seems to hit the player while avoiding walls (rough simulation)
                                       // 4. Shoot based on that angle
                break;

            case AILevel.Hard:
                HardBehavior();        // FULL MCTS withh spatial awareness and time calculation
                break;
        }
    }

    /* ------------- EASY ------------- */
    void EasyBehavior()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            // MONTE CARLO SIMULATIION START
            // Simulate 3-5 random shooting trajectory
            Vector2 bestDirection = Vector2.zero;
            float bestScore = float.MinValue;

            // Do 5 times
            for (int i = 0; i < 5; i++)
            {
                // Generate small random angles
                float angleOffset = Random.Range(-45f, 45f);
                Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * transform.right;

                // Simulate the shots in each angles
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 20f);
                float score = 0f;

                // Picking the best shot
                Color rayColor = Color.yellow;

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    score += 100f;
                    rayColor = Color.green;
                }
                else if (hit.collider == null)
                {
                    score += 10f;
                    rayColor = Color.red;
                }

                // 🎯 Show ray in Scene View
                Debug.DrawRay(transform.position, direction * 20f, rayColor, 0.5f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestDirection = direction;
                }
            }

            // Rotate AI avatar to face bestDirection
            float angle = Mathf.Atan2(bestDirection.y, bestDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            FireBullet(bestDirection.normalized);
            shootTimer = shootInterval;
        }
    }


    /* ------------- MEDIUM ----------- */
    void MediumBehavior()
    {
        if (shootTimer > 0f) return;

        Debug.Log("Medium AI calculating…");

        int bestScore = int.MinValue;
        float bestAngle = 0f;

        Vector2 toPlayer = playerTarget.position - firePoint.position;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        // Monte Carlo: Simulate 21 directions from -20° to +20° around the base angle
        for (int i = -10; i <= 10; i++)
        {
            float angle = baseAngle + i * 2f;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            int score = 0;
            int mask = LayerMask.GetMask("Default", "Player", "WallBoundary");

            // Cast from just outside own collider so we never hit ourselves
            Vector2 safeOrigin = (Vector2)firePoint.position + dir * 0.1f;
            RaycastHit2D[] hits = Physics2D.RaycastAll(safeOrigin, dir, 50f, mask);

            Color rayColor = Color.gray;

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                if (hit.collider.CompareTag("Player"))
                {
                    score += 50;
                    rayColor = Color.green;
                }
                else if (hit.collider.CompareTag("WallBoundary") || hit.collider.name.Contains("Wall"))
                {
                    score -= 20;
                    rayColor = Color.red;
                }
                else
                {
                    score += 10;
                    rayColor = Color.yellow;
                }

                break; // only first valid hit matters
            }

            // 🎯 Show ray in Scene View
            Debug.DrawRay(safeOrigin, dir * 50f, rayColor, 0.5f);

            if (score > bestScore)
            {
                bestScore = score;
                bestAngle = angle;
            }
        }

        // Convert chosen angle to direction vector and shoot
        Vector2 shootDir = new Vector2(Mathf.Cos(bestAngle * Mathf.Deg2Rad),
                                       Mathf.Sin(bestAngle * Mathf.Deg2Rad));
        FireBullet(shootDir);
    }


    /* ------------- HARD ------ */
    // Main behavior for Hard AI using Monte Carlo + Raycasting precision
    void HardBehavior()
    {
        if (shootTimer > 0f) return;

        Debug.Log("Hard AI calculating...");

        int bestScore = int.MinValue;
        float bestAngle = 0f;

        // Calculate angle toward player
        Vector2 toPlayer = playerTarget.position - firePoint.position;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        // Simulate 91 possible angles around player direction (±45°)
        for (int i = -45; i <= 45; i++)
        {
            float angle = baseAngle + i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Run recursive bounce-aware simulation
            int score = SimulateCombatBounce_TIME(firePoint.position, dir, 3, 0f);

            if (score > bestScore)
            {
                bestScore = score;
                bestAngle = angle;
            }
        }

        Vector2 bestDir = new Vector2(Mathf.Cos(bestAngle * Mathf.Deg2Rad), Mathf.Sin(bestAngle * Mathf.Deg2Rad));

        Debug.Log($"Hard AI: Shooting with score {bestScore} at angle {bestAngle}");

        // Fire best evaluated shot
        FireBullet(bestDir);
    }



    /* ---------- TIME-AWARE BOUNCE SIMULATION ---- */
    // Recursively simulates the outcome of a bounce shot
    int SimulateCombatBounce_TIME(Vector2 origin, Vector2 direction, int remainingBounces, float elapsedT)
    {
        if (remainingBounces < 0) return 0;

        int mask = LayerMask.GetMask("Default", "Player", "WallBoundary");
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 50f, mask);

        if (hit.collider == null) return 0;

        Debug.DrawRay(origin, direction * hit.distance, Color.green, 0.2f);

        float travelT = hit.distance / bulletSpeed;
        float arrivalT = elapsedT + travelT;

        string tag = hit.collider.tag;
        string name = hit.collider.name;

        if (tag == "Player")
        {
            int timeScore = Mathf.Max(0, 100 - Mathf.RoundToInt(arrivalT * 10));
            return 200 + timeScore;
        }

        // Handle bouncing off walls
        if (tag == "WallBoundary" || name.Contains("Wall"))
        {
            WallPredictor wall = hit.collider.GetComponent<WallPredictor>();
            if (wall != null)
            {
                float predictedY = wall.PredictY(arrivalT);
                float halfHeight = wall.HalfHeight;

                // Skip unrealistic bounce if outside predicted zone
                if (Mathf.Abs(predictedY - hit.point.y) > halfHeight + wallTolerance)
                {
                    Vector2 advance = hit.point + direction * 0.1f;
                    return SimulateCombatBounce_TIME(advance, direction, remainingBounces, arrivalT);
                }
            }

            // Reflect direction and continue
            Vector2 reflected = Vector2.Reflect(direction, hit.normal);
            Vector2 nextOrigin = hit.point + reflected * 0.1f;
            return SimulateCombatBounce_TIME(nextOrigin, reflected, remainingBounces - 1, arrivalT) - 10;
        }

        return -20; // Penalize bad collisions
    }

    /* ----------------- HELPERS ----------------------------- */
    void RotateTowardsPlayer()
    {
        Vector3 dir = playerTarget.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
    }

    void FireBullet(Vector2 dir)
    {
        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 180f);

        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        PlayerProjectile pp = b.GetComponent<PlayerProjectile>();
        pp.SetDirection(dir);
        if (pp != null) pp.ownerTag = "AI";

        b.tag = "EnemyProjectile";
        b.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;

        if (shootIndicator != null) shootIndicator.SetActive(false);

        shootTimer = shootInterval;
        aiShotsFired++;
        if (aiShotsText != null) aiShotsText.text = "Shots: " + aiShotsFired;
    }

    public void RefreshDifficultyText()
    {
        if (aiDifficultyText != null)
            aiDifficultyText.text = "AI-" + difficulty.ToString().ToUpper();
    }

}
