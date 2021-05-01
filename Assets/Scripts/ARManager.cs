using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour
{
    public GameObject[] markers;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
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
                foreach(GameObject marker in markers)
                {
                    marker.SetActive(false);
                }

                MapManager.Instance.arCamera.SetActive(true);
                MapManager.Instance.mapCamera.SetActive(false);

                MapManager.Instance.canvasARMap.alpha = 0; 
                MapManager.Instance.canvasARMap.interactable = false;
                MapManager.Instance.plane.SetActive(false);

                MapManager.Instance.scriptBuilding.SetActive(false);
                MapManager.Instance.scriptStreet.SetActive(false);
            }
        }
    }

    public void ExitAR()
    {
        foreach(GameObject marker in markers)
        {
            marker.SetActive(true);
        }
        
        MapManager.Instance.arCamera.SetActive(false);
        MapManager.Instance.mapCamera.SetActive(true);

        MapManager.Instance.canvasARMap.alpha = 1;
        MapManager.Instance.canvasARMap.interactable = true;
        MapManager.Instance.plane.SetActive(true);

        MapManager.Instance.scriptBuilding.SetActive(true);
        MapManager.Instance.scriptStreet.SetActive(true);
    }
}
