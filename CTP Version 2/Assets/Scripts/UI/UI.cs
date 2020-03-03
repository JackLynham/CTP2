using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public Camera cam;
    public Generator generator;

    public Button clearButton;
    public Button buildingButton;
    public Button subdivideButton;

    public Slider minStreetLength;
    public Slider maxStreetLength;

    public Slider minRoadDistance;
    public Slider maxRoadDistance;

    public Toggle rotate;
    
    void Start () {
        clearButton.onClick.RemoveAllListeners();
        clearButton.onClick.AddListener(() =>
        {
            generator.Clear();
        });

        buildingButton.onClick.RemoveAllListeners();
        buildingButton.onClick.AddListener(() =>
        {
            if(!generator.currentlyPlacing)
            {
                generator.PlaceBuildings();
            }
        });

        subdivideButton.onClick.RemoveAllListeners();
        subdivideButton.onClick.AddListener(() =>
        {
            generator.Subdivide();
        });

        rotate.onValueChanged.AddListener(delegate { ChangeRotation(); });

        minStreetLength.minValue = .1f;
        minStreetLength.maxValue = 25f;
        minStreetLength.value = generator.minRoadLength;
        minStreetLength.onValueChanged.AddListener(delegate { MinValChange(); });

        maxStreetLength.minValue = 25f;
        maxStreetLength.maxValue = 125f;
        maxStreetLength.value = generator.maxRoadLength;
        maxStreetLength.onValueChanged.AddListener(delegate { MaxValChange(); });

        minRoadDistance.minValue = .5f;
        minRoadDistance.maxValue = 10f;
        minRoadDistance.value = generator.minRoadDistance;
        minRoadDistance.onValueChanged.AddListener(delegate { MinDistanceValChange(); });

        maxRoadDistance.minValue = 4.5f;
        maxRoadDistance.maxValue = 25f;
        maxRoadDistance.value = generator.maxRoadDistance;
        maxRoadDistance.onValueChanged.AddListener(delegate { MaxDistanceValChange(); });
   
	}

    private void MinValChange()
    {
        generator.minRoadLength = minStreetLength.value;
        generator.ReDraw();
    }

    private void MaxValChange()
    {
        generator.maxRoadLength = maxStreetLength.value;
        generator.ReDraw();
    }

    private void MinDistanceValChange()
    {
        generator.minRoadDistance = minRoadDistance.value;
        generator.ReDraw();
    }

    private void MaxDistanceValChange()
    {
        generator.maxRoadDistance = maxRoadDistance.value;
        generator.ReDraw();
    }

    private void ChangeRotation()
    {
        cam.GetComponent<CameraTurn>().speed = (rotate.isOn) ? 20f : 0f;
    }


    void Update()
    { //Draw Roads
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePos = Vector3.zero;
        if (mouseRay.direction.y != 0)
        {
            float dstToXZPlane = Mathf.Abs(mouseRay.origin.y / mouseRay.direction.y);
            mousePos = mouseRay.GetPoint(dstToXZPlane);
        }
        //When you Click saves to a list
        if (Input.GetMouseButtonDown(1))
        {
            generator.AddPoint(mousePos);
           
        }
    }
}
