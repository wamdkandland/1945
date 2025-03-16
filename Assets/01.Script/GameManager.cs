using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemysObjs;
    public Transform[] spawnPoints;
    public Transform[] endPoints; // 엔드 포인트 배열 추가 (인스펙터에서 5개 할당)
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
        // 모든 라이프 이미지를 먼저 숨김
        for (int index = 0; index < lifeImage.Length; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        // 현재 라이프 수만큼 이미지 표시
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    private void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, enemysObjs.Length);
        int ranSpawnPoint = Random.Range(0, spawnPoints.Length);
        int ranEndPoint = Random.Range(0, endPoints.Length); // 랜덤 엔드 포인트 선택

        Transform spawnPoint = spawnPoints[ranSpawnPoint];
        Transform endPoint = endPoints[ranEndPoint];

        GameObject enemy = Instantiate(enemysObjs[ranEnemy],
                            spawnPoint.position,
                            spawnPoint.rotation);

        // 이동 방향 계산
        Vector3 moveDirection = (endPoint.position - spawnPoint.position).normalized;

        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        // 방향 × 속도로 이동 적용
        rb.linearVelocity = moveDirection * enemyLogic.speed;

        // (옵션) 적이 엔드 포인트를 바라보도록 회전
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