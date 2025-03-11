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

    private Rigidbody rb; // Rigidbody 컴포넌트
    private Vector2 moveInput; // 플레이어 입력 값 저장
    private float targetRotationZ; // 목표 회전 각도

    private InputAction moveAction; // 이동 입력 액션

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기

        // 입력 시스템 초기화
        var playerMap = inputActions.FindActionMap("Player"); // "Player" 액션 맵 가져오기
        moveAction = playerMap.FindAction("Move"); // "Move" 액션 가져오기

        // 이동 입력 값 업데이트 설정
        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // 입력이 감지되면 값 저장
        moveAction.canceled += _ => moveInput = Vector2.zero; // 입력이 해제되면 (키를 떼면) 0으로 초기화
    }

    private void OnEnable() => moveAction.Enable(); // 스크립트 활성화 시 입력 활성화
    private void OnDisable() => moveAction.Disable(); // 스크립트 비활성화 시 입력 비활성화

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
}
