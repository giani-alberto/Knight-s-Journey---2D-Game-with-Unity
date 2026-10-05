using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BossExitTransition : MonoBehaviour
{
    [Header("Camera Transitions")]
    public GameObject playerCamera;
    public GameObject bossCamera;

    [Header("Lighting Settings")]
    public Light2D globalLight;
    public float normalIntensity = 0.7f;
    public float lightFadeDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerCamera != null) playerCamera.SetActive(true);
            if (bossCamera != null) bossCamera.SetActive(false);

            if (globalLight != null)
            {
                StartCoroutine(FadeOutLight());
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }

    private IEnumerator FadeOutLight()
    {
        float startIntensity = globalLight.intensity;
        float timeElapsed = 0;

        while (timeElapsed < lightFadeDuration)
        {
            timeElapsed += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(startIntensity, normalIntensity, timeElapsed / lightFadeDuration);
            yield return null;
        }

        globalLight.intensity = normalIntensity;
    }
}