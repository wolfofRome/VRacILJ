using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ComparePosition : IComparer<CarCheckpointScript>
{
    public int Compare(CarCheckpointScript x, CarCheckpointScript y)
    {
        if(x.totalCheckpointsPassed < y.totalCheckpointsPassed)
            return 1;
        else
            return -1;
    }
}
public class CarPositionsInRace : MonoBehaviour
{
    public List<CarCheckpointScript> cars;
    private ComparePosition comparePosition;
    
    void Awake()
    {
        comparePosition = new ComparePosition();
        cars = new List<CarCheckpointScript>((CarCheckpointScript[])Object.FindObjectsOfType(typeof(CarCheckpointScript)));
    }

    // Update is called once per frame
    void Update()
    {
        cars.Sort(comparePosition);
        for(int i = 0; i < cars.Count; ++i)
        {
            cars[i].positionInRace = i+1;
        }
    }
}
