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
    public Transform groupsContent;
    public GameObject otherGroupPrefab;
    List<GameObject> currentGroups = new List<GameObject>();
    int newSizeGroups = 0, currentSizeGroups = 0;

    void Update()
    {
        if(canvasOtherGroups.enabled)
        {
            ShowGroupsData();
        }
    }

    void ClearCurrentGroups()
    {
        // Vaciar lista y borrar grupos actuales
        foreach(GameObject group in currentGroups)
        {
            Destroy(group);
        }
        currentGroups.Clear();
    }

    async void ShowGroupsData()
    {
        List<Dictionary<string, string>> groupsList = await GroupManager.Instance.GetOtherGroupsDataAsync();
        newSizeGroups = groupsList.Count;

        if (currentSizeGroups != newSizeGroups)
        {
            ClearCurrentGroups();
            foreach(Dictionary<string,string> group in groupsList)
            {
                // Instanciar prefab
                GameObject groupElement = Instantiate (otherGroupPrefab, new Vector3(groupsContent.position.x,groupsContent.position.y, groupsContent.position.z) , Quaternion.identity);
                groupElement.transform.parent = groupsContent.transform;

                foreach(KeyValuePair<string,string> pair in group)
                {
                    if(pair.Key == "nameRoom"){
                        Text nameGrouptText = groupElement.transform.Find("GroupNameLabel").GetComponent<Text>();
                        nameGrouptText.text = pair.Value;
                    }
                    if(pair.Key == "nameInductor")
                    {
                        Text nameInductortText = groupElement.transform.Find("InductorNameLabel").GetComponent<Text>();
                        nameInductortText.text = pair.Value;
                    }
                }
                // Añadir a Lista
                currentGroups.Add(groupElement);
            }
            currentSizeGroups = newSizeGroups;
        }
    }
}