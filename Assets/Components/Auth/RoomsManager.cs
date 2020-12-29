using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Database;

public class RoomsManager : MonoBehaviour
{
    public static RoomsManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void PostNewRoom(string room, int size, string idInductor)
    {
        Room element = new Room(room, size, idInductor);
        DataBaseManager.instance.InsertWithoutId("Rooms", element.ConvertJson());
    }

    public async Task PutRoomAsync(string roomId, Dictionary<string,object> data)
    {
        await DataBaseManager.instance.UpdateAsync("Rooms", roomId, data);
    }

    public async Task<Dictionary<string, object>> GetRoomAsync(string roomId)
    {
        return await DataBaseManager.instance.SearchById("Rooms", roomId);
    }

    public async Task<string> SearchAvailableRoom() 
    {

        return await DataBaseManager.instance.SearchAvailableRoom("Rooms");
    }

    public async Task<string> SearchRoomByInductor(string idInductor)
    {
        return await DataBaseManager.instance.SearchRoomByInductor("Rooms", idInductor);

    }

    public async Task DeleteStudentInRoom(string idStudent)
    {
        await DataBaseManager.instance.DeleteStudentInRoom(idStudent);
    }
}

public class Room
{
    public string room;
    public int size;
    public int currentSize;
    public string idInductor;

    public Room() { }

    public Room(string room, int size, string idInductor)
    {
        this.room = room;
        this.size = size;
        this.currentSize = 0;
        this.idInductor = idInductor;
    }

    public int GetSize() { return this.size; }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "room", this.room },
            { "size", this.size },
            { "currentSize", this.currentSize },
            { "idInductor", this.idInductor }
        };
    }
}
