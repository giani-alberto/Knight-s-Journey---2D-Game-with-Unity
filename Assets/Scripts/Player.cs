using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region 1. Variabile si Setari
    [Header("Movement")]
    public Transform groundCheck;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Health & UI")]
    public int health = 100;
    public GameObject inima1, inima2, inima3, inima4, inima5;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers;
    public float detectionRange = 4f;
    public LayerMask enemyLayer;

    [Header("Collectables")]
    public int coins = 0;
    public TMPro.TextMeshProUGUI coinText;
    public bool hasKey = false;
    public GameObject KeyIcon;

    [Header("Sunete")]
    public AudioSource sfxSource;
    public AudioClip sunetBan;
    public AudioSource runSource;
    public AudioClip[] suneteSaritura;
    public AudioClip sunetAterizare;

    [Header("Sunete Combat")]
    public AudioClip sunetSabie;
    public AudioClip[] suneteEfort;
    public AudioClip sunetHurt;
    public AudioClip sunetDeath;

    [Header("Polisare - Particule")]
    public GameObject prafSarituraPrefab;
    public ParticleSystem prafAlergare;

    [Header("Efecte Impact Inamici")]
    public AudioClip sunetImpact;
    public GameObject efectImpactPrefab;

    private bool isDead = false;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private bool wasGroundedLastFrame;
    private float nextAttackTime = 0f;
    private float moveInput;
    private bool levelFinished = false;


    private string monedeDinViataAsta = "";


    private bool miscareStanga = false;
    private bool miscareDreapta = false;
    private bool vreaSaSara = false;
    private bool vreaSaAtace = false;
    private bool vreaRestart = false;
    #endregion

    #region 2. Initializare (Start & FixedUpdate)
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play("Recover");

        if (KeyIcon != null) KeyIcon.SetActive(false);

        IncarcaSalvarea();
        ActualizeazaInimi();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    #endregion

    #region 3. Bucla Principala (Update)
    void Update()
    {
        ActualizeazaInimi();

        if (isDead || levelFinished) return;

        HandleMovementAndParticles();
        HandleJumpAndLanding();
        HandleCombatInput();
        HandleRestartInput();

        UpdateAnimations();
    }
    #endregion

    #region 4. Sisteme de Miscare si Control
    private void HandleMovementAndParticles()
    {
        moveInput = Input.GetAxis("Horizontal");
        if (miscareStanga) moveInput = -1f;
        if (miscareDreapta) moveInput = 1f;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
   
        if (moveInput > 0) transform.localScale = new Vector2(-1, 1);
        else if (moveInput < 0) transform.localScale = new Vector2(1, 1);

        if (isGrounded && moveInput != 0)
        {
            if (prafAlergare != null && !prafAlergare.isPlaying) prafAlergare.Play();
            if (runSource != null && !runSource.isPlaying) runSource.Play();
        }
        else
        {
            if (prafAlergare != null && prafAlergare.isPlaying) prafAlergare.Stop();
            if (runSource != null && runSource.isPlaying) runSource.Stop();
        }
    }
    private void HandleJumpAndLanding()
    {    
        bool incearcaSaSara = Input.GetButtonDown("Jump") || vreaSaSara;
        vreaSaSara = false; 

        if (incearcaSaSara && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (sfxSource != null && suneteSaritura != null && suneteSaritura.Length > 0)
            {
                int indexAleatoriu = Random.Range(0, suneteSaritura.Length);
                sfxSource.PlayOneShot(suneteSaritura[indexAleatoriu], 0.4f);
            }
            if (prafSarituraPrefab != null)
                Instantiate(prafSarituraPrefab, groundCheck.position, Quaternion.identity);
        }
        if (!wasGroundedLastFrame && isGrounded)
        {
            if (prafSarituraPrefab != null)
                Instantiate(prafSarituraPrefab, groundCheck.position, Quaternion.identity);

            if (sfxSource != null && sunetAterizare != null)
            {
                sfxSource.pitch = Random.Range(0.8f, 1.2f);
                sfxSource.PlayOneShot(sunetAterizare, 0.3f);
            }
        }
        wasGroundedLastFrame = isGrounded;
    }

    private void HandleCombatInput()
    {
        bool incearcaSaAtace = Input.GetKeyDown(KeyCode.E) || vreaSaAtace;
        vreaSaAtace = false; 

        if (incearcaSaAtace && !isDead && Time.time >= nextAttackTime)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            nextAttackTime = Time.time + attackCooldown;
            Attack();
        }
    }

    private void HandleRestartInput()
    {
        bool incearcaRestart = Input.GetKeyDown(KeyCode.R) || vreaRestart;
        vreaRestart = false;

        if (incearcaRestart)
        {
            int nivelSalvat = PlayerPrefs.GetInt("NivelSalvat", -1);
            int nivelCurent = SceneManager.GetActiveScene().buildIndex;

            if (nivelSalvat == nivelCurent)
            {
                PlayerPrefs.SetInt("IncarcaPozitia", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IncarcaPozitia", 0);
            }

            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    #endregion

    #region 5. Combat, Sănătate & Salvări
    private void Attack()
    {
        animator.Play("Attack");

        if (sfxSource != null)
        {
            if (sunetSabie != null)
            {
                sfxSource.PlayOneShot(sunetSabie, 0.5f);
            }

            if (suneteEfort != null && suneteEfort.Length > 0)
            {
                int indexAleatoriu = Random.Range(0, suneteEfort.Length);
                sfxSource.PlayOneShot(suneteEfort[indexAleatoriu], 0.5f);
            }
        }
    }

    public void HitEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        bool hasHitSomething = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable enemy_hit = enemy.GetComponent<IDamageable>();
            if (enemy_hit != null)
            {
                enemy_hit.TakeDamage(1);
                hasHitSomething = true;

                if (efectImpactPrefab != null)
                {
                    Instantiate(efectImpactPrefab, enemy.transform.position, Quaternion.identity);
                }

            }
        }

        if (hasHitSomething)
        {
            if (sfxSource != null && sunetImpact != null)
            {
                sfxSource.pitch = Random.Range(0.85f, 1.15f);
                sfxSource.PlayOneShot(sunetImpact, 0.5f);
            }
            StartCoroutine(HitStopTimer(0.04f));
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        health -= damageAmount;
        if (SaveManager.instance != null) SaveManager.instance.SalveazaViata(health);

        if (health > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.Play("Hurt");

            if (sfxSource != null && sunetHurt != null)
            {
                sfxSource.PlayOneShot(sunetHurt, 0.4f);
            }
        }
        else
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage" && !isDead)
        {
            TakeDamage(20);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Checkpoint>() != null)
        {
            if (!string.IsNullOrEmpty(monedeDinViataAsta))
            {
                string[] ids = monedeDinViataAsta.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in ids)
                {
                    PlayerPrefs.SetInt(id, 1);
                }
                PlayerPrefs.Save();
                monedeDinViataAsta = "";
            }
        }
        if (collision.CompareTag("Death") && !isDead)
        {
            health = 0;
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        if (sfxSource != null && sunetDeath != null)
        {
            sfxSource.PlayOneShot(sunetDeath);
        }

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        rb.linearVelocity = new Vector2(-7f * transform.localScale.x, 5f);
        animator.Play("Death");

        if (SaveManager.instance != null) SaveManager.instance.SalveazaViata(100);

        yield return new WaitForSeconds(2.0f);
        PlayerPrefs.SetInt("IncarcaPozitia", 1);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    #endregion

    #region 6. Interacțiuni & UI
    public void ObtainKey()
    {
        hasKey = true;
        if (KeyIcon != null) KeyIcon.SetActive(true);
    }

    public void AddCoin(int amount, string idMoneda = "")
    {
        coins += amount;
        if (coinText != null) coinText.text = coins.ToString();

        if (sfxSource != null && sunetBan != null) sfxSource.PlayOneShot(sunetBan, 0.3f);

        if (!string.IsNullOrEmpty(idMoneda))
        {
            monedeDinViataAsta += idMoneda + ",";
        }
    }

    private void ActualizeazaInimi()
    {
        if (inima5 != null) inima5.SetActive(health >= 100);
        if (inima4 != null) inima4.SetActive(health >= 80);
        if (inima3 != null) inima3.SetActive(health >= 60);
        if (inima2 != null) inima2.SetActive(health >= 40);
        if (inima1 != null) inima1.SetActive(health >= 20);
    }

    private void IncarcaSalvarea()
    {
        if (SaveManager.instance != null)
        {
            if (PlayerPrefs.GetInt("IncarcaPozitia", 0) == 1 && SaveManager.instance.AreSalvare())
            {
                float x = SaveManager.instance.IncarcaX();
                float y = SaveManager.instance.IncarcaY();
                rb.position = new Vector2(x, y);
                transform.position = new Vector3(x, y, transform.position.z);
            }

            PlayerPrefs.SetInt("IncarcaPozitia", 0);
            PlayerPrefs.Save();

            coins = SaveManager.instance.IncarcaMonede();
            if (coinText != null) coinText.text = coins.ToString();
            health = SaveManager.instance.IncarcaViata();
        }
    }
    #endregion

    #region 7. Animații & Utilitare
    private void UpdateAnimations()
    {
        bool enemyNearby = Physics2D.OverlapCircle(transform.position, detectionRange, enemyLayer) != null;
        var state = animator.GetCurrentAnimatorStateInfo(0);

        if ((state.IsName("Hurt") || state.IsName("Recover") || state.IsName("Attack")) && state.normalizedTime < 1.0f) return;

        if (isGrounded)
        {
            if (moveInput == 0) animator.Play(enemyNearby ? "Combat Idle" : "Idle");
            else animator.Play("Run");
        }

        else if (rb.linearVelocity.y > 0)
        {
            animator.Play("Jump");
        }
    }

    public void OpresteTotLaFinish()
    {
        levelFinished = true;
        if (runSource != null) runSource.Stop();
        if (prafAlergare != null) prafAlergare.Stop();
        rb.linearVelocity = Vector2.zero;
        animator.Play("Idle");
    }

    private System.Collections.IEnumerator HitStopTimer(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion

    #region 8. Functii pentru Mobile
    public void ApasaStanga() { miscareStanga = true; }
    public void RidicaStanga() { miscareStanga = false; }

    public void ApasaDreapta() { miscareDreapta = true; }
    public void RidicaDreapta() { miscareDreapta = false; }

    public void ApasaSaritura() { vreaSaSara = true; }
    public void ApasaAtac() { vreaSaAtace = true; }
    public void ApasaRestart() { vreaRestart = true; }

    #endregion
}