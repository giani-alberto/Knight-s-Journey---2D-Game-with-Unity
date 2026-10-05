using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Sunete")]
    public AudioClip sunetCheie; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            if (playerScript != null)
            {
            
                if (playerScript.sfxSource != null && sunetCheie != null)
                {
                    playerScript.sfxSource.PlayOneShot(sunetCheie);
                }
            

              
                playerScript.ObtainKey();

            
                Destroy(gameObject);
            }
        }
    }
}