using UnityEngine;

public class RandomBounceMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public LayerMask wallLayer;

    private Rigidbody rb;
    private Vector3 currentDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitializeDiagonalDirection();
    }

    void InitializeDiagonalDirection()
    {
        // 4방향 대각선 배열 생성
        Vector3[] directions = {
            new Vector3(1, 0, 1).normalized,   // ↗
            new Vector3(1, 0, -1).normalized,  // ↘
            new Vector3(-1, 0, 1).normalized,  // ↖
            new Vector3(-1, 0, -1).normalized  // ↙
        };

        // 랜덤 방향 선택
        int randomIndex = Random.Range(0, directions.Length);
        currentDirection = directions[randomIndex];

        rb.linearVelocity = currentDirection * moveSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            currentDirection = Vector3.Reflect(currentDirection, normal).normalized;
            rb.linearVelocity = currentDirection * moveSpeed;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = currentDirection * moveSpeed;
    }
}