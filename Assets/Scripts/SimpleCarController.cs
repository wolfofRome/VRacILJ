using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour{
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;
    
    public  WheelCollider FLWheel, FRWheel, RLWheel, RRWheel;
    public Transform FLTransform, FRTransform, RLTransform, RRTransform; 
    public float maxSteerAngle = 30;
    public float motorForce = 500;

    public void GetInput(){
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer(){
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        FRWheel.steerAngle = m_steeringAngle;
        FLWheel.steerAngle = m_steeringAngle;
    }

    private void Accelerate(){
        FRWheel.motorTorque = m_verticalInput * motorForce;
        FLWheel.motorTorque = m_verticalInput * motorForce;
    }

    private void UpdateWheelPoses(){
        UpdateWheelPose(FLWheel,FLTransform);
        UpdateWheelPose(FRWheel,FRTransform);
        UpdateWheelPose(RLWheel,RLTransform);
        UpdateWheelPose(RRWheel,RRTransform);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform){
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);
        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    private void FixedUpdate() {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }
}
