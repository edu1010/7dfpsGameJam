using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsActions : MonoBehaviour
{
    bool canPres = true;
    float m_timer = 0f;
    private void Update()
    {
        if (!canPres)
        {
            m_timer += Time.deltaTime;
            if (m_timer > 5f)
            {
                canPres = true;
                m_timer = 0f;
            }
        }
    }
    public void Resume()
    {
        if (canPres)
        {
            GameController.GetGameController().ResumeGame();
            canPres = false;
        }
    }
   public void FinishGame()
    {
        Application.Quit();
    }
    public void NextLevel()
    {
        if (canPres)
        {
            LevelLoader.GetLoadLevel().LoadNextLevel();
            canPres = false;
        }
        
    }
    public void ResetLevel()
    {
        if (canPres)
        {
            GameController.GetGameController().ResetLevel();
            canPres = false;
        }
        
    }

    public void GoToFirstLevel()
    {
        if (canPres)
        {
            LevelLoader.GetLoadLevel().LoadNextLevel(1);
            canPres = false;
        }
       
    }
}
