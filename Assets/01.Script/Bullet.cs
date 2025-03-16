using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Gwall")
        {
            Destroy(gameObject);
        }
    }
}
