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

    public async Task PostNewRoom(string room, int size, string idInductor)
    {
        Room element = new Room(room, size, idInductor);
        await DataBaseManager.instance.InsertWithoutId("Rooms", element.ConvertJson());
    }

    public async Task PutRoomAsync(string idRoom, Dictionary<string,object> data)
    {
        await DataBaseManager.instance.UpdateAsync("Rooms", idRoom, data);
    }

    public async Task<Dictionary<string, object>> GetRoomAsync(string idRoom)
    {
        return await DataBaseManager.instance.SearchById("Rooms", idRoom);
    }

    public async Task<string> GetAvailableRoom() 
    {

        return await DataBaseManager.instance.GetAvailableRoom("Rooms");
    }

    public async Task<string> GetRoomByInductor(string idInductor)
    {
        return await DataBaseManager.instance.GetRoomByInductor("Rooms", idInductor);

    }

    public async Task<List<Dictionary<string, object>>> GetRoomsByOrderOfScore()
    {
        return await DataBaseManager.instance.SearchByOrderDescending("Rooms", "score");
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
    public int score = 0;
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
            { "score", this.score},
            { "idInductor", this.idInductor }
        };
    }
}
