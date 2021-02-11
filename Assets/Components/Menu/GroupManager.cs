using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine.UIElements;

public class GroupManager : MonoBehaviour
{
    public static GroupManager instance;
    public static GroupManager Instance { get => instance; set => instance = value; }

    void Awake ()
    {
        instance = this;
    }

    public async Task<List<string>> ListNameStudents(string idInductor)
    {
        Dictionary<string, object> data = await DataBaseManager.Instance.ListStudentsByGroup("Students", idInductor);
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
        string idRoom = await RoomsManager.Instance.GetRoomByInductor(idInductor);
        Dictionary<string, object> data = await DataBaseManager.Instance.SearchById("Rooms", idRoom);
        foreach (KeyValuePair<string, object> pair in data)
        {
            if (pair.Key == "currentSize")
                return Convert.ToInt32(pair.Value);
        }
        return 0;
    }

    public async Task<Dictionary<string,object>> GetRoomDataAsync()
    {
        string roomId = await RoomsManager.Instance.GetRoomByInductor(await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId()));
        return await RoomsManager.Instance.GetRoomAsync(roomId);
    }

    public async Task<Dictionary<string,object>> GetInductorDataAsync(Text inductorNameLabel)
    {
        return await UsersManager.Instance.GetUserAsync("Inductors", await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId()));        
    }

    public async Task<List<Dictionary<string, string>>> GetOtherGroupsDataAsync()
    {
        string nameInductor = "", nameRoom  = "", idInductor = "";
        List<Dictionary<string, string>> Salas = new List<Dictionary<string, string>>();
        List<DocumentSnapshot> rooms = await DataBaseManager.Instance.SearchByCollection("Rooms");
        foreach (DocumentSnapshot room in rooms)
        {
            foreach (KeyValuePair<string, object> pairRoom in room.ToDictionary())
            {  
                if (pairRoom.Key == "room")
                {
                    nameRoom = pairRoom.Value.ToString();
                }
                else if (pairRoom.Key == "idInductor")
                {
                    idInductor = pairRoom.Value.ToString();
                    Dictionary<string, object> inductor = await UsersManager.Instance.GetUserAsync("Inductors", idInductor);
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

            if (idInductor != await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId()))
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