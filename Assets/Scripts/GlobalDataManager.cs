using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDataManager : MonoBehaviour
{
    private static GlobalDataManager instance;
    public static GlobalDataManager Instance { get => instance; set => instance = value; }

    public static string idUser;
    public static string idInductor;
    public static string idRoom;
    public static string idBuildingPalmas;
    public static string idBuildingLago;
    public static string idBuildingRaulPosada;
    public static string idBuildingGuayacanes;
    public static string nameBuildingPalmas;
    public static string nameBuildingLago;
    public static string nameBuildingRaulPosada;
    public static string nameBuildingGuayacanes;
    public int currentSizeRoom;

    void Awake()
    {
        instance = this;
    }

}
