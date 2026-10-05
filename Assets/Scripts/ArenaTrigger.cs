using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BossTransition : MonoBehaviour
{
    [Header("Camera Transitions")]
    public GameObject playerCamera;
    public GameObject bossCamera;

    [Header("Lighting Settings")]
    public Light2D globalLight;
    public float targetIntensity = 1f;
    public float lightFadeDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerCamera != null) playerCamera.SetActive(false);
            if (bossCamera != null) bossCamera.SetActive(true);

            if (globalLight != null)
            {
                StartCoroutine(FadeInLight());
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }

    private IEnumerator FadeInLight()
    {
        float startIntensity = globalLight.intensity;
        float timeElapsed = 0;

        while (timeElapsed < lightFadeDuration)
        {
            timeElapsed += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / lightFadeDuration);
            yield return null;
        }

        globalLight.intensity = targetIntensity;
    }
}