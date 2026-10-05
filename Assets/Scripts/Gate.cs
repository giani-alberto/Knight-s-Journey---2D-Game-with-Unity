using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Sunete")]
    public AudioClip sunetDeschiderePoarta; 

  
    private void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Player playerScript = collision.gameObject.GetComponent<Player>();

           
            if (playerScript != null && playerScript.hasKey == true)
            {
               
                if (playerScript.sfxSource != null && sunetDeschiderePoarta != null)
                {
                    playerScript.sfxSource.PlayOneShot(sunetDeschiderePoarta);
                }
             

             
                if (playerScript.KeyIcon != null)
                {
                    playerScript.KeyIcon.SetActive(false);
                    playerScript.hasKey = false; 
                }

               
                Destroy(gameObject);
            }
        }
    }
}