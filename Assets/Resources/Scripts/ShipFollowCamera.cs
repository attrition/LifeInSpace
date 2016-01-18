﻿using UnityEngine;
using System.Collections;

public class ShipFollowCamera : MonoBehaviour
{
    public Camera ActiveCamera = null;
    public Transform Ship = null;
    public float FollowHeight = 100f;
    public float OrthoSizeAtIdle = 50f;
    public float OrthoSizeAtMax = 200f;
    public float OrthoLerpTime = 2f;

    //private Ship following;

    // Use this for initialization
    void Start()
    {
        //following = Ship.gameObject.GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveCamera == null || Ship == null)
            return;

        ActiveCamera.transform.position = Ship.transform.position;
        ActiveCamera.transform.position += new Vector3(0f, FollowHeight);
        
    }


    private float RemapRange(float remapValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        var oldRange = oldMax - oldMin;
        if (oldRange < float.Epsilon)
            return newMin;

        var newRange = newMax - newMin;
        return (((remapValue - oldMin) * newRange) / oldRange) + newMin;
    }
}