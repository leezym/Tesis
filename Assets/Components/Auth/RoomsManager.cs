﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class RoomsManager : MonoBehaviour
{
    private static RoomsManager instance;
    public static RoomsManager Instance { get => instance; set => instance = value; }

    public void Awake()
    {
        instance = this;
    }

    public async Task PostNewRoom(string room, int size, string idInductor)
    {
        Room element = new Room(room, size, idInductor);
        await DataBaseManager.Instance.InsertWithoutId("Rooms", element.ConvertJson());
    }

    public async Task PutRoomAsync(string idRoom, Dictionary<string,object> data)
    {
        await DataBaseManager.Instance.UpdateAsync("Rooms", idRoom, data);
    }

    public async Task<Dictionary<string, object>> GetRoomAsync(string idRoom)
    {
        return await DataBaseManager.Instance.SearchById("Rooms", idRoom);
    }

    public async Task<string> GetAvailableRoom() 
    {

        return await DataBaseManager.Instance.GetAvailableRoom("Rooms");
    }

    public async Task<List<Dictionary<string, object>>> GetRoomsByOrderOfScore()
    {
        return await DataBaseManager.Instance.SearchByOrderDescending("Rooms", "score");
    }

    public async Task DeleteStudentInRoom()
    {
        await DataBaseManager.Instance.DeleteStudentInRoom();
    }
}

public class Room
{
    public string room;
    public int size;
    public int score = 0;
    public bool finished = false;
    public string idInductor;

    public Room() { }

    public Room(string room, int size, string idInductor)
    {
        this.room = room;
        this.size = size;
        this.idInductor = idInductor;
    }

    public int GetSize() { return this.size; }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "room", this.room },
            { "size", this.size },
            { "score", this.score},
            { "finished", this.finished},
            { "idInductor", this.idInductor }
        };
    }
}
