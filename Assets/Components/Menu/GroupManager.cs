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
        string nameRoom  = "", idInductor = "", nameInductor = "";
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
            
            Salas.Add(new Dictionary<string, string> () {
                {"nameInductor", nameInductor},
                {"nameRoom", nameRoom}
            });
        }        
        return Salas;
    }
}