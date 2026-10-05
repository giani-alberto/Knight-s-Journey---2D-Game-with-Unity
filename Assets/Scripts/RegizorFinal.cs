using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class RegizorFinal : MonoBehaviour
{
    [Header("Setari Fade")]
    public Image ecranNegru;
    public float durataFadeIn = 4f;
    public float durataFadeOut = 4f;

    [Header("Setari Camera")]
    public Transform cameraPrincipala;
    public float inaltimeUrcare = 12f;
    public float durataUrcare = 8f;
    public float asteptareInainteDeUrcare = 2f;

    [Header("Setari Credite")]
    public GameObject obiectTextCredite;
    public float vitezaScroll = 120f;
    public float durataPanaLaFinalText = 15f;
    public string numeScenaMeniu = "Main Menu";

    private bool incepeScroll = false;

    void Start()
    {
        if (ecranNegru != null) ecranNegru.color = Color.black;
        if (obiectTextCredite != null) obiectTextCredite.SetActive(false);

        StartCoroutine(RuleazaFilmul());
    }

    private IEnumerator RuleazaFilmul()
    {
        float t = 0;
        while (t < durataFadeIn)
        {
            t += Time.deltaTime;
            float transparenta = Mathf.Lerp(1f, 0f, t / durataFadeIn);
            ecranNegru.color = new Color(0, 0, 0, transparenta);
            yield return null;
        }
        ecranNegru.gameObject.SetActive(false);

        yield return new WaitForSeconds(asteptareInainteDeUrcare);

        Vector3 pozitieStart = cameraPrincipala.position;
        Vector3 pozitieFinala = pozitieStart + new Vector3(0, inaltimeUrcare, 0);
        float timpCamera = 0;

        while (timpCamera < durataUrcare)
        {
            timpCamera += Time.deltaTime;
            float progres = Mathf.SmoothStep(0f, 1f, timpCamera / durataUrcare);
            cameraPrincipala.position = Vector3.Lerp(pozitieStart, pozitieFinala, progres);
            yield return null;
        }

        if (obiectTextCredite != null)
        {
            obiectTextCredite.SetActive(true);
            incepeScroll = true;
        }

        yield return new WaitForSeconds(durataPanaLaFinalText);

        incepeScroll = false;

        ecranNegru.gameObject.SetActive(true);
        t = 0;
        while (t < durataFadeOut)
        {
            t += Time.deltaTime;
            float transparenta = Mathf.Lerp(0f, 1f, t / durataFadeOut);
            ecranNegru.color = new Color(0, 0, 0, transparenta);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(numeScenaMeniu);
    }

    public void Skip()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("Main Menu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Skip();
        }

        if (incepeScroll && obiectTextCredite != null)
        {
            obiectTextCredite.transform.Translate(Vector3.up * vitezaScroll * Time.deltaTime);
        }
    }
}