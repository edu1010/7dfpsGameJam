using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.GetGameController().SetGameState(GameStates.PAUSE);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameController.GetGameController().SetGameState(GameStates.PLAY);
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}
