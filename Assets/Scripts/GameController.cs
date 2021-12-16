using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum GameStates
{
    PLAY = 0,
    PAUSE,
    FinishLevel
}
public class GameController : MonoBehaviour
{
    List<IRestartGameElements> m_RestartGameElements;
  

    Player m_Player;

    static GameController m_GameController = null;
    CanvasGroup m_PauseCanvas;
    CanvasGroup m_FinishCanvas;
    GameStates m_GameStates = GameStates.PLAY;
    float m_TimeInLevel = 0f;
    private CanvasGroup m_GameHud;

    private void Awake()
    {
        if (m_GameController == null)
        {
            m_GameController = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(this); // ya existe, no hace falta crearla
        }
        
        
        m_RestartGameElements = new List<IRestartGameElements>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_GameStates == GameStates.PLAY)
                StopGame();
            else if (m_GameStates == GameStates.PAUSE)
                ResumeGame();
        }
        if(m_GameStates == GameStates.PLAY)
            m_TimeInLevel += Time.deltaTime;
    }
    public void AddRestartGameElement (IRestartGameElements RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }
    public void EmptyRestartGameElement()
    {
        m_RestartGameElements = null;
        m_RestartGameElements = new List<IRestartGameElements>();
    }

    public void RestartGame()
    {
        foreach (IRestartGameElements l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
    }
    public void ResetLevel()
    {
        m_Player.ResetLevel();
        RestartGame();

    }

   

    static public GameController GetGameController()
    {
        return m_GameController;
    }

    public void SetPlayer(Player _Player)
    {
        m_Player = _Player;
        m_TimeInLevel = 0;
    }
    public Player GetPlayer()
    {
        return m_Player;
    }
    public void StopGame()
    {
        Time.timeScale = 0f;
        ShoweMouse();
        ShowCanvasGroup(m_PauseCanvas);
        HideCanvasGroup(m_GameHud);
        m_GameStates = GameStates.PAUSE;
    }
    public void ResumeGame()
    {
        Debug.Log("hi");
        Time.timeScale = 1f;
        HideCanvasGroup(m_PauseCanvas);
        ShowCanvasGroup(m_GameHud);
        m_GameStates = GameStates.PLAY;
        Debug.Log("hi");
        HideMouse();
    }
    public void FinishGameLevel()
    {
        Time.timeScale = 1f;
        m_GameStates = GameStates.FinishLevel;
        ShoweMouse();
        ShowCanvasGroup(m_FinishCanvas);
        HideCanvasGroup(m_PauseCanvas);
        HideCanvasGroup(m_GameHud);

    }
    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ShoweMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetPauseCanvas(CanvasGroup _canvas)
    {
        m_PauseCanvas = _canvas;
    } 
    public void SetGameHudCanvas(CanvasGroup _canvas)
    {
        m_GameHud = _canvas;
    }
    public void SetFinishCanvas(CanvasGroup _canvas)
    {
        m_FinishCanvas = _canvas;
    }
    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public GameStates GetGameState()
    {
        return m_GameStates;
    }
    public void SetGameState(GameStates _GameStates)
    {
         m_GameStates= _GameStates;
    }
    public float GetTimeInLevel()
    {
        return m_TimeInLevel;
    }
}
