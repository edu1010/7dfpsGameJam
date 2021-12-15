using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum GameStates
{
    PLAY = 0,
    PAUSE
}
public class GameController : MonoBehaviour
{
    List<IRestartGameElements> m_RestartGameElements;
  

    Player m_Player;

    static GameController m_GameController = null;

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

    public void AddRestartGameElement (IRestartGameElements RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }

    public void RestartGame()
    {
        foreach (IRestartGameElements l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
    }

   

    static public GameController GetGameController()
    {
        return m_GameController;
    }

    public void SetPlayer(Player _Player)
    {
        m_Player = _Player;
    }
    public Player GetPlayer()
    {
        return m_Player;
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
}
