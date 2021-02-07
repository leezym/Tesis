using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour
{
    public 
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
                MapManager.instance.arCamera.SetActive(true);
                MapManager.instance.mapCamera.SetActive(false);
                MapManager.instance.canvasARMap.alpha = 0; 
            }
        }
    }
}
