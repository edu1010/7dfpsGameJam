using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimeUpdater : MonoBehaviour
{
    TextMeshProUGUI M_Text;
    
    // Start is called before the first frame update
    void Start()
    {
        M_Text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        M_Text.text = "Time: " + (int)GameController.GetGameController().GetTimeInLevel() +" seconds";
    }
}
