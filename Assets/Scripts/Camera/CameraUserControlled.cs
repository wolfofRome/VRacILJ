using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUserControlled : MonoBehaviour
{
    public float m_maxX = 15, m_minX = -5, m_maxY = 80, m_minY = -80;
    public GameObject parent;

    //How much the camera is rotated compared to its start rotation
    private Vector3 rotationAdjust = new Vector3(0,0,0);

    private int m_CameraSmoothing = 1;



    void Update()
    {
        float xAxis = Input.GetAxis("Mouse X");
        float yAxis = Input.GetAxis("Mouse Y");
        
        if(false && xAxis == 0 && yAxis == 0)
        {
            /*
            float readjustSpeed = 5;
            Vector3 currentRotation = transform.eulerAngles;
            Vector3 adjustRotation = new Vector3(0,0,0);
            
            if (currentRotation.x < 180)
            {
                adjustRotation.x = currentRotation.x;
                adjustRotation.x = Mathf.Clamp(adjustRotation.x, 0, readjustSpeed);
                adjustRotation.x *= -1;
            }
            else
            {
                adjustRotation.x = 360 - currentRotation.x;
                adjustRotation.x = Mathf.Clamp(adjustRotation.x, 0, readjustSpeed);
            }
            if (currentRotation.y < 180)
            {
                adjustRotation.y = currentRotation.y;
                adjustRotation.y = Mathf.Clamp(adjustRotation.y, 0, readjustSpeed);
                adjustRotation.y *= -1;
            }
            else
            {
                adjustRotation.y = 360 - currentRotation.y;
                adjustRotation.y = Mathf.Clamp(adjustRotation.y, 0, readjustSpeed);
            }

            Debug.Log("Current rotation " + currentRotation + "\nDelta : " + adjustRotation);


            transform.Rotate(adjustRotation, Space.Self);

            rotationAdjust += adjustRotation;
            //transform.Rotate(Vector3.Lerp(transform.localEulerAngles, adjustRotation, Time.deltaTime * 1),Space.Self);
            */
        }
        else
        {
            Debug.Log("Current rotation : " + transform.localEulerAngles);
            yAxis *= -1;
            transform.Rotate(new Vector3(yAxis,xAxis,0.0f),Space.World);

            if ((transform.localEulerAngles.y > m_maxY && transform.localEulerAngles.y < 180) ||
                (transform.localEulerAngles.y < 360 + m_minY && transform.localEulerAngles.y > 180))
            {
                transform.Rotate(new Vector3(0, -xAxis, 0.0f));
            }

            if ((transform.localEulerAngles.x > m_maxX && transform.localEulerAngles.x < 180) ||
                (transform.localEulerAngles.x < 360 + m_minX && transform.localEulerAngles.x > 180))
            {
                transform.Rotate(new Vector3(-yAxis, 0, 0.0f));
            }
        }
    }
}
