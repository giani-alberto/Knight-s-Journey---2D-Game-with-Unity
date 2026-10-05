using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player p = col.GetComponent<Player>();
            if (p != null) p.TakeDamage(damage);
        }
    }
}