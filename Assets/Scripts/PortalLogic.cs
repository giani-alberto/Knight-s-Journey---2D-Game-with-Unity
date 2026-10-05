using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    [Header("Componente")]
    public GameObject hitbox;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetAparitiePortal;
    public AudioClip sunetImpactMana;

    private void Start()
    {
        if (hitbox != null) hitbox.SetActive(false);

        if (audioSource != null && sunetAparitiePortal != null)
        {
            audioSource.PlayOneShot(sunetAparitiePortal);
        }

        Destroy(gameObject, 2.0f);
    }

    public void ActiveazaHitbox()
    {
        if (hitbox != null) hitbox.SetActive(true);

        if (audioSource != null && sunetImpactMana != null)
        {
            audioSource.PlayOneShot(sunetImpactMana);
        }
    }
}