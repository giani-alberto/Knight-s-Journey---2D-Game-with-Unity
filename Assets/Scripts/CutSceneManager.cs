using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Actori")]
    public GameObject playerDummy;
    public GameObject printesaDummy;
    public GameObject bossDummy;
    public GameObject semnExclamare;

    [Header("Animatoare")]
    public Animator playerAnimator;
    public Animator printesaAnimator;
    public Animator bossAnimator;

    [Header("Puncte de Destinatie")]
    public Transform punctOprirePlayer;
    public Transform punctOprirePrintesa;

    [Header("Setari Viteza")]
    public float vitezaMiscarePrintesa = 1.5f;
    public float vitezaMiscarePlayer = 3f;

    [Header("Efecte Cinematice (UI)")]
    public RectTransform baraSus;
    public RectTransform baraJos;
    public Image panouEfecte;
    public float inaltimeBara = 150f;
    public TextMeshProUGUI textSkip;

    [Header("Efecte Vizuale")]
    public GameObject prefabFum;

    [Header("Sistem Audio")]
    public AudioSource audioSourceMuzica;
    public AudioSource audioSourceEfecte;

    [Space]
    public AudioClip sunetPasi;
    public AudioClip sunetAparitieBoss;
    public AudioClip sunetCutremur;
    public AudioClip sunetRapirePoof;

    void Start()
    {
        if (bossDummy != null) bossDummy.SetActive(false);
        if (semnExclamare != null) semnExclamare.SetActive(false);

        if (panouEfecte != null) panouEfecte.color = new Color(0, 0, 0, 0);

        if (baraSus != null) baraSus.anchoredPosition = new Vector2(0, inaltimeBara);
        if (baraJos != null) baraJos.anchoredPosition = new Vector2(0, -inaltimeBara);

        if (audioSourceMuzica != null)
        {
            audioSourceMuzica.loop = true;
            audioSourceMuzica.Play();
        }

        StartCoroutine(RuleazaFilmul());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Skip(); 
        }

        if (textSkip != null)
        {
            Color c = textSkip.color;
            c.a = Mathf.Lerp(0.3f, 1f, Mathf.PingPong(Time.time * 1.5f, 1f));
            textSkip.color = c;
        }
    }

    public void Skip()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("Level 1");
    }

    IEnumerator RuleazaFilmul()
    {
        StartCoroutine(AnimaBenzi(0, 1f));
        yield return new WaitForSeconds(0.5f);

        if (playerAnimator != null) playerAnimator.Play("Run");
        if (printesaAnimator != null) printesaAnimator.Play("Princess_Walk");

        if (audioSourceEfecte != null && sunetPasi != null)
        {
            audioSourceEfecte.clip = sunetPasi;
            audioSourceEfecte.loop = true;
            audioSourceEfecte.Play();
        }

        Vector2 destPlayer = new Vector2(punctOprirePlayer.position.x, playerDummy.transform.position.y);
        Vector2 destPrintesa = new Vector2(punctOprirePrintesa.position.x, printesaDummy.transform.position.y);

        bool playerAjunsa = false;
        bool printesaAjunsa = false;

        while (!playerAjunsa || !printesaAjunsa)
        {
            if (!playerAjunsa)
            {
                playerDummy.transform.position = Vector2.MoveTowards(playerDummy.transform.position, destPlayer, vitezaMiscarePlayer * Time.deltaTime);
                if (Vector2.Distance(playerDummy.transform.position, destPlayer) <= 0.05f)
                {
                    playerAjunsa = true;
                    if (playerAnimator != null) playerAnimator.Play("Idle");
                }
            }
            if (!printesaAjunsa)
            {
                printesaDummy.transform.position = Vector2.MoveTowards(printesaDummy.transform.position, destPrintesa, vitezaMiscarePrintesa * Time.deltaTime);
                if (Vector2.Distance(printesaDummy.transform.position, destPrintesa) <= 0.05f)
                {
                    printesaAjunsa = true;
                    if (printesaAnimator != null) printesaAnimator.Play("Princess_Idle");
                }
            }
            yield return null;
        }

        if (audioSourceEfecte != null)
        {
            audioSourceEfecte.Stop();
            audioSourceEfecte.loop = false;
        }

        if (bossDummy != null) bossDummy.SetActive(true);
        if (bossAnimator != null) bossAnimator.Play("Boss_Spawn");

        if (audioSourceEfecte != null && sunetAparitieBoss != null)
            audioSourceEfecte.PlayOneShot(sunetAparitieBoss);

        StartCoroutine(AnimaPanou(0.5f, 1.5f));

        if (semnExclamare != null) semnExclamare.SetActive(true);
        yield return new WaitForSeconds(1f);

        if (semnExclamare != null) semnExclamare.SetActive(false);
        if (bossAnimator != null) bossAnimator.Play("Boss_Idle");

        if (audioSourceEfecte != null && sunetCutremur != null)
            audioSourceEfecte.PlayOneShot(sunetCutremur);

        StartCoroutine(ShakeCamera(0.4f, 0.15f));
        yield return new WaitForSeconds(1.5f);

        if (printesaDummy != null)
        {
            if (prefabFum != null) Instantiate(prefabFum, printesaDummy.transform.position, Quaternion.identity);
            printesaDummy.SetActive(false);
        }

        if (audioSourceEfecte != null && sunetRapirePoof != null)
            audioSourceEfecte.PlayOneShot(sunetRapirePoof);

        if (bossAnimator != null) bossAnimator.Play("Boss_Death");

        yield return new WaitForSeconds(1f);
        if (bossDummy != null) bossDummy.SetActive(false);

        if (audioSourceMuzica != null) audioSourceMuzica.Stop();


        if (textSkip != null) textSkip.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(AnimaPanou(1.0f, 1.2f));
        yield return new WaitForSeconds(1.5f);
        Skip();
    }

    IEnumerator AnimaPanou(float targetAlpha, float duration)
    {
        if (panouEfecte == null) yield break;
        float elapsed = 0f;
        Color startColor = panouEfecte.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panouEfecte.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }
    }

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Transform camTransform = Camera.main.transform;
        Vector3 originalPos = camTransform.position;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
            camTransform.position = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime; yield return null;
        }
        camTransform.position = originalPos;
    }

    IEnumerator AnimaBenzi(float targetY, float duration)
    {
        if (baraSus == null || baraJos == null) yield break;
        float elapsed = 0f;
        Vector2 startSus = baraSus.anchoredPosition; Vector2 startJos = baraJos.anchoredPosition;
        Vector2 endSus = new Vector2(startSus.x, targetY); Vector2 endJos = new Vector2(startJos.x, -targetY);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime; float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            baraSus.anchoredPosition = Vector2.Lerp(startSus, endSus, t);
            baraJos.anchoredPosition = Vector2.Lerp(startJos, endJos, t);
            yield return null;
        }
    }
}