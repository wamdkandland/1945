using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float bulletSpeed;
    public int health;
    public GameObject eBullet;
    public float maxShotDelay;
    public float curShotDelay;
    public int eScore;
    private Transform player;

    public GameObject[] itemPrefabs;
    [Range(0, 1)][SerializeField] private float dropChance = 0.1f;

    public Renderer renderersO;
    private Color originalColor; // 원래 색상 저장

    private void Awake()
    {
        renderersO = GetComponent<Renderer>();
        originalColor = renderersO.material.color; // 초기 색상 저장
        TryFindPlayer(); // 플레이어가 삭제되었으면 다시 찾기
    }

    private void Update()
    {
        TryFindPlayer(); // 플레이어가 삭제되었으면 다시 찾기
        Fire();
        Reload();
    }

    private void Fire()
    {
        if (curShotDelay < maxShotDelay || player == null) return; // 플레이어가 없으면 발사 중단

        // 플레이어 방향 계산
        Vector3 targetDir = (player.position - transform.position).normalized;

        // 총알 생성 및 방향 설정
        GameObject bullet = Instantiate(eBullet, transform.position, Quaternion.LookRotation(targetDir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // 총알을 플레이어 방향으로 발사
        rb.linearVelocity = targetDir * bulletSpeed;

        curShotDelay = 0;
    }

    private void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void onHit(int damage)
    {
        health -= damage;
        renderersO.material.color = Color.red; // 피격 시 빨간색으로 변경

        // 일정 시간 후 원래 색상으로 복원
        Invoke(nameof(ReturnMat), 0.1f);

        if (health <= 0)
        {
            PlayerMove playerLogic = player.GetComponent<PlayerMove>();
            playerLogic.score += eScore;

            TryDropItem(); // 아이템 드롭 시도
            Destroy(gameObject);
        }
    }
    private void TryDropItem()
    {
        // 확률 체크 (10%)
        if (Random.Range(0f, 1f) < dropChance && itemPrefabs.Length > 0)
        {
            // 랜덤 아이템 선택
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }

    void ReturnMat()
    {
        renderersO.material.color = originalColor; // 원래 색상으로 복원
    }
    private void TryFindPlayer()
    {
        // 기존 플레이어가 삭제되었거나, 비활성화된 경우 다시 찾기
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            var playerObj = FindAnyObjectByType<PlayerMove>();
            player = playerObj != null ? playerObj.transform : null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EWall")
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "PBullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            onHit(bullet.damage);
            Destroy(other.gameObject);
        }
    }    
}
