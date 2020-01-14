using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Intersection
{
	public List<Point> Points;

	public Intersection(List<Point> points)
	{
		Points = points;
	}

	public bool IsThisOne(Intersection inter)
	{
		int c = 0;
		foreach (Point p in inter.Points)
			if (this.Points.Exists (f => f == p))
				c++;

		if (c == this.Points.Count && c == inter.Points.Count)
			return true;

		return false;
	}
}
