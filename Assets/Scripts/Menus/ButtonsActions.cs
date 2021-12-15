using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsActions : MonoBehaviour
{

    public void Resume()
    {
        GameController.GetGameController().ResumeGame();
    }
   public void FinishGame()
    {
        Application.Quit();
    }
    public void NextLevel()
    {
        LevelLoader.GetLoadLevel().LoadNextLevel();
    }
}
