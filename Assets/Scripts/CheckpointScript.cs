using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("Checkpoint validé : " + gameObject.name);
            ccs.NextCheckpoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
