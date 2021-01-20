using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Google.Maps.Examples.Shared;

[System.Serializable]
public class GeoMapManager : MonoBehaviour
{
    public CanvasGroup canvasGeoMap;
    public GameObject mainCamera, mapCamera, scriptStreet, scriptBuilding;
    float latitude = 0, longitude = 0;

    void Update() 
    {
        if(canvasGeoMap.alpha == 1)
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

    private IEnumerator CurrentLocation()
    {
        Input.location.Start();
        latitude = Input.location.lastData.latitude;
        yield return latitude;
        longitude = Input.location.lastData.longitude;
        yield return longitude;
        Input.location.Stop();

        ///GameObject.FindObjectOfType<DynamicMapsService>().SetLatLng(latitude, longitude);
        StopCoroutine("CurrentLocation");
    }
    
    /*private string URLMap = "";
    public RawImage imageMap;
    //public Text latitudeText, longitudeText;
    public int zoom = 20, width, height;
    public string key = "AIzaSyD_lCc7joX14k6-LSwAJOWayQqVTljgxb4";

    private void Start()
    {
        StartCoroutine("CurrentLocation");
    }

    public void Refresh()
    {
        StartCoroutine("CurrentLocation");
    }

    public void ZoomIn()
    {
        zoom++;
        StartCoroutine("CurrentLocation");
    }

    public void ZoomOut()
    {
        if (zoom >= 0)
            zoom--;
        StartCoroutine("CurrentLocation");
    }

    private IEnumerator CurrentLocation()
    {
        Input.location.Start();
        latitude = Input.location.lastData.latitude;
        yield return latitude;
        longitude = Input.location.lastData.longitude;
        yield return longitude;
        Input.location.Stop();
        
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
    }

     private IEnumerator CurrentLocation()
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
