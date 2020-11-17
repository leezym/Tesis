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
    public Text content;
    int newSizeGroups = 0, currentSizeGroups = 0;

    void Update()
    {
        if(canvasOtherGroups.enabled)
        {
            ShowGroupsData();
        }
    }

    async void ShowGroupsData()
    {
        List<Dictionary<string, string>> groups = await GroupManager.instance.GetOtherGroupsDataAsync();
        /*if (groups.Count != 0)
        {*/
            content.text = "";
            foreach(Dictionary<string, string> data in groups)
            {
                string nameInductor = "", nameRoom = "";
                foreach(KeyValuePair<string, string> pair in data)
                {
                    if(pair.Key == "nameInductor")
                        nameInductor = pair.Value;
                    else if (pair.Key == "nameRoom")
                        nameRoom = pair.Value;
                }
                content.text += nameRoom.ToUpper() + "\n" + nameInductor + "\n\n";
            }
        //}
    }
}