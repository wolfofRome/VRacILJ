using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUserControlled : MonoBehaviour
{
    public float m_maxX = 15, m_minX = -5, m_maxY = 80, m_minY = -80;
    public GameObject parent;

    private Vector3 rotationAdjust = new Vector3(0,0,0);
    private Vector3 initialRotation;

    void Start()
    {
        initialRotation = transform.eulerAngles;
    }

    void Update()
    {
        float xAxis = Input.GetAxis("Mouse X");
        float yAxis = Input.GetAxis("Mouse Y");

        rotationAdjust.x = Mathf.Clamp(yAxis * -1 + rotationAdjust.x,m_minX,m_maxX);
        rotationAdjust.y = Mathf.Clamp(xAxis + rotationAdjust.y,m_minY,m_maxY);

        Vector3 positiveRotation = rotationAdjust;
        if (positiveRotation.x < 0) positiveRotation.x += 360;
        if (positiveRotation.y < 0) positiveRotation.y += 360;
        //print(rotationAdjust + " ## " +positiveRotation);

        Vector3 parentRotation = parent.transform.eulerAngles;

        transform.eulerAngles = initialRotation + parentRotation + positiveRotation;
    }
}
