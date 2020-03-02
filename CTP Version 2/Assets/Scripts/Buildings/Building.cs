using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour 
{
    public Vector3 center;
    public BoxCollider collider;

    private void Awake()
	{
        collider = GetComponentInChildren<BoxCollider>();
    }

    public bool Intersects(Building building)
	{
        //  return collider != null && collider.bounds.Intersects(building.collider.bounds);
        return true;
    }
}
