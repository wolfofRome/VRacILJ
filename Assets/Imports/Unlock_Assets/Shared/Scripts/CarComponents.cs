using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;




public class CarComponents : MonoBehaviour {

	public bool blink;
	[Header("Lights")]
	public bool frontLightsOn;
	public bool brakeEffectsOn;

	public Transform			SpeedNeedle;
	public Vector2				SpeedNeedleRotateRange	= Vector3.zero; 
	private Vector3 			SpeedEulers				=   Vector3.zero;
	public Transform			RpmNeedle;
	public Vector2				RpmNeedleRotateRange  	= Vector3.zero; 
	private Vector3 			RpmdEulers				=   Vector3.zero;
	public float				_NeedleSmoothing 		= 1.0f;
	public Transform 			steeringWheel;
	public GameObject 			brakeEffects;
	public GameObject 			frontLightEffects;
	public GameObject 			reverseEffect;
	private float 				rotateNeedles			= 0.0f;

	public Text txtSpeed, txtRPM;

	private IEnumerator coroutine;


	void Start () {
		blink 			= true;
		frontLightsOn 	= true;
		brakeEffectsOn 	= true;

		if (SpeedNeedle) SpeedEulers = SpeedNeedle.localEulerAngles;
		if (RpmNeedle) RpmdEulers = RpmNeedle.localEulerAngles;

		coroutine = WaitLights(2.0f);
		StartCoroutine(coroutine);

	
	}
	

	void Update () {
		if (blink) {
			TurnOnFrontLights ();
			TurnOnBackLights ();
		}

        /*
		if (SpeedNeedle) {

				Vector3 temp = new Vector3 (SpeedEulers.x, SpeedEulers.y, Mathf.Lerp (SpeedNeedleRotateRange.x, SpeedNeedleRotateRange.y, (rotateNeedles)));
				SpeedNeedle.localEulerAngles = Vector3.Lerp (SpeedNeedle.localEulerAngles, temp, Time.deltaTime * _NeedleSmoothing);

		}
        */

		if (RpmNeedle)
		{
			Vector3 temp = new Vector3( RpmdEulers.x,RpmdEulers.y,Mathf.Lerp( RpmNeedleRotateRange.x, RpmNeedleRotateRange.y,	(rotateNeedles)));
			RpmNeedle.localEulerAngles = Vector3.Lerp( RpmNeedle.localEulerAngles, temp, Time.deltaTime * _NeedleSmoothing);
		}

        /*
		if (steeringWheel != null) {
			Vector3 eulers = steeringWheel.localRotation.eulerAngles;
			eulers.z = rotateNeedles * 15.0f;

			steeringWheel.localRotation = Quaternion.Slerp (steeringWheel.localRotation, Quaternion.Euler (eulers), Time.deltaTime * 2.5f);

		}
        */

		// txtSpeed.text = ((int)(rotateNeedles * 100.0f)).ToString () + " km/h";
		if(txtRPM != null)
			txtRPM.text = ((int)(rotateNeedles * 1000.0f)).ToString ();
		
	}

	public void TurnOnFrontLights()
	{
		if (frontLightsOn) {
			frontLightEffects.SetActive (true);
			rotateNeedles += Time.deltaTime;
		} else {
			frontLightEffects.SetActive (false);
			rotateNeedles -= Time.deltaTime;
		}
	}

	public void TurnOnBackLights()
	{
		if (brakeEffectsOn) {
			brakeEffects.SetActive (true);
		} else {
			brakeEffects.SetActive (false);
		}
	}



	private IEnumerator WaitLights(float waitTime) {
		while (true) {
			yield return new WaitForSeconds(waitTime);
			frontLightsOn = !frontLightsOn;
			brakeEffectsOn = !brakeEffectsOn;
		}
	}


}
