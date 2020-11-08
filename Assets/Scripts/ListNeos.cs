using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;


public class ListNeos : MonoBehaviour
{
    public Text content;
    private string idInductor;
    public List<string> NeoJaverianos = new List<string>();
    int currentSizeStudents = 0, newSizeStudents = 0;
    public Font fontStudent;

    void Start()
    {
    }

    void Update()
    {
        if (GetComponent<Canvas>().enabled)
            DetectStudent();
    }

    async void DetectStudent()
    {     
        if (idInductor == null){
            idInductor = AuthManager.instance.GetUserId();
            currentSizeStudents = await SearchCurrentSizeRoom();
        }

        NeoJaverianos = await DataBaseManager.instance.ListNameStudents("Students", idInductor);
        newSizeStudents = NeoJaverianos.Count;

        if (currentSizeStudents != newSizeStudents)
        {
            content.text = "";
            foreach(string name in NeoJaverianos)
            {
                content.text += name + "\n";
            }
            currentSizeStudents = newSizeStudents;
        }
    }

    async Task<int> SearchCurrentSizeRoom()
    {
        string idRoom = await RoomsManager.instance.SearchRoomByInductor("Rooms", idInductor);
        Dictionary<string, object> data = await DataBaseManager.instance.SearchById("Rooms", idRoom);
        foreach (KeyValuePair<string, object> pair in data)
        {
            if (pair.Key == "currentSize")
                return Convert.ToInt32(pair.Value);
        }
        return 0;
    }
}
