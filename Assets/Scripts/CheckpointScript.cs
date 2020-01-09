using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CheckpointScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other) {
        if(!(other.CompareTag("Car") || other.CompareTag("Player")))
            return;

        CarCheckpointScript ccs = other.transform.root.gameObject.GetComponent<CarCheckpointScript>();
        if(ccs.IsNextCheckpoint(transform))
        {
            ccs.NextCheckpoint();
        }
		CarAIControl ai = other.transform.root.gameObject.GetComponent<CarAIControl>();

		if(ai != null) 
		{
			ai.SetTarget(ccs.GetNextCheckPoint());
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
