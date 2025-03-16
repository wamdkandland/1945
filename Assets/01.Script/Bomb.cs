using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public enum ItemType { B, P }
    public ItemType itemType;

    public TextMeshPro text;

    private void Start()
    {
        ApplyStyle();
    }
    private void Update()
    {
        transform.Rotate(0, 0, 60 * Time.deltaTime);
    }

    private void ApplyStyle()
    {
        text.fontStyle = FontStyles.Bold; // 볼드 처리
        text.outlineWidth = 0.2f; // 외곽선 두께
        text.material.EnableKeyword("_GLOW_ON");
        text.material.SetFloat("_GlowPower", 0.8f); // 빛 효과 강도 조절

        if (itemType == ItemType.B)
        {
            text.text = "B";
            text.color = Color.green; // 글자색 녹색
            text.outlineColor = Color.white; // 외곽선 흰색
        }
        else if (itemType == ItemType.P)
        {
            text.text = "P";
            text.color = Color.red; // 글자색 빨간색
            text.outlineColor = Color.white; // 외곽선 흰색
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();
            if (player != null)
            {
                switch (itemType)
                {
                    case ItemType.B:
                        player.AddBomb(); // 폭탄 개수 증가
                        break;
                    case ItemType.P:
                        player.IncreasePower(); // 파워 업
                        break;
                }
                Destroy(gameObject); // 아이템 제거
            }
        }
    }
}
