using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road:MonoBehaviour
{
    public Point start;             //Gets Start points and end points and returns te values
    public Point end;

    public Road(Point a, Point b)
	{
		start = new Point (a.position, this);
		end = new Point (b.position, this);
	}

	public Point GetOther(Point main)
	{
		return start.Equals(main) ? end : start;
	}

    public float Length()
	{
		return Vector2.Distance (start.position, end.position);
	}

	public override bool Equals(object other)
	{
        Road otherRoad = other as Road;
		return start.Equals(otherRoad.start) && end.Equals(otherRoad.end) || 
		start.Equals(otherRoad.end) && end.Equals(otherRoad.start);

	}
}

