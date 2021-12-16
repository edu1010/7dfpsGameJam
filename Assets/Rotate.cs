using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float m_RootSpeed = 50f;
    Vector3 currentEulerAngles = Vector3.zero;
    
    // Update is called once per frame
    void Update()
    {
        currentEulerAngles += new Vector3(0, 1, 0) * Time.deltaTime * m_RootSpeed;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentEulerAngles.y, transform.eulerAngles.z);
    }
}
