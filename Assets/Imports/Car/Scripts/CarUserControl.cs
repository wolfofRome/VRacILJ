using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float f1 = Input.GetAxis("Fire1");
#if !MOBILE_INPUT
            float handbrake = Input.GetAxis("Jump");
            if(Input.GetButton("Respawn"))
            {
                gameObject.GetComponent<CarCheckpointScript>().Respawn();
            }
            m_Car.Move(h, v, v, handbrake,f1);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
