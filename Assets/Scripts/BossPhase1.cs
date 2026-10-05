using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;

public class BossPhase : MonoBehaviour, IDamageable
{
    #region 1. Variabile și Setări
    [Header("Setari Miscare & Atac")]
    public float speed = 1.5f;
    public float attackRange = 4.0f;
    public int attackDamage = 30;
    public float timpIncarcare = 0.8f;
    public float timpEpuizare = 2.0f;
    public float timpHurtRecuperare = 0.5f;

    [Header("Setari Tranzitie Faza 2")]
    public GameObject indicatorPrefab;
    public GameObject piatraPrefab;
    public float inaltimeTavan = 12f;
    public int numarPietre = 5;
    public Transform punctSpawnFaza2;

    [Header("Setari Faza 2")]
    public GameObject portalPrefab;

    [Header("Referinte Sistem")]
    public GameObject healthBarUI;
    public Slider healthSlider;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public int health = 20;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sunetCutremur;
    public GameObject containerMuzicaPestera;
    public AudioSource sursaMuzicaBoss;

    [Header("Cinematic Moarte")]
    public GameObject cheiePrefab;
    public Image flashImage;
    public AudioClip sunetMoarteLumina;

    private int maxHealth;
    private Animator anim;
    private Transform player;
    private SpriteRenderer[] toateDesenele;
    private CinemachineImpulseSource impulseSource;

    private bool isDead = false;
    private bool isSpawning = true;
    private bool isAttacking = false;
    private bool areVoieSaApara = false;
    private bool isHurt = false;
    private bool isExhausted = false;
    private bool inTranzitieFaza2 = false;
    public bool esteInFaza2 { get; private set; } = false;

    private Coroutine magicAttackCoroutine;
    private Coroutine hurtCoroutine;
    private Coroutine faza2LoopCoroutine;
    #endregion

    #region 2. Initializare & Update
    private void Start()
    {
        anim = GetComponent<Animator>();
        toateDesenele = GetComponentsInChildren<SpriteRenderer>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        SetVizibilitate(false);
        isSpawning = false;

        impulseSource = GetComponent<CinemachineImpulseSource>();

        maxHealth = health;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
        if (healthBarUI != null) healthBarUI.SetActive(false);
    }

