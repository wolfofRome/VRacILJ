using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUserControlled : MonoBehaviour
{
    public float m_maxX = 15, m_minX = -5, m_maxY = 80, m_minY = -80;

    public float cameraReadjustSpeed = 2;
    public float delayBeforeReadjust = .25f;
    public GameObject leftMirror;
    public GameObject rightMirror;

    private float m_ReadjustTimer = 0;

    void Update()
    {
        float xAxis = Input.GetAxis("Mouse X");
        float yAxis = Input.GetAxis("Mouse Y");
        
        if(xAxis == 0 && yAxis == 0)
        {
            m_ReadjustTimer += Time.deltaTime;
            if(m_ReadjustTimer > delayBeforeReadjust)
            {
                Vector3 currentRotation = transform.localEulerAngles;
                Vector3 adjustRotation = new Vector3(0, 0, 0);

                if (currentRotation.x < 180)
                {
                    adjustRotation.x = currentRotation.x;
                    adjustRotation.x = Mathf.Clamp(adjustRotation.x, 0, cameraReadjustSpeed);
                    adjustRotation.x *= -1;
                }
                else
                {
                    adjustRotation.x = 360 - currentRotation.x;
                    adjustRotation.x = Mathf.Clamp(adjustRotation.x, 0, cameraReadjustSpeed);
                }
                if (currentRotation.y < 180)
                {
                    adjustRotation.y = currentRotation.y;
                    adjustRotation.y = Mathf.Clamp(adjustRotation.y, 0, cameraReadjustSpeed);
                    adjustRotation.y *= -1;
                }
                else
                {
                    adjustRotation.y = 360 - currentRotation.y;
                    adjustRotation.y = Mathf.Clamp(adjustRotation.y, 0, cameraReadjustSpeed);
                }
                //Debug.Log("Current rotation " + currentRotation + "\nDelta : " + adjustRotation);
                transform.Rotate(adjustRotation, Space.Self);
            }            
        }
        else
        {
            m_ReadjustTimer = 0.0f;
            //Debug.Log("Current rotation : " + transform.localEulerAngles);
            yAxis *= -1;
            transform.Rotate(new Vector3(yAxis,xAxis,0.0f),Space.World);

            if ((transform.localEulerAngles.y > m_maxY && transform.localEulerAngles.y < 180) ||
                (transform.localEulerAngles.y < 360 + m_minY && transform.localEulerAngles.y > 180))
            {
                transform.Rotate(new Vector3(0.0f, -xAxis, 0.0f));
            }

            if ((transform.localEulerAngles.x > m_maxX && transform.localEulerAngles.x < 180) ||
                (transform.localEulerAngles.x < 360 + m_minX && transform.localEulerAngles.x > 180))
            {
                transform.Rotate(new Vector3(-yAxis, 0.0f, 0.0f));
            }
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
        if(transform.localEulerAngles.y < 10f)
            rightMirror.SetActive(false);
        else
            rightMirror.SetActive(true);
        if(transform.localEulerAngles.y > 15f && transform.localEulerAngles.y < 250f)
            leftMirror.SetActive(false);
        else
            leftMirror.SetActive(true);
    }
}
