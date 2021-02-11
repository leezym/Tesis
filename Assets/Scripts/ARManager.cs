using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour
{
    public GameObject[] markers;
    public GameObject palmas, guayacanes, lago, raulPosada;
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){}
            DetectClick();
    }

    void DetectClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(Physics.Raycast (ray, out hit))
        {
            if(hit.transform.tag == "Marker")
            {
                MapManager.Instance.arCamera.SetActive(true);
                MapManager.Instance.mapCamera.SetActive(false);
                MapManager.Instance.canvasARMap.alpha = 0; 
                MapManager.Instance.plane.SetActive(false);
                foreach(GameObject marker in markers)
                    marker.SetActive(false);
            }

            if(hit.transform.name == "PalmasMarker")
                palmas.SetActive(true);

            if(hit.transform.name == "GuayacanesMarker")
                guayacanes.SetActive(true);
            
            if(hit.transform.name == "RaulPosadaMarker")
                raulPosada.SetActive(true);
            
            if(hit.transform.name == "LagoMarker")
                lago.SetActive(true);
        }
    }

    public void Exit()
    {
        MapManager.Instance.arCamera.SetActive(false);
        MapManager.Instance.mapCamera.SetActive(true);
        MapManager.Instance.canvasARMap.alpha = 1; 
        MapManager.Instance.plane.SetActive(true);
        foreach(GameObject marker in markers)
            marker.SetActive(true);
        
        palmas.SetActive(false);
        guayacanes.SetActive(false);    
        raulPosada.SetActive(false);    
        lago.SetActive(false);
    }
}
