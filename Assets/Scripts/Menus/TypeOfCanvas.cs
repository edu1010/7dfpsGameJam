using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOfCanvas : MonoBehaviour
{
    public TCanvas m_canvas;
    // Start is called before the first frame update
    void Awake()
    {
        switch (m_canvas)
        {
            case (TCanvas.pauseCanvas):
                GameController.GetGameController().SetPauseCanvas(gameObject.GetComponent<CanvasGroup>());
                break;
            case (TCanvas.FinishLevelCanvas):
                GameController.GetGameController().SetFinishCanvas(gameObject.GetComponent<CanvasGroup>());
                break;
            case (TCanvas.GameHud):
                GameController.GetGameController().SetGameHudCanvas(gameObject.GetComponent<CanvasGroup>());
                break;
        }
    }
   
}
public enum TCanvas
{
    pauseCanvas,
    FinishLevelCanvas,
    GameHud
}