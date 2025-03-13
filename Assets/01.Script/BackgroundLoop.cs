using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    private float moveDistance;  // 🔹 리포지션 거리

    private void Awake()
    {
        BoxCollider backgroundCollider = GetComponent<BoxCollider>();

        if (backgroundCollider == null)
        {
            Debug.LogError("BackgroundLoop: BoxCollider가 없습니다! Collider를 추가하세요.");
            return;
        }

        moveDistance = backgroundCollider.size.y * transform.lossyScale.y;  // 🔥 Y축 크기 사용
    }

    void Update()
    {
        if (transform.position.z <= -moveDistance*0.7f)  // 🔹 Z축 값 체크
        {
            Reposition();
        }
    }

    private void Reposition()
    {
        Vector3 offset = new Vector3(0, 0, moveDistance*2);  // 🔥 충분히 뒤로 이동하도록 수정
        transform.position += offset;
    }
}