﻿using UnityEngine;
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
    public bool AngularInertialDampening = true;
    public float AngularInertialFriction = 1f;

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

        engineBurn.SetFiring(false);
        forwardLeftThruster.SetFiring(false);
        forwardRightThruster.SetFiring(false);
        backLeftThruster.SetFiring(false);
        backRightThruster.SetFiring(false);
        reverseThruster.SetFiring(false);
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            InertialDampening = false;
            AngularInertialDampening = false;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            InertialDampening = false;
            AngularInertialDampening = true;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            InertialDampening = true;
            AngularInertialDampening = true;
        }

        if (InertialDampening)
            shipbody.drag = InertialFriction;
        else
            shipbody.drag = 0f;

        if (AngularInertialDampening)
            shipbody.angularDrag = AngularInertialFriction;
        else
            shipbody.angularDrag = 0f;
        
        // emergency full-stop
        if (Input.GetKey(KeyCode.Space))
        {
            shipbody.drag = InertialFriction * 4f;
            shipbody.angularDrag = AngularInertialFriction * 4f;

            Thrust = 0f;
            ReverseThrust = 0f;
            TimeStartedThrusting = 0f;
            TimeStartedReversing = 0f;

            if (Speed > 1f)
            {
                forwardLeftThruster.SetFiring(true);
                forwardRightThruster.SetFiring(true);
                backLeftThruster.SetFiring(true);
                backRightThruster.SetFiring(true);
                reverseThruster.SetFiring(true);
            }
        }
        else // regular ship handling
        { 
            if (Input.GetKey(KeyCode.W))
            {
                if (TimeStartedThrusting == 0f)
                    TimeStartedThrusting = Time.time;

                Thrust = MaxThrust * ((Time.time - TimeStartedThrusting) / SecondsToMaxThrust);
                if (Thrust > MaxThrust)
                    Thrust = MaxThrust;

                engineBurn.SetFiring(true);
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

                reverseThruster.SetFiring(true);
            }
            else
            {
                TimeStartedReversing = 0f;
                ReverseThrust = 0f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                lateralThrust = -MaxLateralThrust;
                forwardRightThruster.SetFiring(true);
                backRightThruster.SetFiring(true);
            }
            if (Input.GetKey(KeyCode.E))
            {
                lateralThrust = MaxLateralThrust;
                forwardLeftThruster.SetFiring(true);
                backLeftThruster.SetFiring(true);
            }
        }        

        if (Input.GetKey(KeyCode.D))
        {
            shipbody.AddRelativeTorque(this.transform.up * RotateSpeed, ForceMode.Impulse);
            forwardLeftThruster.SetFiring(true);
        }

        if (Input.GetKey(KeyCode.A))
        {
            shipbody.AddTorque(this.transform.up * -RotateSpeed, ForceMode.Impulse);
            forwardRightThruster.SetFiring(true);
        }

        if (Input.GetKey(KeyCode.F))
        {
            var turning = Vector3.Cross(shipbody.velocity, this.transform.forward).y;
            shipbody.angularDrag = InertialFriction * 8f;

            if (Input.GetKey(KeyCode.LeftShift))
                turning = -turning;

            if (turning > 0.65f)
            {
                shipbody.AddTorque(this.transform.up * RotateSpeed * 4f, ForceMode.Impulse);
                forwardLeftThruster.SetFiring(true);
                backRightThruster.SetFiring(true);
            }
            if (turning < -0.65f)
            {
                shipbody.AddTorque(this.transform.up * -RotateSpeed * 4f, ForceMode.Impulse);                
                forwardRightThruster.SetFiring(true);
                backLeftThruster.SetFiring(true);
            }
        }

        TotalForwardThrust = Thrust - ReverseThrust;
        if (TotalForwardThrust != 0f)
            shipbody.AddForce(this.transform.forward * TotalForwardThrust, ForceMode.Impulse);

        if (lateralThrust != 0f)
            shipbody.AddRelativeForce(lateralThrust, 0f, 0f, ForceMode.Impulse);

        Speed = shipbody.velocity.magnitude;
    }
}
