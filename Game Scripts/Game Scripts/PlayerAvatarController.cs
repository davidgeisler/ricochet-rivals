using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAvatarController : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;

    public GameObject shootIndicator;
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float shootInterval = 0.2f;
    private float shootTimer;
    private bool canShoot = true;

    public TextMeshProUGUI shotsFiredText; 
    private int shotsFired = 0;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Track and aim toward mouse
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Cooldown logic for shooting
        if (!canShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                canShoot = true;
                shootIndicator.SetActive(true);
                shootTimer = 0f;
            }
        }

        // Right click to shoot
        if (Input.GetMouseButtonUp(1) && canShoot)
        {
            canShoot = false;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetDirection(direction);
            bullet.GetComponent<PlayerProjectile>().ownerTag = "Player"; // prevent self-damage

            shotsFired++;
            if (shotsFiredText != null)
            {
                shotsFiredText.text = shotsFired.ToString();
            }
            shootIndicator.SetActive(false);
        }
    }
}
