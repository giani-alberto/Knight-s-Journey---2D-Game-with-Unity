using UnityEngine;

public class TriggerActiveazaBoss : MonoBehaviour
{
 
    public BossPhase boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.CompareTag("Player"))
        {        
            if (boss != null)
            {
                boss.ActiveazaBoss();
            }        
            GetComponent<Collider2D>().enabled = false;
     
        }
    }
}