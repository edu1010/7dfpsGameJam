using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{
    public Animator m_transition;
    public float m_transitionTime = 1f;


    static LevelLoader m_LevelLoader = null;
   

    private void Awake()
    {
        if (m_LevelLoader == null)
        {
            m_LevelLoader = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(this); // ya existe, no hace falta crearla
        }
    }

    static public LevelLoader GetLoadLevel()
    {
        Debug.Log("loader");
        return m_LevelLoader;
    }
    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        else
            StartCoroutine(LoadLevel(0));
    }
    public void LoadNextLevel(int nextLevel)
    {
        StartCoroutine(LoadLevel(nextLevel));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
       
        m_transition.SetTrigger("Start");
        yield return new WaitForSeconds(m_transitionTime);
        AsyncOperation LoadLevel = SceneManager.LoadSceneAsync(levelIndex);
        LoadLevel.completed += (asyncOperation) =>
        {
            m_transition.SetTrigger("End");

            GameController.GetGameController().ResumeGame();
        };
    }

}


