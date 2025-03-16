using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 25f; // 이동 속도
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도
    [SerializeField] private float maxRotationZ = 30f; // 최대 Z축 회전 각도

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions; // 입력 액션 에셋

    [Header("Combat")]
    public GameObject bulletObj;
    public float power;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private Transform firePoint; // 발사 위치 (비행기 앞쪽에 빈 오브젝트 생성 권장)
    [SerializeField] private float fireRate = 0.2f; // 초당 발사 횟수
    private float nextFireTime;

    [Header("Bomb Settings")]
    //[SerializeField] private GameObject bombEffect; // 폭발 이펙트 프리팹
    [SerializeField] private int bombCount = 2; // 초기 폭탄 개수
    [SerializeField] private float bombCooldown = 5f; // 재사용 대기시간
    private float nextBombTime;
    public TextMeshProUGUI bombCountText; // 폭탄 UI 텍스트

    private Rigidbody rb; // Rigidbody 컴포넌트
    private Vector2 moveInput; // 플레이어 입력 값 저장
    private float targetRotationZ; // 목표 회전 각도

    private InputAction moveAction; // 이동 입력 액션
    private InputAction fireAction;// 발사 입력 액션
    private InputAction bombAction;// 발사 입력 액션

    public GameManager manager;
    public int life;
    public int score;
    public bool hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기

        // 입력 시스템 초기화
        var playerMap = inputActions.FindActionMap("Player"); // "Player" 액션 맵 가져오기
        moveAction = playerMap.FindAction("Move"); // "Move" 액션 가져오기

        // 이동 입력 값 업데이트 설정
        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // 입력이 감지되면 값 저장
        moveAction.canceled += _ => moveInput = Vector2.zero; // 입력이 해제되면 (키를 떼면) 0으로 초기화
                                                              // 
        fireAction = playerMap.FindAction("Fire");
        fireAction.performed += _ => Fire(); // 버튼 눌렸을 때 Fire() 실행
        power = 1;

        bombAction = playerMap.FindAction("Bomb");
        bombAction.performed += _ => Bomb();
    }
    private void Start()
    {
        UpdateBombUI();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        fireAction.Enable(); // Fire 액션 활성화
    }

    private void OnDisable()
    {
        moveAction.Disable();
        fireAction.Disable(); // Fire 액션 비활성화
    }

    private void FixedUpdate()
    {
        HandleMovement(); // 이동 처리
        HandleRotation(); // 회전 처리
    }

    private void HandleMovement()
    {
        // X축(좌우) + Z축(앞뒤) 이동 벡터 계산
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        rb.linearVelocity = movement; // Rigidbody 속도 설정
    }

    private void HandleRotation()
    {
        // 이동 방향에 따라 목표 회전 각도 설정 (왼쪽 입력 시 +각도, 오른쪽 입력 시 -각도)
        targetRotationZ = -moveInput.x * maxRotationZ;

        // 현재 Z축 회전 각도를 -180 ~ 180 범위로 변환
        float currentZ = NormalizeAngle(rb.rotation.eulerAngles.z);

        // 부드러운 회전 적용 (Lerp를 사용하여 점진적으로 회전)
        float newZ = Mathf.Lerp(
            currentZ,
            targetRotationZ,
            rotationSpeed * Time.fixedDeltaTime
        );

        // Rigidbody의 회전 값 업데이트
        rb.MoveRotation(Quaternion.Euler(0, 0, newZ));
    }

    // 각도 정규화 (-180 ~ 180 범위로 변환)
    private float NormalizeAngle(float angle)
    {
        angle %= 360; // 360도를 기준으로 나눈 나머지 값 사용
        return angle > 180 ? angle - 360 : angle; // 180도를 초과하면 -360을 해서 -180 ~ 180 범위로 변환
    }        

    void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        // 파워 단계별 발사 위치 오프셋 정의
        Vector3[] offsets = power switch
        {
            1 => new Vector3[] { Vector3.zero },
            2 => new Vector3[] { Vector3.right , Vector3.left  },
            3 => new Vector3[] { Vector3.right * 1.5f, Vector3.zero, Vector3.left * 1.5f },
            _ => Array.Empty<Vector3>()
        };

        // 모든 오프셋 위치에 총알 발사
        foreach (var offset in offsets)
        {
            FireBullet(firePoint.position + offset, Quaternion.Euler(90f, -90f, 0));
        }
    }
    void Bomb()
    {
        // 쿨타임 및 폭탄 개수 체크
        if (Time.time < nextBombTime || bombCount <= 0) return;

        // 모든 적 & 적 총알 제거
        DestroyAllObjects("Enemy");
        DestroyAllObjects("EBullet");

        // 폭탄 효과 생성
        //if (bombEffect != null)
        //    Instantiate(bombEffect, transform.position, Quaternion.identity);

        // 폭탄 사용 처리
        bombCount--;
        nextBombTime = Time.time + bombCooldown;
        UpdateBombUI();
    }

    void DestroyAllObjects(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    void UpdateBombUI()
    {
        if (bombCountText != null)
            bombCountText.text = bombCount.ToString(); // 숫자만 표시
    }

    private void FireBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletObj, position, rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag =="Enemy" ||  other.gameObject.tag == "EBullet")
        {
            if (hit)return;
            
            hit = true;
            life--;
            manager.updateLife(life);
            if (life <= 0)
            {
                manager.gameOver();
            }
            else
            {
                manager.RewpawnPlayer();
                gameObject.SetActive(false);
            }
        }
    }

    // 폭탄 추가 메서드
    public void AddBomb()
    {
        bombCount++;
        UpdateBombUI(); // UI 업데이트
    }

    // 파워 업 메서드
    public void IncreasePower()
    {
        power = Mathf.Min(power + 1, 3); // 최대 3으로 제한
                                         // 파워 업 효과음 또는 이펙트 추가 가능
    }
}

