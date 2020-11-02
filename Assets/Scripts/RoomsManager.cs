using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class RoomsManager : MonoBehaviour
{
    public static RoomsManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void PostNewRoom(string room, string idInductor)
    {
        Room element = new Room(room, idInductor);
        DataBaseManager.instance.InsertWithoutId("Rooms", element.ConvertJson());
    }

    /*public async Task PutRoomAsync(string db, string userId, string atribute, string value)
    {
        await DataBaseManager.instance.UpdateUserAsync(db, userId, atribute, value);
    }

    public async Task<Dictionary<string, object>> GetRoomAsync(string db, string userId)
    {
        return await DataBaseManager.instance.SelectUserByIdAsync(db, userId);
    }*/

    public async Task DeleteRoomAsync(string db, string inductorId)
    {
        await DataBaseManager.instance.DeleteRoomAsync(db, inductorId);

    }
}

public class Room
{
    public string room;
    public string idInductor;

    public Room() { }

    public Room(string room, string idInductor)
    {
        this.room = room;
        this.idInductor = idInductor;
    }
    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "room", this.room },
            { "idInductor", this.idInductor }
        };
    }
}
