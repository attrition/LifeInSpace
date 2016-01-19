using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public List<Ship> Ships;
    public int CurrentShip = -1;
    public ShipOrthoFollowCamera orthoFollowCam;

    // Use this for initialization
    void Start()
    {
        for (int z = -10000; z <= 10000; z += 100)
            Debug.DrawLine(new Vector3(-10000, -10, z), new Vector3(10000, -10, z), Color.blue, 300f);

        for (int x = -10000; x <= 10000; x += 100)
            Debug.DrawLine(new Vector3(x, -10, -10000), new Vector3(x, -10, 10000), Color.blue, 300f);

        Ships = new List<Ship>(this.gameObject.GetComponentsInChildren<Ship>());
        if (Ships.Count > 0)
            CurrentShip = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Ships.Count > 0)
            {
                Ships[CurrentShip].playerControlled = false;
                CurrentShip++;
                if (CurrentShip >= Ships.Count)
                    CurrentShip = 0;
                Ships[CurrentShip].playerControlled = true;
                orthoFollowCam.Target = Ships[CurrentShip].transform;
            }
        }
    }
}
