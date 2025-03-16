using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemysObjs;
    public Transform[] spawnPoints;
    public Transform[] endPoints; // ���� ����Ʈ �迭 �߰� (�ν����Ϳ��� 5�� �Ҵ�)
    public float curSpawnDelay;
    public float maxSpawnDelay;

    public GameObject player;
    public TextMeshProUGUI scoretext;    
    public GameObject gameOverset;
    public Image[] lifeImage;
    

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }

        scoreuiUpdate();
    }

    void scoreuiUpdate()
    {
        PlayerMove playerLogic = player.GetComponent<PlayerMove>();
        scoretext.text = string.Format("{0:n0}", playerLogic.score);
    }

    public void updateLife(int life)
    {
        // ��� ������ �̹����� ���� ����
        for (int index = 0; index < lifeImage.Length; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        // ���� ������ ����ŭ �̹��� ǥ��
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    private void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, enemysObjs.Length);
        int ranSpawnPoint = Random.Range(0, spawnPoints.Length);
        int ranEndPoint = Random.Range(0, endPoints.Length); // ���� ���� ����Ʈ ����

        Transform spawnPoint = spawnPoints[ranSpawnPoint];
        Transform endPoint = endPoints[ranEndPoint];

        GameObject enemy = Instantiate(enemysObjs[ranEnemy],
                            spawnPoint.position,
                            spawnPoint.rotation);

        // �̵� ���� ���
        Vector3 moveDirection = (endPoint.position - spawnPoint.position).normalized;

        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        // ���� �� �ӵ��� �̵� ����
        rb.linearVelocity = moveDirection * enemyLogic.speed;

        // (�ɼ�) ���� ���� ����Ʈ�� �ٶ󺸵��� ȸ��
        enemy.transform.LookAt(endPoint);
    }

    public void RewpawnPlayer()
    {
        Invoke("RewpawnPlayerexe", 2f);        
    }

    public void RewpawnPlayerexe()
    {
        player.transform.position = new Vector3(0, 0, -20);
        player.SetActive(true);
        PlayerMove playerLogic = player.GetComponent<PlayerMove>();
        playerLogic.hit = false;
    }

    public void gameOver()
    {
        player.gameObject.SetActive(false);
        gameOverset.SetActive(true);
        Time.timeScale = 0f;
    }

    public void restartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}