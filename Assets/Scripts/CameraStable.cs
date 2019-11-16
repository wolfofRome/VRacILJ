using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStable : MonoBehaviour{

    public GameObject followedCar;
    public float carX;
    public float carY;
    public float carZ;

    void Update(){
        carX = followedCar.transform.eulerAngles.x;
        carY = followedCar.transform.eulerAngles.y;
        carZ = followedCar.transform.eulerAngles.z;

        transform.eulerAngles = new Vector3(carX - carX, carY, carZ - carZ);
    }
}
