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
    private Color originalColor; // ���� ���� ����

    private void Awake()
    {
        renderersO = GetComponent<Renderer>();
        originalColor = renderersO.material.color; // �ʱ� ���� ����
        TryFindPlayer(); // �÷��̾ �����Ǿ����� �ٽ� ã��
    }

    private void Update()
    {
        TryFindPlayer(); // �÷��̾ �����Ǿ����� �ٽ� ã��
        Fire();
        Reload();
    }

    private void Fire()
    {
        if (curShotDelay < maxShotDelay || player == null) return; // �÷��̾ ������ �߻� �ߴ�

        // �÷��̾� ���� ���
        Vector3 targetDir = (player.position - transform.position).normalized;

        // �Ѿ� ���� �� ���� ����
        GameObject bullet = Instantiate(eBullet, transform.position, Quaternion.LookRotation(targetDir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // �Ѿ��� �÷��̾� �������� �߻�
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
        renderersO.material.color = Color.red; // �ǰ� �� ���������� ����

        // ���� �ð� �� ���� �������� ����
        Invoke(nameof(ReturnMat), 0.1f);

        if (health <= 0)
        {
            PlayerMove playerLogic = player.GetComponent<PlayerMove>();
            playerLogic.score += eScore;

            TryDropItem(); // ������ ��� �õ�
            Destroy(gameObject);
        }
    }
    private void TryDropItem()
    {
        // Ȯ�� üũ (10%)
        if (Random.Range(0f, 1f) < dropChance && itemPrefabs.Length > 0)
        {
            // ���� ������ ����
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }

    void ReturnMat()
    {
        renderersO.material.color = originalColor; // ���� �������� ����
    }
    private void TryFindPlayer()
    {
        // ���� �÷��̾ �����Ǿ��ų�, ��Ȱ��ȭ�� ��� �ٽ� ã��
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
