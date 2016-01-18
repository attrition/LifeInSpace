using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour
{
    public bool On;

    // Use this for initialization
    void Start()
    {
        On = false;
    }

    void Update()
    {
    }

    public void SetOn(bool status)
    {
        On = status;
        this.gameObject.SetActive(On);
    }
}
