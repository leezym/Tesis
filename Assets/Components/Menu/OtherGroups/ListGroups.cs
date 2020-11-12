using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class ListGroups : MonoBehaviour
{
    public Canvas canvasOtherGroups;
    public GameObject infoGroup, content;

    void Update()
    {
        if(canvasOtherGroups.enabled)
        {
            ShowGroupsData();
        }
    }

    async void ShowGroupsData()
    {
        Dictionary<string, string> data = await GroupManager.instance.GetOtherGroupsDataAsync();
        foreach(KeyValuePair<string, string> pair in data)
        {
            GameObject instantiate = Instantiate(infoGroup, content.transform);
            Text nameInductor = instantiate.transform.Find("TextGroup").GetComponent<Text>();
            Text nameRoom = instantiate.transform.Find("TextInductor").GetComponent<Text>();
            
            if(pair.Key == "nameInductor")
                nameInductor.text = pair.Value;
            else if (pair.Key == "nameRoom")
                nameRoom.text = pair.Value;
        }
    }
}