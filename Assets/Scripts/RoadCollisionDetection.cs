using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car { 

    public class RoadCollisionDetection : MonoBehaviour
    {
        public CarController playerCar;

        void OnTriggerEnter(Collider other)
        {
            if(other.tag == "CarWheel")
            {
                //Debug.Log("Enter");
                ++playerCar.cptWheelsOnRoad;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other.tag == "CarWheel")
            {
                //Debug.Log("Leave");
                --playerCar.cptWheelsOnRoad;
            }
        }
    }
}
