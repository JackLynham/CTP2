using UnityEngine;
using System.Collections.Generic;

public class RoadNetwork 
{
    public List<Road> Roads;
    public List<Intersection> Intersections;

    public float scale;

    public Generator generator;

    public RoadNetwork(float scale, Generator _generator)
	{
		Roads = new List<Road> ();
		Intersections = new List<Intersection> ();

		scale = scale;
        generator = _generator;
    }

	public void Subdivide()
	{
		List<Road> temporaryRoads = new List<Road> (Roads);

		for(int i = 0; i < temporaryRoads.Count; i++){
			
			SplitRoad (temporaryRoads[i]);
		
		}
	}

	public void AddCityCentre(List<Vector3> _intersections)
	{ 
        Point a = new Point();
        Point b = new Point();
        for (int i = 0; i < _intersections.Count; i++)
		{
			if(i % 2 == 0 || i == 0)
			{
                
                a.position.x = _intersections[i].x;         //Draws new road for every point made
                a.position.y = _intersections[i].z;
            } 
			else
			{
                b.position.x = _intersections[i].x;
				b.position.y = _intersections[i].z;
                Road rb = new Road(a, b);
                Roads.Add(rb);
                Intersection ab = new Intersection(new List<Point>() { rb.start });
               	Intersections.Add(ab);

            }
        }

    }

	private void SplitRoad(Road _road)
	{
		float splitPercentile = Random.Range (0.1f, 0.95f);  //Gets a random split percentage

        Vector3 startPosition = _road.start.GetVector3();
		Vector3 endPosition = _road.end.GetVector3();
		float length = Vector3.Distance (startPosition, endPosition);
		length *= splitPercentile;

		Vector3 direction = (startPosition - endPosition).normalized;
		Vector3 splitPosition = endPosition + (direction * length);

		Vector3 perpendicularA = Vector3.Cross (startPosition - endPosition, Vector3.down).normalized;
		float newLength =  Random.Range(generator.minRoadLength,generator.maxRoadLength);
		Vector3 splitPositionEnd = splitPosition + (perpendicularA * newLength);

		Road roadA = new Road (new Point(new Vector2 (splitPosition.x, splitPosition.z),null), 
		new Point(new Vector2 (splitPositionEnd.x, splitPositionEnd.z),null));
  	
		Vector3 perpendicularB = Vector3.Cross (startPosition - endPosition, Vector3.down).normalized * -1;
		Vector3 splitPositionEndOther = splitPosition + (perpendicularB * newLength);
		Road roadB = new Road (new Point (new Vector2 (splitPosition.x, splitPosition.z), null), 
		new Point (new Vector2 (splitPositionEndOther.x, splitPositionEndOther.z), null));

		bool validRoadA = false;
		bool validRoadB = false;

		if (!RoadIntersecting (roadA, generator.maxRoadDistance)) 
		{
			Vector2 intersection = Vector3.zero;
			Road other = null;

			int iCount = IntersectionCount(roadA,out intersection,out other,_road);

			if(iCount <= 1)
			{
				this.Roads.RemoveAll(p => p.Equals(roadA));
				this.Roads.Add (roadA);
				validRoadA = true;
			}

			if(iCount == 1)
			{
				Road[] segmentsA = Patch (other, new Point (intersection, other));
				Road[] segmentsB = Patch (roadA, new Point (intersection, roadA));
				
				List<Point> points = new List<Point>();
				if(segmentsA[0].Length() > generator.minRoadDistance)
					points.Add(segmentsA [0].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsA[0]));
				
				if(segmentsA[1].Length() > generator.minRoadDistance)
					points.Add(segmentsA [1].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsA[1]));
				
				if(segmentsB[0].Length() > generator.minRoadDistance)
					points.Add(segmentsB [0].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsB[0]));
				
