using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        for (int z = -1000; z <= 1000; z += 100)
            Debug.DrawLine(new Vector3(-1000, -10, z), new Vector3(1000, -10, z), Color.blue, 300f);

        for (int x = -1000; x <= 1000; x += 100)
            Debug.DrawLine(new Vector3(x, -10, -1000), new Vector3(x, -10, 1000), Color.blue, 300f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
