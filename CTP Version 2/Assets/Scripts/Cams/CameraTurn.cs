using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurn : MonoBehaviour {

    public float speed;

    private bool down;
    private bool up;

    void Update () 
    {
        transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * speed);


        if(up== true)
        {
            transform.Translate(Vector3.up *100 * Time.deltaTime, Space.World);
        }

        if(down == true)
        {
           transform.Translate(Vector3.down * 100 *Time.deltaTime, Space.World);
        }
      
        if (Input.GetKeyDown("w"))
        {
            up = true;
        }
        if (Input.GetKeyUp("w"))
        {
            up = false;
        }

        if (Input.GetKeyDown("s"))
        {
            down = true;
        }

        if (Input.GetKeyUp("s"))
        {
            down = false;
        }
    }
}
