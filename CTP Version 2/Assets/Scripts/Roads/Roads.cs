using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roads:MonoBehaviour
{
    public Point start;             //Gets Start points and end points and returns te values
    public Point end;

    public Roads(Point a, Point b)
	{
		start = new Point (a.position, this);
		end = new Point (b.position, this);
	}

	public Point GetOther(Point main)       //Returns Opposite End of the road 
	{
		return start.Equals(main) ? end : start;
	}

    public float Length()
	{
		return Vector2.Distance (start.position, end.position);
	}

	public override bool Equals(object other) 
	{
        Roads otherRoad = other as Roads;
		return start.Equals(otherRoad.start) && end.Equals(otherRoad.end) || 
		start.Equals(otherRoad.end) && end.Equals(otherRoad.start);

	}
}

