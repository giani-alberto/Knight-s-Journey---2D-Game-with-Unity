using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    private Animator childAnimator;
    private string idUnic;
    private bool aFostActivat = false;

    [Header("Sistem Audio")]
    public AudioSource audioSource;
    public AudioClip sunetCheckpoint;

    void Start()
    {
        childAnimator = GetComponentInChildren<Animator>();

        float posX = Mathf.Round(transform.position.x * 100f) / 100f;
        idUnic = "Check_" + SceneManager.GetActiveScene().name + "_" + posX;

        if (PlayerPrefs.GetInt(idUnic, 0) == 1)
        {
            aFostActivat = true;
            if (childAnimator != null) childAnimator.SetBool("active", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !aFostActivat)
        {
            Activeaza(other.gameObject);
        }
    }

    void Activeaza(GameObject jucator)
    {
        aFostActivat = true;
        if (audioSource != null && sunetCheckpoint != null)
        {
            audioSource.PlayOneShot(sunetCheckpoint,0.2f);
        }

        if (childAnimator != null)
        {
            childAnimator.SetBool("active", true);
        }

        if (SaveManager.instance != null)
        {
            Player p = jucator.GetComponent<Player>();
            SaveManager.instance.SalveazaPozitie(transform.position.x, transform.position.y);
            SaveManager.instance.SalveazaViata(p.health);
            SaveManager.instance.SalveazaMonede(p.coins);
            SaveManager.instance.SalveazaScenaCurenta();

            PlayerPrefs.SetInt("NivelSalvat", SceneManager.GetActiveScene().buildIndex);

            PlayerPrefs.SetInt(idUnic, 1);
            PlayerPrefs.SetInt("IncarcaPozitia", 1);
            PlayerPrefs.Save();
        }
    }
}