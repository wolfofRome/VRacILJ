using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameManagerTimer : MonoBehaviour
{
	private static GameManagerTimer instance;
	private bool canVehiclesDrive = false;
	private AudioSource audioSource;
	public AudioClip three;
	public AudioClip two;
	public AudioClip one;
	public AudioClip go;
	public float TimeSinceStart; 

	public static GameManagerTimer GetInstance() 
	{
		return instance;
	}

	public bool CanVehiclesDrive() 
	{
		return canVehiclesDrive;
	}

	private void Awake() 
	{
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else
			instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(Countdown(3));
    }

	IEnumerator Countdown(int seconds) 
	{
		int count = seconds;

		while(count >= 0) 
		{
			yield return new WaitForSeconds(1);
			if (count == 3)
				audioSource.PlayOneShot(three);
			if (count == 2)
				audioSource.PlayOneShot(two);
			if (count == 1)
				audioSource.PlayOneShot(one);
			if (count == 0)
				audioSource.PlayOneShot(go);
			--count;
		}
		TimeSinceStart = 0;
		canVehiclesDrive = true;
	}

	public string GetTimerAsString()
	{
		string min = Mathf.Floor(TimeSinceStart / 60).ToString("00");
		string sec = (TimeSinceStart % 60).ToString("00");
		return (min + ":" + sec);
	}
	void Update()
	{
		TimeSinceStart += Time.deltaTime;
	}
}
