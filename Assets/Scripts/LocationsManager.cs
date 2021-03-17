using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class LocationsManager : MonoBehaviour
{
    private static LocationsManager instance;
    public static LocationsManager Instance { get => instance; set => instance = value; }

    public void Awake()
    {
        instance = this;
    }

    public async Task<List<Coords>> GetLocationAsync(string userType)
    {
        List<Coords> coordsList = new List<Coords>();
        if(userType == "inductor")
        {
            coordsList = await DataBaseManager.Instance.GetOthersInductorsLocation();
        }
        else if (userType == "student")
        {
            coordsList = await DataBaseManager.Instance.GetMyInductorLocation();
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
