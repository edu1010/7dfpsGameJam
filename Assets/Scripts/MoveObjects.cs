
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjects : MonoBehaviour
{
    public List <Transform> m_WayyPoints;
    int m_CurrentWaypoint = 0;
    public float m_Time = 20;
    float m_Smooth = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float l_rateTiempo = 1f / m_Time;
        transform.position = Vector3.Lerp(transform.position, m_WayyPoints[m_CurrentWaypoint].position, l_rateTiempo);
            if(Vector3.Distance(transform.position, m_WayyPoints[m_CurrentWaypoint].position) <= 1)
        {
            m_CurrentWaypoint += 1;
            if(m_CurrentWaypoint >= m_WayyPoints.Count)
            {
                m_CurrentWaypoint = 0;
            }
        }
    }


}
