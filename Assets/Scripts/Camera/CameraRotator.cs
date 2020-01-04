using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
	private Vector3 localRotation;
	private Transform parentTransform;
    
    public float VerticalSpeed;
    public float HorizontalSpeed;
	
	[Range(0.01f, 1.0f)]
    public float SmoothFactor;
	
    // Start is called before the first frame update
    void Start()
    {
		parentTransform = transform.parent;
		localRotation = new Vector3(-100, 20, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
		localRotation.x += Input.GetAxis("Mouse X") * HorizontalSpeed;
		localRotation.y += Input.GetAxis("Mouse Y") * VerticalSpeed;

		localRotation.y = Mathf.Clamp(localRotation.y, 0f, 90f);

		Quaternion quat = Quaternion.Euler(localRotation.y, localRotation.x, 0);
		parentTransform.rotation = Quaternion.Lerp(parentTransform.rotation, quat, SmoothFactor);
	}
}
