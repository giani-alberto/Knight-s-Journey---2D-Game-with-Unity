using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Butoane Meniu Principal")]
    public Button continueButton;

    [Header("UI Level Select")]
    public GameObject panelLevelSelect; 
    public Button butonLevel2;          
    public Button butonLevel3;        

    void Start()
    {
        Application.targetFrameRate = 120;
        int salvareExistenta = PlayerPrefs.GetInt("AreSalvare", 0);
        if (continueButton != null)
        {
            continueButton.interactable = (salvareExistenta == 1);
        }

    
        if (panelLevelSelect != null)
        {
            panelLevelSelect.SetActive(false);
        }
    }

   
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level 0");
    }

    public void ContinueGame()
    {
        if (SaveManager.instance != null && SaveManager.instance.AreSalvare())
        {
            string scenaDeIncarcat = SaveManager.instance.IncarcaUltimaScena();
            PlayerPrefs.SetInt("IncarcaPozitia", 1);
            SceneManager.LoadScene(scenaDeIncarcat);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DeschideLevelSelect()
    {
        panelLevelSelect.SetActive(true);

        if (butonLevel2 != null) butonLevel2.interactable = true;
        if (butonLevel3 != null) butonLevel3.interactable = true;
    }

    public void InchideLevelSelect()
    {
        panelLevelSelect.SetActive(false);
    }
    public void IncarcaNivelDinSelect(string numeNivel)
    {
        Time.timeScale = 1.0f;


        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

      
        SceneManager.LoadScene(numeNivel);
    }
}