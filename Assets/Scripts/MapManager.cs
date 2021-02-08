using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MapManager : MonoBehaviour
{
    // API Key Android by Firebase AIzaSyD_lCc7joX14k6-LSwAJOWayQqVTljgxb4
    // Universidad 3.3479566,-76.5333869
    // punto A 3.3478998,-76.5324315
    // punto B 3.3481168,-76.5311696
    // punto C 3.3483012,-76.5316218
    public static MapManager instance;

    GoogleMap googleMap;
    public Image spriteMapRenderer;
    public CanvasGroup canvasARMap;
    public Canvas canvasGeoMap;
    public GameObject mainCamera, mapCamera, arCamera, scriptStreet, scriptBuilding, plane;
    public GameObject buttonChangeMapNeos;
    float latitude = 0, longitude = 0;

    void Awake()
    {
        instance= this;
    }
    
    void Start()
    {
        googleMap = GameObject.FindObjectOfType<GoogleMap>();        
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

        if(AuthManager.instance.GetUserType()=="student"){
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
        /*Input.location.Start();
        googleMap.markers[0].locations[0].latitude = Input.location.lastData.latitude;
        googleMap.markers[0].locations[0].longitude = Input.location.lastData.longitude;;
        Input.location.Stop();*/

        //punto A
        Input.location.Start();
        googleMap.markers[0].locations[0].latitude = 3.3478998f;
        googleMap.markers[0].locations[0].longitude = -76.5324315f;
        Input.location.Stop();

    }

    async void OthersCurrentLocation()
    {
        List<Coords> coordsList = await LocationsManager.instance.GetLocationAsync(AuthManager.instance.GetUserType());
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

    /*private IEnumerator CurrentLocation()
    {
        // Maps Static API
        //URLMap = "https://maps.googleapis.com/maps/api/staticmap?center=Brooklyn+Bridge,New+York,NY&zoom=13&size=600x300&maptype=roadmap&markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyAc-1EHPTPBRlRNpL6S37-p9dfXWAJ8pp0";

        URLMap = "https://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=" + zoom + "&size=" + width + "x" + height + "&maptype=satellite&markers=color:red%7Clabel:Here%7C" + latitude + "," + longitude + "&key=" + key;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(URLMap);
        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            imageMap.texture = DownloadHandlerTexture.GetContent(www);
            //imageMap.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

        StopCoroutine("CurrentLocation");
    }*/
}
