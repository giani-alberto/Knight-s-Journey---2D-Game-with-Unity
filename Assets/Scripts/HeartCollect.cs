using UnityEngine;
using UnityEngine.SceneManagement; 

public class HeartCollect : MonoBehaviour
{
    public int healthAmount = 20;

    [Header("Setari Plutire")]
    public float vitezaPlutire = 2f;
    public float inaltimePlutire = 0.2f;

    [Header("Sunete")]
    public AudioClip sunetInima; 

    private Vector2 pozitieInitiala;
    private string idUnic; 

    void Start()
    {
        pozitieInitiala = transform.position;
        idUnic = SceneManager.GetActiveScene().name + "_" + gameObject.name;

        if (PlayerPrefs.GetInt(idUnic, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        float nouY = pozitieInitiala.y + Mathf.Sin(Time.time * vitezaPlutire) * inaltimePlutire;
        transform.position = new Vector2(pozitieInitiala.x, nouY);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
        
            if (playerScript != null && playerScript.health < 100)
            {
                playerScript.health += healthAmount;
                if (playerScript.health > 100) playerScript.health = 100;
                if (playerScript.sfxSource != null && sunetInima != null)
                {
                    playerScript.sfxSource.PlayOneShot(sunetInima,0.5f);
                }

                if (SaveManager.instance != null)
                {
                    SaveManager.instance.SalveazaViata(playerScript.health);
                    PlayerPrefs.SetInt(idUnic, 1);
                    PlayerPrefs.Save();
                    SaveManager.instance.SalveazaScenaCurenta();
                }
                Destroy(gameObject);
            }
        }
    }
}