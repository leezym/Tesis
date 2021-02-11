using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class LocationsManager : MonoBehaviour
{
    public static LocationsManager instance;
    public static LocationsManager Instance { get => instance; set => instance = value; }

    public void Awake()
    {
        instance = this;
    }

    public async Task<List<Coords>> GetLocationAsync(string userType)
    {
        List<Coords> coordsList = new List<Coords>();
        if(AuthManager.Instance.GetUserType() == "inductor")
        {
            string idInductor = await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId());
            coordsList = await DataBaseManager.Instance.GetOthersInductorsLocation(idInductor);
            Debug.Log("coordenadas");
        }
        else if (AuthManager.Instance.GetUserType() == "student")
        {
            string idStudent = AuthManager.Instance.GetUserId();
            coordsList = await DataBaseManager.Instance.GetMyInductorLocation(idStudent);
        }
        
        return coordsList;
    }
}

public class Coords
{
    public float latitude;
    public float longitude;

    public Coords() { }

    public Coords(float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }
}
