using UnityEngine;

public class Restart : MonoBehaviour
{
  public void LoadCurrentScene()
    {
        Time.timeScale = 1.0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
       
    }
}
