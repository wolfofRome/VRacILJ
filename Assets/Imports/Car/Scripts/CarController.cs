using System;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

    internal enum TransmitionType
    {
        Manual,
        Auto
    }

    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private TransmitionType m_TransmitionType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 6;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        /**Variables added**/
        public Transform SteeringWheel;
        public Text SpeedText;
        public Transform SpeedNeedle;
        public Text GearText;
        public float[] powerPerGear = { 2.66f, 1.78f, 1.3f, 1f, .7f, .5f };
        public float[] maxSpeedPerGearFactor = { .15f, .30f, .50f, .75f, .85f, 1 };
        public float[] minSpeedPerGearFactor = { .0f, .15f, .30f, .50f, .75f, .85f };
        private bool gearChangeLock = false;

        // Use this for initialization
        private void Start()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);

            if(powerPerGear[NoOfGears - 1] < 1)
            {
                float offset = 1 - powerPerGear[NoOfGears - 1];
                for(int i = 0; i < powerPerGear.Length; ++i)
                {
                    powerPerGear[i] += offset;
                }
            }
        }

        private void GearChanging()
        {
            GearChanging(0);
        }

        public  float cameraTiltWhenGearChange = 2.5f;

        private void GearChanging(float gearChange)
        {

            if(m_TransmitionType == TransmitionType.Manual)
            {
                if (!gearChangeLock)
                {
                    m_GearNum = Mathf.Clamp(m_GearNum + (int)gearChange, 0, NoOfGears-1);
                    if(gearChange > 0)
                    {
                        //MainCamera.transform.Rotate(new Vector3(cameraTiltWhenGearChange, 0, 0));
                    }
                    else if (gearChange < 0)
                    {
                        //MainCamera.transform.Rotate(new Vector3(-cameraTiltWhenGearChange, 0, 0));
                        //Debug.Log(MainCamera.transform.localEulerAngles);
                    }
                }

                if(gearChange != 0)
                {
                    gearChangeLock = true;
                }
                else
                {
                    gearChangeLock = false;
                }
            }
            else
            {
                float speed = m_Rigidbody.velocity.magnitude;
                switch (m_SpeedType)
                {
                    case SpeedType.MPH:
                        speed *= 2.23693629f;
                        break;
                    case SpeedType.KPH:
                        speed *= 3.6f;
                        break;
                }

                float f = Mathf.Abs(speed / MaxSpeed);
                float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
                float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

                if (m_GearNum > 0 && f < downgearlimit)
                {
                    m_GearNum--;
                }

                if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
                {
                    m_GearNum++;
                }
            }
            if(GearText != null)           
                GearText.text = (m_GearNum + 1).ToString();
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }

        private float ConvertSpeed(float speed)
        {
            switch (m_SpeedType)
            {
                case SpeedType.MPH:
                    speed *= 2.23693629f;
                    break;
                case SpeedType.KPH:
                    speed *= 3.6f;
                    break;
            }
            return speed;
        }

        private void CalculateGearFactor()
        {
            float speed = ConvertSpeed(m_Rigidbody.velocity.magnitude);
            float minGearSpeed = m_Topspeed * minSpeedPerGearFactor[m_GearNum];
            float topGearSpeed = m_Topspeed * maxSpeedPerGearFactor[m_GearNum];

            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(minGearSpeed, topGearSpeed, Mathf.Abs(speed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = m_GearNum/(float) NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }

        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            Move(steering, accel, footbrake, handbrake, 0);
        }

        public void Move(float steering, float accel, float footbrake, float handbrake, float gearChange)
        {
			if (GameManagerTimer.GetInstance() != null && !GameManagerTimer.GetInstance().CanVehiclesDrive())
				return;
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            if(gameObject.tag == "Car")
            {
                if(footbrake < 0)
                    GetComponent<AILights>().brakeEffectsOn = true;
                else
                    GetComponent<AILights>().brakeEffectsOn = false;
                GetComponent<AILights>().SwitchBackLights(); 
            }
            BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering*m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            /*
            if (handbrake > 0f)
            {
                var hbTorque = handbrake*m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }*/


            CalculateRevs();
            GearChanging(gearChange);

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();

            /* Speed */
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    break;
            }

            AdjustSteeringWheelRotation();
            if(gameObject.tag == "Player")
            {
                AdjustNeedleRotationAndSpeedText(speed);
                AdjustCameraZoomAndRotation(speed);

                float leftRumble = m_GearFactor - rumbleCapTrigger;
                float rightRumble = (4 - cptWheelsOnRoad)*2;
                TriggerGearRumble(rightRumble, leftRumble);
                
            }

        }
        public int cptWheelsOnRoad = 0;

        public float rumbleCapTrigger = .9f;

        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            float topGearSpeed = m_Topspeed * maxSpeedPerGearFactor[m_GearNum];
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > topGearSpeed)
                        m_Rigidbody.velocity = (topGearSpeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > topGearSpeed) {
                        Vector3 speedV = m_Rigidbody.velocity;
                        Vector3 aimedAtSpeed = (topGearSpeed / 3.6f) * m_Rigidbody.velocity.normalized;
                        m_Rigidbody.velocity = Vector3.Lerp(speedV, aimedAtSpeed, Time.deltaTime * _NeedleSmoothing);
                    }
                    break;
            }
        }


        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;
            accel *= powerPerGear[m_GearNum];
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    //Debug.Log("Accel : " +accel+"curTorque : "+ m_CurrentTorque+"Thurst torque :  " + thrustTorque);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
                }
                else if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                    m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
                }
            }
        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce*
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }


        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }

        /**Start of handwritten**/

        public Vector2 SpeedNeedleRotateRange = Vector3.zero;
        public float _NeedleSmoothing = 1;
        public Transform RPMNeedle;
        public Vector2 RPMNeedleRotateRange = Vector3.zero;
        public Camera MainCamera;

        private void AdjustSteeringWheelRotation()
        {
            if (SteeringWheel != null)
            {
                Vector3 eulers = SteeringWheel.localRotation.eulerAngles;
                eulers.z = -m_SteerAngle * 2;

                SteeringWheel.localRotation = Quaternion.Slerp(SteeringWheel.localRotation, Quaternion.Euler(eulers), Time.deltaTime * 2.5f);

            }
        }

        private void AdjustNeedleRotationAndSpeedText(float speed)
        {
            if(SpeedText != null)            
                SpeedText.text = ((int)speed).ToString();

            Vector3 SpeedEulers = SpeedNeedle.localRotation.eulerAngles;
            Vector3 temp = new Vector3(SpeedEulers.x, SpeedEulers.y, -Mathf.Lerp(SpeedNeedleRotateRange.x, SpeedNeedleRotateRange.y, speed / MaxSpeed));

            SpeedNeedle.localEulerAngles = Vector3.Lerp(temp, SpeedNeedle.localEulerAngles, Time.deltaTime * _NeedleSmoothing);

            /* RPM */
            if(RPMNeedle != null)
            {
                Vector3 RPMEulers = RPMNeedle.localRotation.eulerAngles;
                temp = new Vector3(RPMEulers.x, RPMEulers.y, -Mathf.Lerp(RPMNeedleRotateRange.x, RPMNeedleRotateRange.y, m_GearFactor));

                RPMNeedle.localEulerAngles = Vector3.Lerp(temp, RPMNeedle.localEulerAngles, Time.deltaTime * _NeedleSmoothing);
            }
        }

        public Vector2 fovLimits = new Vector2(60,80);

        private void AdjustCameraZoomAndRotation(float speed)
        {
            float speedFactor = speed / m_Topspeed;
            float newFOV = Mathf.Lerp(fovLimits.x, fovLimits.y, speedFactor);

            MainCamera.fieldOfView = newFOV;
        }


        bool playerIndexSet = false;
        PlayerIndex playerIndex;
        GamePadState state;
        GamePadState prevState;

        private void TriggerGearRumble(float left, float right)
        {
            // Find a PlayerIndex, for a single player game
            // Will find the first controller that is connected ans use it
            if (!playerIndexSet || !prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex = testPlayerIndex;
                        playerIndexSet = true;
                    }
                }
            }
            prevState = state;
            state = GamePad.GetState(playerIndex);

            //Actually vibrating
            GamePad.SetVibration(playerIndex, left, right);
        }
    }
}
