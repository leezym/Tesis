using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MapManager : MonoBehaviour
{
    // API Key Android by Firebase AIzaSyD_lCc7joX14k6-LSwAJOWayQqVTljgxb4
    private static MapManager instance;
    public static MapManager Instance { get => instance; set => instance = value; }

    GoogleMap googleMap;
    public Image spriteMapRenderer;
    public CanvasGroup canvasARMap;
    public Canvas canvasGeoMap;
    public GameObject mainCamera, mapCamera, arCamera, scriptStreet, scriptBuilding, plane;
    public GameObject buttonChangeMapNeos;
    float latitude = 0, longitude = 0;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        googleMap = GameObject.FindObjectOfType<GoogleMap>();  
        Input.location.Start();
        googleMap.centerLocation.latitude = Input.location.lastData.latitude;
        googleMap.centerLocation.longitude = Input.location.lastData.longitude;        
        Input.location.Stop();    
    }

    void Update() 
    {
        if(canvasARMap.alpha == 1)
        {
            mainCamera.SetActive(false);
            mapCamera.SetActive(true);
            scriptStreet.SetActive(true);
            scriptBuilding.SetActive(true);
        }
        else
        {
            mainCamera.SetActive(true);
            mapCamera.SetActive(false);
            scriptStreet.SetActive(false);
            scriptBuilding.SetActive(false);
        }

        if(AuthManager.Instance.GetUserType()=="student"){
            SetChangeMapButtonActive();
        }else{
            SetChangeMapButtonNotActive();
        }
    }

    public void Refresh()
    {
        MyCurrentLocation();
        OthersCurrentLocation();
    }


    public void ZoomIn()
    {
        googleMap.zoom++;
        Refresh();
    }

    public void ZoomOut()
    {
        if (googleMap.zoom >= 0)
            googleMap.zoom--;
        Refresh();
    }

    void MyCurrentLocation()
    {
        Input.location.Start();
        googleMap.markers[0].locations[0].latitude = Input.location.lastData.latitude;
        googleMap.markers[0].locations[0].longitude = Input.location.lastData.longitude;;
        Input.location.Stop();

        //punto A
        /*Input.location.Start();
        googleMap.markers[0].locations[0].latitude = 3.3478998f;
        googleMap.markers[0].locations[0].longitude = -76.5324315f;
        Input.location.Stop();*/

    }

    async void OthersCurrentLocation()
    {
        List<Coords> coordsList = await LocationsManager.Instance.GetLocationAsync(AuthManager.Instance.GetUserType());
        List<GoogleMapLocation> locationsList = new List<GoogleMapLocation>();
        foreach(Coords coords in coordsList)
        {
            GoogleMapLocation loc = new GoogleMapLocation();
            loc.address = "";
            loc.latitude = coords.latitude;
            loc.longitude = coords.longitude;
            locationsList.Add(loc);
        }

        googleMap.markers[1].locations = locationsList.ToArray();
        StartCoroutine(googleMap._Refresh());
    }

    void SetChangeMapButtonActive(){
        buttonChangeMapNeos.SetActive(true);
    }

    void SetChangeMapButtonNotActive(){
        buttonChangeMapNeos.SetActive(false);
    }

    public void ChangeMapFromAR(){
        canvasARMap.alpha = 0;
    }
}