    private void Update()
    {
        if (esteInFaza2) return;

        if (!areVoieSaApara || isDead || player == null || isSpawning || isAttacking || isHurt || inTranzitieFaza2) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange)
            magicAttackCoroutine = StartCoroutine(MagicAttackRoutine());
        else
            ChasePlayer();
    }
    #endregion

    #region 3. Activare & Faza 1
    public void ActiveazaBoss()
    {
        if (!areVoieSaApara && !isDead)
        {
            areVoieSaApara = true;
            if (healthBarUI != null) healthBarUI.SetActive(true);

            if (containerMuzicaPestera != null) containerMuzicaPestera.SetActive(false);
            if (sursaMuzicaBoss != null) sursaMuzicaBoss.Play();

            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        isSpawning = true;
        if (impulseSource != null) impulseSource.GenerateImpulse();

        if (audioSource != null && sunetCutremur != null)
        {
            audioSource.PlayOneShot(sunetCutremur);
        }

        anim.Play("Boss_Spawn", -1, 0f);
        anim.Update(0f); 
        SetVizibilitate(true);

     
        float durataSpawn = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(durataSpawn);

        isSpawning = false;
    }

    private void ChasePlayer()
    {
        anim.Play("Boss_Walk");
        Vector2 targetPos = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (esteInFaza2) return;

        float dir = player.position.x - transform.position.x;
        if (dir != 0) transform.localScale = new Vector3(dir > 0 ? -1 : 1, 1, 1);
    }

    private IEnumerator MagicAttackRoutine()
    {
        isAttacking = true;
        isExhausted = false;
        anim.Play("Boss_Atack");

        yield return new WaitForSeconds(0.2f);
        anim.speed = 0f;
        SetColor(new Color(1f, 0.4f, 0.4f));
        yield return new WaitForSeconds(timpIncarcare);

        anim.speed = 1.2f;
        SetColor(Color.white);
        yield return new WaitForSeconds(1.0f);

        isExhausted = true;
        anim.Play("Boss_Idle");
        SetColor(new Color(0.6f, 0.6f, 1f));
        yield return new WaitForSeconds(timpEpuizare);

        FinalizeazaAtac();
    }

    private void FinalizeazaAtac()
    {
        isExhausted = false;
        SetColor(Color.white);
        anim.speed = 1f;
        isAttacking = false;
    }

    public void HitPlayer()
    {
        if (isDead) return;
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hit != null)
        {
            Player playerScript = hit.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage);
            }
        }
    }
    #endregion

    #region 4. Faza 2 & Daune
    public void TakeDamage(int dmg)
    {
        if (isDead || isSpawning || !areVoieSaApara || inTranzitieFaza2) return;

        health -= dmg;
        if (healthSlider != null) healthSlider.value = health;

        if (!esteInFaza2 && health <= maxHealth / 2)
        {
            StartCoroutine(TranzitieFaza2Routine());
            return;
        }

        if (health > 0)
        {
            if (magicAttackCoroutine != null) StopCoroutine(magicAttackCoroutine);
            isAttacking = false;

            if (hurtCoroutine != null) StopCoroutine(hurtCoroutine);
            hurtCoroutine = StartCoroutine(HurtRoutine());
        }
        else
        {
            Moarte();
        }
    }

    private IEnumerator TranzitieFaza2Routine()
    {
        inTranzitieFaza2 = true;

        if (magicAttackCoroutine != null) StopCoroutine(magicAttackCoroutine);
        if (hurtCoroutine != null) StopCoroutine(hurtCoroutine);

        anim.speed = 1f;
        SetColor(Color.white);

        anim.Play("Boss_Death");
        yield return new WaitForSeconds(1.5f);
        SetVizibilitate(false);
        GetComponent<Collider2D>().enabled = false;

        if (audioSource != null && sunetCutremur != null)
        {
            audioSource.clip = sunetCutremur;
            audioSource.loop = true;
            audioSource.Play();
        }

        for (int i = 0; i < numarPietre; i++)
        {
            if (player == null) break;

            float tintaX = player.position.x;

            Vector2 pozitieIndicator = new Vector2(tintaX, player.position.y + 2f);
            GameObject indicator = Instantiate(indicatorPrefab, pozitieIndicator, Quaternion.identity);
            indicator.transform.SetParent(player);
            indicator.transform.localPosition = new Vector3(0, 1.5f, 0);
            if (impulseSource != null) impulseSource.GenerateImpulse(0.3f);

            yield return new WaitForSeconds(0.7f);
            Destroy(indicator);

            Vector2 pozitiePiatra = new Vector2(tintaX, inaltimeTavan);
            Instantiate(piatraPrefab, pozitiePiatra, Quaternion.identity);

            yield return new WaitForSeconds(0.8f);
        }

        GameObject[] pietreRamase = GameObject.FindGameObjectsWithTag("Damage");
        foreach (GameObject piatra in pietreRamase)
        {
            Destroy(piatra);
        }

        if (punctSpawnFaza2 != null)
        {
            transform.position = punctSpawnFaza2.position;
        }
        transform.localScale = new Vector3(1, 1, 1);

        GetComponent<Collider2D>().enabled = true;

        anim.Play("Boss_Spawn", -1, 0f);
        anim.Update(0f);
        SetVizibilitate(true);

        float durataSpawn2 = anim.GetCurrentAnimatorStateInfo(0).length;
        isSpawning = true;
        yield return new WaitForSeconds(durataSpawn2);
        isSpawning = false;

        esteInFaza2 = true;
        inTranzitieFaza2 = false;

        faza2LoopCoroutine = StartCoroutine(Faza2MotorRoutine());
    }

    private IEnumerator Faza2MotorRoutine()
    {
        while (esteInFaza2 && !isDead)
        {
            isAttacking = true;
            isExhausted = false;

            for (int i = 0; i < 4; i++)
            {
                if (isDead) yield break;

                anim.Play("Boss_Spell1");
                yield return new WaitForSeconds(1.0f);

                if (player != null)
                {
                    Instantiate(portalPrefab, new Vector2(player.position.x, -1.85f), Quaternion.identity);
                }

                yield return new WaitForSeconds(1.0f);

                anim.Play("Boss_Idle");

                yield return new WaitForSeconds(1.0f);
            }

            isExhausted = true;
            anim.Play("Boss_Idle");
            SetColor(new Color(0.6f, 0.6f, 1f));

            yield return new WaitForSeconds(timpEpuizare);

            SetColor(Color.white);
            isExhausted = false;
        }
    }

    private IEnumerator HurtRoutine()
    {
        if (esteInFaza2)
        {
            SetColor(new Color(1f, 0.4f, 0.4f));
            yield return new WaitForSeconds(0.2f);

            if (!isDead)
            {
                if (isExhausted) SetColor(new Color(0.6f, 0.6f, 1f));
                else SetColor(Color.white);
            }
            yield break;
        }

        isHurt = true;
        anim.speed = 1f;
        anim.Play("Boss_Hurt", -1, 0f);

        yield return new WaitForSeconds(timpHurtRecuperare);

        isHurt = false;

        if (!isDead)
        {
            if (isExhausted)
            {
                SetColor(new Color(0.6f, 0.6f, 1f));
                anim.Play("Boss_Idle");
            }
            else
            {
                SetColor(Color.white);
                isAttacking = false;
                anim.Play("Boss_Idle");
            }
        }
    }
    #endregion

    #region 5. Moarte & Utilitare
    private void Moarte()
    {
        if (isDead) return;
        isDead = true;

        if (magicAttackCoroutine != null) StopCoroutine(magicAttackCoroutine);
        if (hurtCoroutine != null) StopCoroutine(hurtCoroutine);
        if (faza2LoopCoroutine != null) StopCoroutine(faza2LoopCoroutine);

        anim.speed = 1f;
        SetColor(Color.white);
        anim.Play("Boss_Death");

        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (sursaMuzicaBoss != null) sursaMuzicaBoss.Stop();
        if (containerMuzicaPestera != null) containerMuzicaPestera.SetActive(true);
        if (audioSource != null) audioSource.Stop();

        Vector3 pozitieFixaCheie = transform.position;
        StartCoroutine(SecventaMoarteDramatica(pozitieFixaCheie));
    }

    private IEnumerator SecventaMoarteDramatica(Vector3 pozitieCheie)
    {
        Time.timeScale = 0.05f;

        if (audioSource != null && sunetMoarteLumina != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(sunetMoarteLumina);
        }

        float durataFlash = 1.5f;
        float timp = 0;

        while (timp < durataFlash)
        {
            timp += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, 1, Mathf.Pow(timp / durataFlash, 2));
            if (flashImage != null) flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        Time.timeScale = 1.0f;

        SetVizibilitate(false);
        if (healthBarUI != null) healthBarUI.SetActive(false);

        if (cheiePrefab != null)
        {
            Vector3 spawnPoint = new Vector3(pozitieCheie.x + 2.5f, pozitieCheie.y + 1f, pozitieCheie.z);
            Instantiate(cheiePrefab, spawnPoint, Quaternion.identity);
        }

        yield return new WaitForSecondsRealtime(0.8f);

        timp = 0;
        float durataFadeOut = 1.5f;
        while (timp < durataFadeOut)
        {
            timp += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timp / durataFadeOut);
            if (flashImage != null) flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void SetVizibilitate(bool eVisibil)
    {
        foreach (SpriteRenderer sr in toateDesenele) { sr.enabled = eVisibil; }
    }

    private void SetColor(Color c)
    {
        foreach (SpriteRenderer sr in toateDesenele) { sr.color = c; }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
}