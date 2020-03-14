using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Collider colliderItem;
    public bool A = false;
    
    private void OnTriggerEnter(Collider other)
    {

        //Collider[] collidersObj = Generator.Instance.roadParent.GetComponentsInChildren<Collider>();


        //for (int index = 0; index < Generator.Instance.roads.Count; index++)
        //{
        //    Debug.Log("FuckYour mum ");
        //    colliderItem = collidersObj[index];
        //    colliderItem.enabled = false;

        //}



    }

}
