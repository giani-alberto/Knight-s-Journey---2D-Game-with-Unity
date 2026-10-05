using UnityEngine;

public class Skeleton_Enemy : MonoBehaviour, IDamageable
{
    [Header("Setari Miscare")]
    public float speed = 2f;
    public Transform[] points;
    private int i = 0;

    [Header("Setari Combat")]
    public float detectionRange = 2.5f;
    public float attackRange = 0.8f;
    public float attackCooldown = 2f;
    private float nextAttackTime;
    public int attackDamage = 20;

    [Header("Efecte Hit")]
    public float knockbackForceX = 3f;
    public float knockbackForceY = 2f;
    public Color hitColor = Color.red;
    public float blinkDuration = 0.15f;

    [Header("Referinte")]
    public Transform attackPoint;
    public LayerMask playerLayer;
    public int health = 2;

    private Animator anim;
    private Transform player;
    private bool isDead = false;

    private SpriteRenderer[] toateDesenele;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        toateDesenele = GetComponentsInChildren<SpriteRenderer>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Skeleton_Attack_2") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            LookAtPlayer();


            if (Time.time >= nextAttackTime)
            {
                anim.Play("Skeleton_Attack_2");
                nextAttackTime = Time.time + attackCooldown;
            }
            else
            {

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Skeleton_Idle"))
                {
                    anim.Play("Skeleton_Idle");
                }
            }
        }
        else
        {
            Patrol(); 
        }
    }

    void Patrol()
    {
        anim.Play("Skeleton_Walking");

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, points[i].position) < 0.2f)
        {
            i = (i + 1) % points.Length;
        }


        float dir = points[i].position.x - transform.position.x;
        if (dir != 0) transform.localScale = new Vector3(dir > 0 ? 1 : -1, 1, 1);
    }

    void LookAtPlayer()
    {
        float dir = player.position.x - transform.position.x;
        if (dir != 0) transform.localScale = new Vector3(dir > 0 ? 1 : -1, 1, 1);
    }


    public void HitPlayer()
    {
        if (isDead) return;
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hit != null)
        {
            hit.GetComponent<Player>().TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        health -= dmg;

        StartCoroutine(HitFeedback());

        if (health <= 0) Moarte();
    }

    private System.Collections.IEnumerator HitFeedback()
    {

        foreach (SpriteRenderer sr in toateDesenele)
        {
            if (sr != null) sr.color = hitColor;
        }

        if (rb != null && player != null)
        {
            float pushDirection = transform.position.x > player.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(pushDirection * knockbackForceX, knockbackForceY);
        }

        yield return new WaitForSeconds(blinkDuration);


        foreach (SpriteRenderer sr in toateDesenele)
        {
            if (sr != null) sr.color = Color.white;
        }
    }

    void Moarte()
    {
        if (isDead) return;
        isDead = true;

        anim.Play("Skeleton_Dying");

        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.simulated = false;

        Invoke("AscundeCadavrul", 1.5f);
    }

    void AscundeCadavrul()
    {
        gameObject.SetActive(false); 
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}