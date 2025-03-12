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

    private void ApplyStyle()
    {
        text.fontStyle = FontStyles.Bold; // º¼µå Ã³¸®
        text.outlineWidth = 0.2f; // ¿Ü°û¼± µÎ²²
        text.material.EnableKeyword("_GLOW_ON");
        text.material.SetFloat("_GlowPower", 0.8f); // ºû È¿°ú °­µµ Á¶Àý

        if (itemType == ItemType.B)
        {
            text.text = "B";
            text.color = Color.green; // ±ÛÀÚ»ö ³ì»ö
            text.outlineColor = Color.white; // ¿Ü°û¼± Èò»ö
        }
        else if (itemType == ItemType.P)
        {
            text.text = "P";
            text.color = Color.red; // ±ÛÀÚ»ö »¡°£»ö
            text.outlineColor = Color.white; // ¿Ü°û¼± Èò»ö
        }
    }
}
