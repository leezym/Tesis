using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class GroupManager : MonoBehaviour
{
    public static GroupManager instance;

    void Awake ()
    {
        instance = this;
    }

    public async Task<List<string>> ListNameStudents(string idInductor)
    {
        return await DataBaseManager.instance.ListNameStudents("Students", idInductor);
    }

    public async Task<int> SearchCurrentSizeRoom(string idInductor)
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

    public async Task<Dictionary<string,object>> GetRoomDataAsync()
    {
        string roomId = await RoomsManager.instance.SearchRoomByInductor("Rooms", AuthManager.instance.GetUserId());
        return await RoomsManager.instance.GetRoomAsync("Rooms", roomId);
    }

    public async Task<Dictionary<string,object>> GetInductorDataAsync(Text inductorNameLabel)
    {
        return await UsersManager.instance.GetUserAsync("Inductors", AuthManager.instance.GetUserId());        
    }
}