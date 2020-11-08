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

    /*public async Task PutRoomAsync(string db, string userId, string atribute, string value)
    {
        await DataBaseManager.instance.UpdateUserAsync(db, userId, atribute, value);
    }*/

    public async Task<Dictionary<string, object>> GetRoomAsync(string db, string userId)
    {
        return await DataBaseManager.instance.SearchById(db, userId);
    }

    public async Task<string> SearchAvailableRoom(string db) 
    {

        return await DataBaseManager.instance.SearchAvailableRoom(db);
    }

    public async Task DeleteRoomAsync(string db, string inductorId)
    {
        await DataBaseManager.instance.DeleteRoomAsync(db, inductorId);

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
