using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WallPredictor : MonoBehaviour
{
    public float height = 1f;
    public float speed = 1f;

    private Vector3 basePos;
    private Collider2D col;

    void Start()
    {
        basePos = transform.position;
        col = GetComponent<Collider2D>();
    }

    // Predicts wall's Y position at a future time
    public float PredictY(float timeOffset)
    {
        return basePos.y + Mathf.Sin((Time.time + timeOffset) * speed) * height;
    }

    // Exposes half the wall's height based on collider bounds
    public float HalfHeight => col != null ? col.bounds.extents.y : 0.5f;
}
