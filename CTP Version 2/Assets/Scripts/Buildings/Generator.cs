using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Generator : MonoBehaviour {

    //Road Stuff
   public GameObject roadParent;
    public Transform intersectionParent;
    public float minRoadLength;
    public float maxRoadLength;
    public float minRoadDistance;
    public float maxRoadDistance;
    public float roadScale;
    public float roadTiling;
    public Material roadMaterial;
    public Material intersectionMaterial;

    public GameObject topLeft;
    public GameObject topRight;
    public GameObject bottomLeft; 
    public GameObject bottomRight;

    //Building Stuff 
    public Transform buildingsParent;
    public float buildingLength;
    public float buildingWidth;
    public List<GameObject> buildingPrefabs = new List<GameObject>();

    static Generator instance;
	public static Generator Instance{ get { return instance; } }

    public  List<Roads> roads = new List<Roads>();
    private List<Intersection> intersections = new List<Intersection>();

    private List<Vector3> manualPoints = new List<Vector3>();

    private RoadNetwork roadNetwork;

    [HideInInspector] public bool currentlyPlacing;
    private List<Building> addedBuildings = new List<Building>();  //Adds building to new building list
    private Queue<Roads> roadsToBuild = new Queue<Roads>();

    private MeshFilter roadMeshFilter;
    private MeshFilter intersectionMeshFilter;

    private MeshRenderer roadMeshRenderer;
    private MeshRenderer intersectionMeshRenderer;

    private BoxCollider roadBoxCollider;
   // private Rigidbody roadRigidbody;

    private float roadWidth;
    private float intersectionOffset;
    private List<Roads> addedRoads = new List<Roads>();
    private List<Intersection> addedIntersections = new List<Intersection>();

    private void Awake()
	{
        instance = this;
        roadNetwork = new RoadNetwork(100f, this); //Create a New road Network
        
		ClearRoads();
    }

	private void Start()
	{
        roadWidth = 1f * roadScale;
        intersectionOffset = .5f * roadScale;   //seting up the roads

    }

	private void Update()
	{
		if(currentlyPlacing)
		{
			if(roadsToBuild.Count > 0)
			{
                Roads currentRoad = roadsToBuild.Dequeue();
                EvaluateRoad(currentRoad);   //Calls the evaluate road function
            }
			else
			{
                currentlyPlacing = false;
            }
		}
	}

	public void Clear()
	{
        roadNetwork = new RoadNetwork(100f, this);   //Clears any list that has been made 
        ClearBuildings();
        ClearRoads();
        manualPoints.Clear();
        roads.Clear();
        intersections.Clear();
       
    }

	public void AddPoint(Vector3 _point)  //Adds points then calls the draw fucntion
	{
        manualPoints.Add(_point);
        TryDraw();
    }

	private void TryDraw()
	{
		if(manualPoints.Count > 1)
		{
            roadNetwork.AddCityCentre(manualPoints);
        }

        for (int i = 0; i < roadNetwork.Roads.Count; i++)
		{
            AddRoads(roadNetwork.Roads[i]);
            //GameObject test = new GameObject("Road");
        }

        for (int i = 0; i < roadNetwork.Intersections.Count; i++)
		{
            AddIntersection(roadNetwork.Intersections[i]);
        }

        roads = new List<Roads>(roadNetwork.Roads);
        intersections = new List<Intersection>(roadNetwork.Intersections);
    }

	public void ReDraw()
	{
        roadNetwork = new RoadNetwork(100f, this);
        ClearBuildings();
        ClearRoads();

        roads.Clear();
        intersections.Clear();

        TryDraw();
        Subdivide();
    }

	public void Subdivide() //Calls Subdivide fucntions in Road Network Class
	{
        ClearBuildings();
        roadNetwork.AddCityCentre(manualPoints);
        roadNetwork.Subdivide();

        TryDraw();
    }

	public void EvaluateRoad(Roads _road)
	{
		Vector2 start = _road.start.position;
        Vector2 end = _road.end.position;
        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        Vector2 current = start;
        bool side = true;
        for (float f = roadWidth; f < distance || side; f += 1.5f)
        {
            if (f > distance && side)
            {
                side = false;
                f = roadWidth;
            }

            Vector2 per = new Vector2(-dir.y, dir.x);
            if (side)
                per *= -1;

            BuildingGen(start, dir, distance, side, f, per);
        }
    }



    private void BuildingGen(Vector2 start, Vector2 dir, float distance, bool side, float f, Vector2 per)
    {
        for (int i = 0; i < 1; i++)
        {
            Vector2 roadOffset = per.normalized * (roadWidth + Generator.Instance.buildingLength);
            Vector2 tc = start + (dir * f) + roadOffset;

            if (f - Generator.Instance.buildingWidth < 0 || f + Generator.Instance.buildingWidth > distance)
                continue;

            Vector3 center = new Vector3(tc.x, 0, tc.y);

            GameObject building = null;

            //Generates Buildings

            float seed = Random.Range(0, 100);
            float perlinVal = Mathf.PerlinNoise(center.x / 10f + seed, center.y / 10f + seed);


            bool a = false;
            if(!a)
            {
                building = Instantiate(Instance.buildingPrefabs[4], center, Quaternion.identity);
                a = true;
            }
           
            
            //if (!a)
            //{
            //    if (perlinVal < .55f)
            //    {
            //        building = Instantiate(Generator.Instance.buildingPrefabs[0], center, Quaternion.identity);
            //        a = true;
            //    }
            //    else
            //    {
            //        building = Instantiate(Generator.Instance.buildingPrefabs[1], center, Quaternion.identity);
            //        a = true;
            //    }
            //}


            //if (building.transform.position.x <= 0)
            //{
            if (perlinVal < .55f && building.transform.position.x <= 0)
            {
                building = Instantiate(Generator.Instance.buildingPrefabs[0], center, Quaternion.identity);

            }
            else if (perlinVal > .55f && building.transform.position.x >= 0)
            {
                building = Instantiate(Generator.Instance.buildingPrefabs[2], center, Quaternion.identity);

            }

            //}

            //if (building.transform.position.x >= 0)
            //{
            //    if (perlinVal < .55f)
            //    {
            //        building = Instantiate(Generator.Instance.buildingPrefabs[0], center, Quaternion.identity);

            //    }
            //    else
            //    {
            //        building = Instantiate(Generator.Instance.buildingPrefabs[1], center, Quaternion.identity);


            building.transform.parent = buildingsParent.transform;
            building.transform.RotateAround(center, Vector3.up, GetRotation(dir) - (side ? 180 : 0));

            Building buildingComp = building.AddComponent<Building>();
            buildingComp.center = center;


            if (CheckValidPlacement(buildingComp))
            {
                addedBuildings.Add(buildingComp);
                break;
            }
            else
                GameObject.DestroyImmediate(building);

        }
            
                
              
        
    }

    public void ClearBuildings()
	{
        currentlyPlacing = false;
        roadsToBuild.Clear();
		foreach(Transform building in buildingsParent)
		{
			Destroy(building.gameObject);
		}
        addedBuildings.Clear();
    }

	public void PlaceBuildings()
	{
		if(roads.Count == 0 )
		{
            return;
        }

        currentlyPlacing = true;
        for (int i = 0; i < roads.Count; i++)
		{
            roadsToBuild.Enqueue(roads[i]);
        }
    }

	 private float GetRotation(Vector3 segDir)                   //     Returns the angle in radians whose Tan is y/x.
	{
		float a1 = Mathf.Atan2 (segDir.x, segDir.y) * Mathf.Rad2Deg;  
		a1 += a1 < 0 ? 360 : 0;

		return a1;
	}

    private bool CheckValidPlacement(Building building)  //Check the interections to see if it can be placed there
	{
        if(addedBuildings.Count == 0)
        {
            return true;
        }

		foreach (Building other in addedBuildings)
			if (Vector3.Distance (building.center, other.center) > 25f)
				continue;
			else if (building.Intersects (other))
				return false;

        foreach(Intersection interse in intersections)
        {
            float smalDist = 999f;
            for (int i = 0; i < interse.Points.Count; i++)
            {
                float dist = Vector3.Distance(building.center, interse.Points[i].position);
            }

            if(smalDist < 10f)
            {
                if(IntersectsRoad(building))
                {
                    return false;
                }
            }
                
        }
		

		return true;
	}

    private bool IntersectsRoad(Building building)
	{
        //can be made more efficient, currently evaluates all roads, check distance if needed
		foreach (Roads segment in roads) {
			Ray ray = new Ray(segment.start.GetVector3(),segment.end.GetVector3() - segment.start.GetVector3());
			RaycastHit hit = new RaycastHit();
			float distance = Vector2.Distance(segment.start.position,segment.end.position);

			if(building.collider.Raycast(ray,out hit,distance))
				return true;
		}

		return false;
	}

	private void ClearRoads()
	{
        addedRoads.Clear();
        addedIntersections.Clear();

        //GameObject test = new GameObject("Road");

        //roadMeshFilter = test.AddComponent<MeshFilter>();
        

        roadMeshFilter = roadParent.gameObject.GetComponent<MeshFilter>();
        roadMeshFilter.mesh = new Mesh();
        


        roadMeshRenderer = roadParent.gameObject.GetComponent<MeshRenderer>();
       //roadMeshRenderer = test.AddComponent<MeshRenderer>();

        roadMeshRenderer.material = roadMaterial;

        intersectionMeshFilter = intersectionParent.gameObject.GetComponent<MeshFilter>();
        intersectionMeshFilter.mesh = new Mesh();

        intersectionMeshRenderer = intersectionParent.gameObject.GetComponent<MeshRenderer>();
        intersectionMeshRenderer.material = intersectionMaterial;

    }

	private void AddRoads(Roads _road)
	{
        AddRoadMesh(_road);

        addedRoads.Add(_road);
       // GameObject Road = null;
       // Road = new GameObject("Road");
       // Road.transform.parent = roadParent.transform;
       //// Road.transform.position == 


        //building.transform.RotateAround(center, Vector3.up, GetRotation(dir) - (side ? 180 : 0));

      //  Roads roadTest = Road.AddComponent<Roads>();
       // roadTest.c;
      //  Building buildingComp = building.AddComponent<Building>();
     //   buildingComp.center = center;

    }

	public void AddIntersection(Intersection inter)
	{
		List<Vector3[]> interPoints = new List<Vector3[]> ();
		foreach (Point point in inter.Points) {
			interPoints.Add(this.GetVerticeOffset(point, point.road.GetOther(point)));
		}

		if (addedIntersections.Exists (p => p.IsThisOne (inter)))
			return;

		addedIntersections.Add (inter);

		Mesh mesh = intersectionMeshFilter.sharedMesh;

        List<int> triangles = mesh.vertexCount == 0 ? new List<int>() : new List<int> (mesh.triangles);
		List<Vector3> vertices = new List<Vector3> (mesh.vertices);
		List<Vector3> normals = new List<Vector3> (mesh.normals);
		List<Vector2> uvs = new List<Vector2> (mesh.uv);

		int last = vertices.Count - 1;

		List<Vector3> interVecs = new List<Vector3> ();
		foreach(Vector3[] points in interPoints)
			interVecs.AddRange(points);

		Vector3 center = new Vector3 (interVecs.Average (p => p.x), 0, interVecs.Average (p => p.z));

		IComparer<Vector3> comparer = new CircleSort (center);

		interVecs.Sort(comparer);

		interVecs.Reverse ();
		interVecs.Add (interVecs [0]);


		for(int i=0;i < interVecs.Count - 1; i++)
		{
			Vector3 vertA = interVecs[i];
			Vector3 vertB = interVecs[i+1];
			Vector3 vertC = center;

			vertices.AddRange (new Vector3[]{vertA,vertB,vertC});
			triangles.AddRange (new int[]{ ++last, ++last, ++last});
			normals.AddRange (new Vector3[]{ Vector3.up, Vector3.up, Vector3.up});
			uvs.AddRange (new Vector2[]{ new Vector2 (0, 1), new Vector2 (1, 1), new Vector2 (0.5f, 0.5f)});
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();;
		mesh.RecalculateNormals ();
	}

	private Vector3[] GetVerticeOffset(Point main, Point other)
	{
		Vector3[] result = new Vector3[2];

		Vector3 pointA = new Vector3 (main.position.x, 0, main.position.y);
		Vector3 pointB = new Vector3 (other.position.x, 0, other.position.y);
		
		Vector3 segvec = (pointA - pointB).normalized;
		pointA -= segvec * this.intersectionOffset;
		pointB += segvec * this.intersectionOffset;
		
		Vector3 per = Vector3.Cross (pointA - pointB, Vector3.down).normalized;
		
		//create result
		result[0] = pointA + per * (0.5f * this.roadWidth);
		result[1] = pointA - per * (0.5f * this.roadWidth);

		return result;
	}

	private void AddRoadMesh(Roads segment)
	{
        Mesh mesh = roadMeshFilter.mesh;

        List<int> triangles = mesh.vertexCount == 0 ? new List<int>() : new List<int> (mesh.triangles);
		List<Vector3> vertices = new List<Vector3> (mesh.vertices);
		List<Vector3> normals = new List<Vector3> (mesh.normals);
		List<Vector2> uvs = new List<Vector2> (mesh.uv);

		int last = vertices.Count;

		Vector3 pointA = new Vector3 (segment.start.position.x, 0, segment.start.position.y);
		Vector3 pointB = new Vector3 (segment.end.position.x, 0, segment.end.position.y);

		Vector3 segvec = (pointA - pointB).normalized;
		pointA -= segvec * this.intersectionOffset;
		pointB += segvec * this.intersectionOffset;

		Vector3 per = Vector3.Cross (pointA - pointB, Vector3.down).normalized;

		Vector3 vertTL = pointA + per * (0.5f * this.roadWidth);
		Vector3 vertTR = pointA - per * (0.5f * this.roadWidth);
		Vector3 vertBL = pointB + per * (0.5f * this.roadWidth);
		Vector3 vertBR = pointB - per * (0.5f * this.roadWidth);

		vertices.AddRange (new Vector3[]{vertTL,vertTR,vertBL,vertBR});

		triangles.AddRange (new int[]{ last, last + 2, last + 1});
		triangles.AddRange (new int[]{ last + 1, last + 2, last + 3});

		normals.AddRange (new Vector3[]{ Vector3.up, Vector3.up, Vector3.up, Vector3.up});

		float length = Vector3.Distance (pointA, pointB) * roadTiling;
		uvs.AddRange (new Vector2[]{ new Vector2 (0, length), new Vector2 (1, length), new Vector2 (0, 0), new Vector2 (1, 0)});

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals ();

        AddBoxCollider();
	}

    private void AddBoxCollider()
    {
        BoxCollider boxCollider = roadBoxCollider;
        roadBoxCollider = gameObject.AddComponent<BoxCollider>();
    }
}

 

public class CircleSort : IComparer<Vector3>
{
	public Vector3 Center { get; set; }

	public CircleSort(Vector3 center)
	{
		this.Center = center;
	}

	public int Compare(Vector3 a, Vector3 b)
	{
		float a1 = Mathf.Atan2 (a.x - Center.x, a.z - Center.z) * Mathf.Rad2Deg;
		float a2 = Mathf.Atan2 (b.x - Center.x, b.z - Center.z) * Mathf.Rad2Deg;

		a1 += a1 < 0 ? 360 : 0;
		a2 += a2 < 0 ? 360 : 0;

		return a1 > a2 ? -1 : 1;
	}
}
