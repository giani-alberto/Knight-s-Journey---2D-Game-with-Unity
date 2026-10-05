using System.Collections;
using UnityEngine;

public class PlatformaCazatoare : MonoBehaviour
{
    public float timpPanaCade = 1.0f;     
    public float timpPanaRevine = 1.5f;   
    public float intensitateTremur = 0.05f;

    private Rigidbody2D rb;
    private BoxCollider2D col; 
    private Vector3 pozitieInitiala;
    private bool aFostAtinga = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>(); 
        pozitieInitiala = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !aFostAtinga)
        {
            if (collision.contacts[0].normal.y <= -0.5f)
            {
                aFostAtinga = true;
                StartCoroutine(TremuraCadeSiRevine());
            }
        }
    }

    IEnumerator TremuraCadeSiRevine()
    {
        float timpTrecut = 0f;

        while (timpTrecut < timpPanaCade)
        {
            float xAleator = Random.Range(-1f, 1f) * intensitateTremur;
            float yAleator = Random.Range(-1f, 1f) * intensitateTremur;
            transform.position = pozitieInitiala + new Vector3(xAleator, yAleator, 0);

            timpTrecut += Time.deltaTime;
            yield return null;
        }

        transform.position = pozitieInitiala;
        rb.bodyType = RigidbodyType2D.Dynamic;

    
      
        col.enabled = false;

      
        yield return new WaitForSeconds(timpPanaRevine);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;        
        transform.position = pozitieInitiala;

        col.enabled = true;

        aFostAtinga = false;
    }
}