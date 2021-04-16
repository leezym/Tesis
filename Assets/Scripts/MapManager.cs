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
    public GameObject buttonChangeMapNeos, backButtonInductor, backButtonStudent;
    float currentLatitude = 0, currentLongitude = 0;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        googleMap = GameObject.FindObjectOfType<GoogleMap>();  
    }

    public void CheckLocation(){
        StartCoroutine(CheckLocationOn());
    }

    public IEnumerator CheckLocationOn()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser){
            NotificationsManager.Instance.SetFailureNotificationMessage("Por favor otorga permisos a la aplicación para acceder a tu ubicación y activa la ubicación de tu dispositivo.\n Cierra sesión y vuelve a intentar.");
            yield break;
        }
        else
        {
            StartCoroutine(Location());
        }
    }
    public IEnumerator Location()
    {
        // Start service before querying location
        Input.location.Start();
        
        //NotificationsManager.Instance.SetSuccessNotificationMessage("GPS enabled");
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else if (Input.location.status == LocationServiceStatus.Running)
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
            googleMap.centerLocation.latitude = Input.location.lastData.latitude;
            googleMap.centerLocation.longitude = Input.location.lastData.longitude;

            GoogleMapLocation loc = new GoogleMapLocation();
            loc.address = "";
            loc.latitude = googleMap.centerLocation.latitude;
            loc.longitude = googleMap.centerLocation.longitude;
            googleMap.markers[0].locations[0] = loc;
        }
        
        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

        StopCoroutine(Location());
    }

    void Update() 
    {
        SetMap();
        
        if(GlobalDataManager.Instance.userType == "student"){
            SetChangeMapButtonActive();
        }else{
            SetChangeMapButtonNotActive();
        }
    }

    void SetMap()
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
    }

    public async void PutLocation()
    {
        if (GlobalDataManager.Instance.userType == "student")
        {
            // Mapa
            if (GlobalDataManager.Instance.idUserStudent != "")
            {
                await UsersManager.Instance.PutUserAsync("Students", GlobalDataManager.Instance.idUserStudent, new Dictionary<string, object>{
                    {"latitude", currentLatitude},
                    {"longitude", currentLongitude}
                });
            }
        }
        else if(GlobalDataManager.Instance.userType == "inductor")
        { 
            // Mapa
            if(GlobalDataManager.Instance.idUserInductor != "")
            {
                await UsersManager.Instance.PutUserAsync("Inductors", GlobalDataManager.Instance.idUserInductor, new Dictionary<string, object>{
                    {"latitude", currentLatitude},
                    {"longitude", currentLongitude}
                });
            }
        }
    }

    public void Refresh()
    {
        CheckLocation();
        OthersCurrentLocation();
        StartCoroutine(googleMap._Refresh());
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

    async void OthersCurrentLocation()
    {
        List<Coords> coordsList = await LocationsManager.Instance.GetLocationAsync(GlobalDataManager.Instance.userType);
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
    }

    void SetChangeMapButtonActive()
    {
        buttonChangeMapNeos.SetActive(true);
        backButtonInductor.SetActive(false);
        backButtonStudent.SetActive(true);
    }

    void SetChangeMapButtonNotActive()
    {
        buttonChangeMapNeos.SetActive(false);
        backButtonInductor.SetActive(true);
        backButtonStudent.SetActive(false);
    }
}
