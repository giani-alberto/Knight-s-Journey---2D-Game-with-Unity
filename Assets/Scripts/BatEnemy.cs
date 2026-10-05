using UnityEngine;

public class Bat_Enemy : MonoBehaviour, IDamageable
{
    #region 1. Variabile și Setări
    [Header("Setari Miscare (Zbor)")]
    public float speedPatrol = 2f;
    public float speedChase = 4f;
    public Transform[] points;
    private int i = 0;

    [Header("Setari Combat")]
    public float detectionRange = 5f;
    public float attackRange = 0.8f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Referinte Sistem")]
    public Transform attackPoint;
    public LayerMask playerLayer;
    public int health = 1;

    private float nextAttackTime;
    private Animator anim;
    private Transform player;
    private bool isDead = false;
    #endregion

    #region 2. Initializare & Update
    private void Start()
    {
        anim = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Bat_Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            LookAtPlayer();

            if (dist <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    anim.Play("Bat_Attack");
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            else
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Bat_Fly"))
                    anim.Play("Bat_Fly");

                transform.position = Vector2.MoveTowards(transform.position, player.position, speedChase * Time.deltaTime);
            }
        }
        else
        {
            Patrol();
        }
    }
    #endregion

    #region 3. Comportament & AI
    private void Patrol()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Bat_Fly"))
            anim.Play("Bat_Fly");

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speedPatrol * Time.deltaTime);

        if (Vector2.Distance(transform.position, points[i].position) < 0.2f)
        {
            i = (i + 1) % points.Length;
        }

        float dir = points[i].position.x - transform.position.x;
        if (dir != 0) transform.localScale = new Vector3(dir > 0 ? -1 : 1, 1, 1);
    }

    private void LookAtPlayer()
    {
        float dir = player.position.x - transform.position.x;
        if (dir != 0) transform.localScale = new Vector3(dir > 0 ? -1 : 1, 1, 1);
    }
    #endregion

    #region 4. Interacțiuni (Combat)
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

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        health -= dmg;
        if (health <= 0) Moarte();
    }

    private void Moarte()
    {
        if (isDead) return;
        isDead = true;
        anim.Play("Bat_Death");

        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Destroy(gameObject, 1.0f);
    }
    #endregion

    #region 5. Utilitare (Gizmos)
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion
}