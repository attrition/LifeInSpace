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
    
    public void SetFiring(bool status)
    {
        On = status;
        this.gameObject.SetActive(On);
    }
}
