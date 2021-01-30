using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class LocationsManager : MonoBehaviour
{
    public static LocationsManager instance;

    public void Awake()
    {
        instance = this;
    }

    public async Task<List<Coords>> GetLocationAsync(string userType)
    {
        List<Coords> coordsList = new List<Coords>();
        if(AuthManager.instance.GetUserType() == "inductor")
        {
            string idInductor = await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId());
            coordsList = await DataBaseManager.instance.GetOthersInductorsLocation(idInductor);
            Debug.Log("coordenadas");
        }
        else if (AuthManager.instance.GetUserType() == "student")
        {
            string idStudent = AuthManager.instance.GetUserId();
            coordsList = await DataBaseManager.instance.GetMyInductorLocation(idStudent);
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
