using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{ 


    private void OnTriggerEnter(Collider other)
    {
          Building building = GetComponent<Building>();
        if (other.tag == "Player")
        {
     
            Destroy(building.collider);
            Destroy(gameObject);
        }
    }
}
