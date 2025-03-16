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
        text.fontStyle = FontStyles.Bold; // ���� ó��
        text.outlineWidth = 0.2f; // �ܰ��� �β�
        text.material.EnableKeyword("_GLOW_ON");
        text.material.SetFloat("_GlowPower", 0.8f); // �� ȿ�� ���� ����

        if (itemType == ItemType.B)
        {
            text.text = "B";
            text.color = Color.green; // ���ڻ� ���
            text.outlineColor = Color.white; // �ܰ��� ���
        }
        else if (itemType == ItemType.P)
        {
            text.text = "P";
            text.color = Color.red; // ���ڻ� ������
            text.outlineColor = Color.white; // �ܰ��� ���
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
                        player.AddBomb(); // ��ź ���� ����
                        break;
                    case ItemType.P:
                        player.IncreasePower(); // �Ŀ� ��
                        break;
                }
                Destroy(gameObject); // ������ ����
            }
        }
    }
}
