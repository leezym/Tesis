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
    [Header ("INDUCTOR")]
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

    public async Task<Dictionary<string,object>> GetRoomDataAsync()
    {
        return await RoomsManager.Instance.GetRoomAsync(GlobalDataManager.Instance.idRoomByInductor);
    }

    public async Task<Dictionary<string,object>> GetInductorDataAsync(Text inductorNameLabel)
    {
        return await UsersManager.Instance.GetUserAsync("Inductors", GlobalDataManager.Instance.idUserInductor);        
    }

    public async Task<List<Dictionary<string, string>>> GetOtherGroupsDataAsync()
    {
        string nameRoom  = "";
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
            }

            string nameInductor = (await DataBaseManager.Instance.SearchAttribute("Inductors", GlobalDataManager.Instance.idUserInductor, "name")).ToString();
            Salas.Add(new Dictionary<string, string> () {
                {"nameInductor", nameInductor},
                {"nameRoom", nameRoom}
            });
        }        
        return Salas;
    }
}