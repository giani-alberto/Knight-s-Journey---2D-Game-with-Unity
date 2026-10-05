using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    IEnumerator LoadLevelCoroutine(int levelIndex)
    {
    
        transition.SetTrigger("StartFade");

       
        yield return new WaitForSeconds(transitionTime);

        
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(levelIndex);
    }
}