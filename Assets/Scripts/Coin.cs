using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private string idUnic;

    private void Start()
    {
        string numeScena = SceneManager.GetActiveScene().name;
        idUnic = "Moneda_" + numeScena + "_" + transform.position.x + "_" + transform.position.y;

        if (PlayerPrefs.GetInt(idUnic, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().AddCoin(value, idUnic);
            Destroy(gameObject);
        }
    }
}