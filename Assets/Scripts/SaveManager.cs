using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SalveazaMonede(int numarMonede)
    {
        PlayerPrefs.SetInt("MonedeSalvate", numarMonede);
        PlayerPrefs.Save();
    }
    public void SalveazaViata(int viataCurenta)
    {
        PlayerPrefs.SetInt("ViataSalvata", viataCurenta);
        PlayerPrefs.Save();
    }

   
    public int IncarcaViata()
    {
     
        return PlayerPrefs.GetInt("ViataSalvata", 100);
    }

    public void SalveazaNivelulDeblocat(int indexNivel)
    {
        int nivelCurentSalvat = PlayerPrefs.GetInt("NivelDeblocat", 1);
        if (indexNivel > nivelCurentSalvat)
        {
            PlayerPrefs.SetInt("NivelDeblocat", indexNivel);
            PlayerPrefs.Save();
        }
    }

    public int IncarcaMonede()
    {
        return PlayerPrefs.GetInt("MonedeSalvate", 0);
    }

    public int IncarcaNivelulDeblocat()
    {
        return PlayerPrefs.GetInt("NivelDeblocat", 1);
    }

   
    public void SalveazaPozitie(float x, float y)
    {
        PlayerPrefs.SetFloat("PlayerX", x);
        PlayerPrefs.SetFloat("PlayerY", y);
        PlayerPrefs.SetInt("AreSalvare", 1); 
        PlayerPrefs.Save();
    }
    public void SalveazaScenaCurenta()
    {
    
        string numeScena = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("UltimaScenaSalvata", numeScena);
        PlayerPrefs.Save();
        Debug.Log("Scena salvata: " + numeScena);
    }

    public string IncarcaUltimaScena()
    {
     
        return PlayerPrefs.GetString("UltimaScenaSalvata", "Level 1");
    }

  
    public float IncarcaX() { return PlayerPrefs.GetFloat("PlayerX", 0); }
    public float IncarcaY() { return PlayerPrefs.GetFloat("PlayerY", 0); }
    public bool AreSalvare() { return PlayerPrefs.GetInt("AreSalvare", 0) == 1; }
}