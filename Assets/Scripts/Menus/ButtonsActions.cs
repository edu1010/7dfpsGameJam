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
            if (m_timer > 1f)
            {
                canPres = true;
                m_timer = 0f;
            }
        }
    }
    public void Resume()
    {
        GameController.GetGameController().ResumeGame();
        canPres = false;
    }
   public void FinishGame()
    {
        Application.Quit();
    }
    public void NextLevel()
    {
        LevelLoader.GetLoadLevel().LoadNextLevel();
        canPres = false;
    }
    public void ResetLevel()
    {
        GameController.GetGameController().ResetLevel();
        canPres = false;
    }

    public void GoToFirstLevel()
    {
        LevelLoader.GetLoadLevel().LoadNextLevel(1);
        canPres = false;
    }
}
