using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PrincessTrigger : MonoBehaviour
{
    [Header("Configurare")]
    public string numeScenaFinal = "Credits";

    [Header("Efecte Vizuale & Audio")]
    public Light2D luminaMagica;
    public Image ecranAlb;
    public AudioSource audioSource;
    public AudioClip sunetMagie;

    private bool aFostAtinsa = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !aFostAtinsa)
        {
            aFostAtinsa = true;

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            StartCoroutine(SecventaFinala());
        }
    }

    private IEnumerator SecventaFinala()
    {
        if (audioSource != null && sunetMagie != null)
        {
            audioSource.PlayOneShot(sunetMagie);
        }

        Time.timeScale = 0.4f;

        float durataFade = 2.5f;
        float timp = 0;
        float intensitateInitiala = luminaMagica != null ? luminaMagica.intensity : 0;

        while (timp < durataFade)
        {
            timp += Time.unscaledDeltaTime;
            float progres = timp / durataFade;

            if (luminaMagica != null)
            {
                luminaMagica.intensity = Mathf.Lerp(intensitateInitiala, 15f, progres);
            }

            if (ecranAlb != null)
            {
                Color culoare = ecranAlb.color;
                culoare.a = progres;
                ecranAlb.color = culoare;
            }

            yield return null;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(numeScenaFinal);
    }
}