				if(segmentsB[1].Length() > generator.minRoadDistance)
					points.Add(segmentsB [1].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsB[1]));
				
				Intersection inter = new Intersection (points);
				this.Intersections.Add (inter);
			}
		}

		if (!RoadIntersecting (roadB, generator.maxRoadDistance)) 
		{
			Vector2 intersection = Vector3.zero;
			Road other = null;

			int iCount = IntersectionCount(roadB,out intersection,out other,_road);

			if(iCount <= 1)
			{
				this.Roads.RemoveAll(p => p.Equals(roadB));
				this.Roads.Add (roadB);
				validRoadB = true;
			}

			if(iCount == 1)
			{
				Road[] segmentsA = Patch (other, new Point (intersection, other));
				Road[] segmentsB = Patch (roadB, new Point (intersection, roadB));

				List<Point> points = new List<Point>();
				if(segmentsA[0].Length() > generator.minRoadDistance)
					points.Add(segmentsA [0].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsA[0]));

				if(segmentsA[1].Length() > generator.minRoadDistance)
					points.Add(segmentsA [1].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsA[1]));

				if(segmentsB[0].Length() > generator.minRoadDistance)
					points.Add(segmentsB [0].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsB[0]));

				if(segmentsB[1].Length() > generator.minRoadDistance)
					points.Add(segmentsB [1].end);
				else
					this.Roads.RemoveAll(p => p.Equals(segmentsB[1]));

				Intersection inter = new Intersection (points);
				this.Intersections.Add (inter);
			}
		}

		if (validRoadA || validRoadB) {
			Road[] segments = Patch (_road, new Point (new Vector2 (splitPosition.x, splitPosition.z), _road));

			if (validRoadA && validRoadB) {
				Intersection inter = new Intersection (new List<Point>{segments [0].end,segments [1].end,roadA.start,roadB.start});
				this.Intersections.Add (inter);
			} else if (validRoadA) {
				Intersection inter = new Intersection (new List<Point>{segments [0].end,segments [1].end,roadA.start});
				this.Intersections.Add (inter);
			} else if (validRoadB) {
				Intersection inter = new Intersection (new List<Point>{segments [0].end,segments [1].end,roadB.start});
				this.Intersections.Add (inter);
			}
		}
	}

	private float DistancePointRoad( Point P, Road S)
	{
		Vector2 v = S.end.position - S.start.position;
		Vector2 w = P.position - S.start.position;
		
		float c1 = Vector2.Dot(w,v);
		if ( c1 <= 0 )
			return Vector2.Distance(P.position, S.start.position);
		
		float c2 = Vector2.Dot(v,v);
		if ( c2 <= c1 )
			return Vector2.Distance(P.position, S.end.position);
		
		float b = c1 / c2;
		Vector2 Pb = S.start.position + (v * b);
		return Vector2.Distance(P.position, Pb);
	}

	private int IntersectionCount(Road segment, out Vector2 intersection, out Road other, Road skip)
	{
		intersection = Vector2.zero;
		other = null;

		Vector2 tmp = Vector2.zero;
		Vector2 interTmp = Vector3.zero;

		int count = 0;
		
		for (int i=0; i<this.Roads.Count; i++) {
			Road seg = this.Roads[i];
			if (seg.Equals(skip))
				continue;
			else if (Vector2.Distance (seg.start.position, segment.start.position) < 0.01f || Vector2.Distance (seg.end.position, segment.end.position) < 0.01f)
				continue;
			else if (Vector2.Distance (seg.start.position, segment.end.position) < 0.01f || Vector2.Distance (seg.end.position, segment.start.position) < 0.01f)
				continue;
			else if (TwoDimentsionalIntersection (segment, seg, out interTmp, out tmp) != 0) {
				other = seg;
				intersection = new Vector2(interTmp.x,interTmp.y);
				count++;
			}
		}

		return count;
	}

	private float perp (Vector2 u, Vector2 v)
	{
		return ((u).x * (v).y - (u).y * (v).x);
	}

	int TwoDimentsionalIntersection(Road S1, Road S2, out Vector2 I0, out Vector2 I1)
	{
		Vector2 u = S1.end.position - S1.start.position;
		Vector2 v = S2.end.position - S2.start.position;
		Vector2 w = S1.start.position - S2.start.position;
		float D = perp(u,v);

		I0 = Vector2.zero;
		I1 = Vector2.zero;

		if (Mathf.Abs(D) < 0.01f) {           
			if (perp(u,w) != 0 || perp(v,w) != 0)  {
				return 0;                    
			}
			
			float du = Vector2.Dot(u,u);
			float dv = Vector2.Dot(v,v);
			if (du==0 && dv==0) {           
				if (S1.start.position !=  S2.start.position)        
					return 0;
				I0 = S1.start.position;                 
				return 1;
			}
			if (du==0) {                     
				if  (InRoad(S1.start, S2) == 0)  
					return 0;
				I0 = S1.start.position;
				return 1;
			}
			if (dv==0) {                    
				if  (InRoad(S2.start, S1) == 0) 
					return 0;
				I0 = S2.start.position;
				return 1;
			}
			
			float t0, t1;                    
			Vector2 w2 = S1.end.position - S2.start.position;
			if (v.x != 0) {
				t0 = w.x / v.x;
				t1 = w2.x / v.x;
			}
			else {
				t0 = w.y / v.y;
				t1 = w2.y / v.y;
			}
			if (t0 > t1) {                   
				float t=t0; t0=t1; t1=t;    
			}
			if (t0 > 1 || t1 < 0) {
				return 0;      
			}
			t0 = t0<0? 0 : t0;               
			t1 = t1>1? 1 : t1;               
			if (t0 == t1) {                  
				I0 = S2.start.position +  t0 * v;
				return 1;
			}
			
			
			I0 = S2.start.position + t0 * v;
			I1 = S2.start.position + t1 * v;
			return 2;
		}
		
		float     sI = perp(v,w) / D;
		if (sI < 0 || sI > 1)                
			return 0;
		
		
		float     tI = perp(u,w) / D;
		if (tI < 0 || tI > 1)               
			return 0;
		
		I0 = S1.start.position + sI * u;                
		return 1;
	}

	
	int InRoad( Point P, Road S)
	{
		if (S.start.position.x != S.end.position.x) {   
			if (S.start.position.x <= P.position.x && P.position.x <= S.end.position.x)
				return 1;
			if (S.start.position.x >= P.position.x && P.position.x >= S.end.position.x)
				return 1;
		}
		else {    // S is vertical, so test y  coordinate
			if (S.start.position.y <= P.position.y && P.position.y <= S.end.position.y)
				return 1;
			if (S.start.position.y >= P.position.y && P.position.y >= S.end.position.y)
				return 1;
		}
		return 0;
	}

	private bool RoadIntersecting(Road segment, float max)
	{
		foreach (Road seg in this.Roads) {
			bool amax = DistancePointRoad (seg.start, segment) < max;
			bool bmax = DistancePointRoad (seg.end, segment) < max;

			bool amin = MinPointDistance(seg,segment,max / 1.0f);

			if(amax || bmax || amin)
				return true;
		}

		return false;
	}

	private bool MinPointDistance(Road a, Road b, float min)
	{
		if (Vector2.Distance (a.start.position, b.start.position) < min)
			return true;
		if (Vector2.Distance (a.start.position, b.end.position) < min)
			return true;
		if (Vector2.Distance (a.end.position, b.start.position) < min)
			return true;
		if (Vector2.Distance (a.end.position, b.end.position) < min)
			return true;

		return false;
	}

	private bool PointWithin(Point point, float distance)
	{
		foreach (Road segment in this.Roads)
			if (Vector2.Distance (point.position, segment.start.position) < distance)
				return true;
			else if (Vector2.Distance (point.position, segment.end.position) < distance)
				return true;

		return false;
	}

	private Road[] Patch(Road segment, Point splitPosition)
	{
		this.Roads.RemoveAll(p => p.Equals(segment));

		Road left = new Road (segment.start, new Point(splitPosition.position));
		Road right = new Road (segment.end, new Point(splitPosition.position));

		this.Roads.Add (left);
		this.Roads.Add (right);

		return new Road[] {left,right};
	}
}
