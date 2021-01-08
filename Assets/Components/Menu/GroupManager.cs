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
        Dictionary<string, object> data = await DataBaseManager.instance.ListStudentsByGroup("Students", idInductor);
        List<string> Estudiantes = new List<string>();
        if (data != null)
        {
            foreach (KeyValuePair<string, object> pair in data)
            {
                if (pair.Key == "name")
                {
                    Estudiantes.Add(pair.Value.ToString());
                }        
            }
        }
        return Estudiantes;
    }

    public async Task<int> SearchCurrentSizeRoom(string idInductor)
    {
        string idRoom = await RoomsManager.instance.GetRoomByInductor(idInductor);
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
        string roomId = await RoomsManager.instance.GetRoomByInductor(await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()));
        return await RoomsManager.instance.GetRoomAsync(roomId);
    }

    public async Task<Dictionary<string,object>> GetInductorDataAsync(Text inductorNameLabel)
    {
        return await UsersManager.instance.GetUserAsync("Inductors", await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()));        
    }

    public async Task<List<Dictionary<string, string>>> GetOtherGroupsDataAsync()
    {
        string nameInductor = "", nameRoom  = "", idInductor = "";
        List<Dictionary<string, string>> Salas = new List<Dictionary<string, string>>();
        List<Dictionary<string, object>> rooms = await DataBaseManager.instance.SearchByCollection("Rooms");
        foreach (Dictionary<string, object> room in rooms)
        {
            foreach (KeyValuePair<string, object> pairRoom in room)
            {  
                if (pairRoom.Key == "room")
                {
                    nameRoom = pairRoom.Value.ToString();
                }
                else if (pairRoom.Key == "idInductor")
                {
                    idInductor = pairRoom.Value.ToString();
                    Dictionary<string, object> inductor = await UsersManager.instance.GetUserAsync("Inductors", idInductor);
                    if (inductor != null)
                    {
                        foreach (KeyValuePair<string, object> pairInductor in inductor)
                        {   
                            if (pairInductor.Key == "name")
                            {
                                nameInductor = pairInductor.Value.ToString();
                            }
                        }
                    }
                }
            }

            if (idInductor != await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()))
            {
                Salas.Add(new Dictionary<string, string> () {
                    {"nameInductor", nameInductor},
                    {"nameRoom", nameRoom}
                });
            }
        }        
        return Salas;
    }
}