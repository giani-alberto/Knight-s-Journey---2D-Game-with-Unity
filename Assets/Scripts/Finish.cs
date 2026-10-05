using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    public GameObject winUI;

    [Header("Sunete")]
    public AudioSource sfxSource;
    public AudioClip sunetFinish;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           
            Player jucatorScript = collision.GetComponent<Player>();
            if (jucatorScript != null)
            {
                jucatorScript.OpresteTotLaFinish();
            }

            
            if (sfxSource != null && sunetFinish != null)
            {
                sfxSource.PlayOneShot(sunetFinish);
            }

            Time.timeScale = 0f;
            winUI.SetActive(true);
        }
    }
}