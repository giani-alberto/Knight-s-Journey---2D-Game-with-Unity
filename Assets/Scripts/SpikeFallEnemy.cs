using System.Collections;
using UnityEngine;

public class SpikeFallEnemy : MonoBehaviour
{
    [Header("Setari Miscare")]
    public float vitezaCadere = 5f;
    public float vitezaRidicarelenta = 2f;
    public float timpAsteptareJos = 1f;

    [Header("Setari Senzor")]
    public float razaDetectie = 10f;
    public LayerMask layerJucator;

    private Rigidbody2D rb;
    private Vector3 pozitieInitiala;
    private bool ataca = false;
    private bool aLovitCeva = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pozitieInitiala = transform.position;
    }

    void Update()
    {
        if (ataca == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, razaDetectie, layerJucator);
            Debug.DrawRay(transform.position, Vector2.down * razaDetectie, Color.red);

            if (hit.collider != null)
            {
                StartCoroutine(CadeSiSeRidica());
            }
        }
    }

    IEnumerator CadeSiSeRidica()
    {
        ataca = true;
        aLovitCeva = false;

      
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = vitezaCadere;

  
        while (aLovitCeva == false)
        {
            yield return null;
        }

        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(timpAsteptareJos);

  
        while (Vector3.Distance(transform.position, pozitieInitiala) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pozitieInitiala, vitezaRidicarelenta * Time.deltaTime);
            yield return null;
        }

        transform.position = pozitieInitiala;
        ataca = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (ataca == true && rb.bodyType == RigidbodyType2D.Dynamic)
        {
      
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
            {
                aLovitCeva = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * razaDetectie);
    }

}