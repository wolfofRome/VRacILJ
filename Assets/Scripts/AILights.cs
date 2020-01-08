using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILights : MonoBehaviour
{
    public GameObject 			brakeEffects;
	public GameObject 			frontLightEffects;
	
    public bool frontLightsOn = true;
    public bool brakeEffectsOn = false;
    // Start is called before the first frame update
    void Start()
    {
        SwitchFrontLights();
        SwitchBackLights();
    }

	public void SwitchFrontLights()
	{
		if (frontLightsOn) 
        {
			frontLightEffects.SetActive (true);
		} 
        else 
        {
			frontLightEffects.SetActive (false);
		}
	}

	public void SwitchBackLights()
	{
		if (brakeEffectsOn) 
        {
			brakeEffects.SetActive (true);
		} 
        else
        {
			brakeEffects.SetActive (false);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
