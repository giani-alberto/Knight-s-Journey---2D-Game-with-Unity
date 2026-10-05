using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string nextLevelName;

    public void LoadNextLevel()
    {
      
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SalveazaNivelulDeblocat(2);
        }

    
        PlayerPrefs.SetInt("IncarcaPozitia", 0);
        PlayerPrefs.Save();

        Time.timeScale = 1.0f;

       
        SceneManager.LoadScene(nextLevelName);
    }
}