using UnityEngine;
using System.Collections;

public class ShipOrthoFollowCamera : MonoBehaviour
{
    public Camera ActiveCamera = null;
    public Transform Target = null;
    public float FollowHeight = 200f;
    public float OrthoSize = 150f;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveCamera == null || Target == null)
            return;

        ActiveCamera.transform.position = Target.transform.position;
        ActiveCamera.transform.position += new Vector3(0f, FollowHeight);
        ActiveCamera.orthographicSize = Target.gameObject.GetComponent<Ship>().DesiredCameraOrthoSize;
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
