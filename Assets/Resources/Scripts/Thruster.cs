using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour
{
    public bool Firing;

    // Use this for initialization
    void Start()
    {
        Firing = false;
    }
    
    public void SetFiring(bool status)
    {
        Firing = status;
        this.gameObject.SetActive(Firing);
    }
}
