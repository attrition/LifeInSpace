using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
    public float Mass = 40f; // in kT

    public float Thrust = 0f; // current thrust (0 up to MaxThrust)
    public float MaxThrust = 50f; // property of the engine
    public float SecondsToMaxThrust = 1f;
    public float TimeStartedThrusting = 0f;

    public float ReverseThrust = 0f;
    public float MaxReverseThrust = 25f; // lower speed at this rate
    public float SecondsToMaxReverseThrust = 1f;
    public float TimeStartedReversing = 0f;

    public float MaxLateralThrust = 25f;

    public float TotalForwardThrust = 0f;

    public float Speed = 0f;
    public float RotateSpeed = 1f;

    public bool InertialDampening = true; // whether or not to add friction to speed
    public float InertialFriction = 1f;

    private Rigidbody shipbody = null;    

    private Thruster engineBurn = null;
    private Thruster forwardLeftThruster = null;
    private Thruster forwardRightThruster = null;
    private Thruster backLeftThruster = null;
    private Thruster backRightThruster = null;
    private Thruster reverseThruster = null;

    // Use this for initialization
    void Start()
    {
        shipbody = this.gameObject.GetComponentInChildren<Rigidbody>();

        var thrusters = this.gameObject.GetComponentsInChildren<Thruster>();
        foreach (var thruster in thrusters)
        {
            if (thruster.name == "EngineBurn")
                engineBurn = thruster;
            else if (thruster.name == "ForwardLeftThruster")
                forwardLeftThruster = thruster;
            else if (thruster.name == "ForwardRightThruster")
                forwardRightThruster = thruster;
            else if (thruster.name == "BackLeftThruster")
                backLeftThruster = thruster;
            else if (thruster.name == "BackRightThruster")
                backRightThruster = thruster;
            else if (thruster.name == "ReverseThruster")
                reverseThruster = thruster;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        shipbody.mass = Mass;
        var lateralThrust = 0f;
        var facing = false;

        engineBurn.SetOn(false);
        forwardLeftThruster.SetOn(false);
        forwardRightThruster.SetOn(false);
        backLeftThruster.SetOn(false);
        backRightThruster.SetOn(false);
        reverseThruster.SetOn(false);

        InertialDampening = !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        if (InertialDampening)
            shipbody.drag = InertialFriction;
        else
            shipbody.drag = 0f;

        // emergency full-stop
        if (Input.GetKey(KeyCode.Space))
        {
            shipbody.drag = InertialFriction * 4f;
            Thrust = 0f;
            ReverseThrust = 0f;
            TimeStartedThrusting = 0f;
            TimeStartedReversing = 0f;

            forwardLeftThruster.SetOn(true);
            forwardRightThruster.SetOn(true);
            backLeftThruster.SetOn(true);
            backRightThruster.SetOn(true);
            reverseThruster.SetOn(true);
        }
        else // regular ship handling
        {
            // face mouse position if rmb down
            if (Input.GetMouseButton(1))
            {
                facing = true;

                // rotate towards mouse cursor
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.y = 0f;


                var direction = mouseWorldPos - this.transform.position;                
                var rotateTowards = Quaternion.LookRotation(direction);

                var rotateForce = Mathf.Min(RotateSpeed * Time.deltaTime, 1f);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotateTowards, rotateForce);

                var turning = Vector3.Cross(direction, this.transform.forward).y;

                if (turning > 2f)
                    forwardRightThruster.SetOn(true);
                if (turning < -2f)
                    forwardLeftThruster.SetOn(true);
            }
            else
                facing = false;

            if (Input.GetKey(KeyCode.W))
            {
                if (TimeStartedThrusting == 0f)
                    TimeStartedThrusting = Time.time;

                Thrust = MaxThrust * ((Time.time - TimeStartedThrusting) / SecondsToMaxThrust);
                if (Thrust > MaxThrust)
                    Thrust = MaxThrust;

                engineBurn.SetOn(true);
            }
            else
            {
                TimeStartedThrusting = 0f;
                Thrust = 0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                if (TimeStartedReversing == 0f)
                    TimeStartedReversing = Time.time;

                ReverseThrust = MaxReverseThrust * ((Time.time - TimeStartedReversing) / SecondsToMaxReverseThrust);
                if (ReverseThrust > MaxReverseThrust)
                    ReverseThrust = MaxReverseThrust;

                reverseThruster.SetOn(true);
            }
            else
            {
                TimeStartedReversing = 0f;
                ReverseThrust = 0f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                lateralThrust = -MaxLateralThrust;
                forwardRightThruster.SetOn(true);
                backRightThruster.SetOn(true);
            }
            if (Input.GetKey(KeyCode.D))
            {
                lateralThrust = MaxLateralThrust;
                forwardLeftThruster.SetOn(true);
                backLeftThruster.SetOn(true);
            }
        }
        if (Input.GetKey(KeyCode.Q) && !facing)

        {
            var rotateTowards = Quaternion.LookRotation(-this.transform.right);
            var rotateForce = Mathf.Min((RotateSpeed / 3f) * Time.deltaTime, 1f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotateTowards, rotateForce);

            forwardRightThruster.SetOn(true);
        }

        if (Input.GetKey(KeyCode.E) && !facing)
        {
            var rotateTowards = Quaternion.LookRotation(this.transform.right);
            var rotateForce = Mathf.Min((RotateSpeed / 3f) * Time.deltaTime, 1f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotateTowards, rotateForce);

            forwardLeftThruster.SetOn(true);
        }

        TotalForwardThrust = Thrust - ReverseThrust;
        if (TotalForwardThrust != 0f)
            shipbody.AddForce(this.transform.forward * TotalForwardThrust, ForceMode.Impulse);

        if (lateralThrust != 0f)
            shipbody.AddRelativeForce(lateralThrust, 0f, 0f, ForceMode.Impulse);

        Speed = shipbody.velocity.magnitude;
    }
}
