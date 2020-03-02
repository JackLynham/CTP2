using UnityEngine;

public class Point:MonoBehaviour
{
    public Vector2 position;
    public Roads road;

    public Point(){	}

	public Point(Vector2 _position, Roads _road = null)
	{
		position = new Vector2(_position.x, _position.y);
		road = _road;
	}

	public Vector3 GetVector3()
	{
		return new Vector3 (position.x, 0,position.y);  //Returns the Postions of the points of the roads
	}

	public override bool Equals(object other) 
	{
        return (Vector2.Distance((other as Point).position, position) < 0.01f);
    }
}
