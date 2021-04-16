using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalDataManager : MonoBehaviour
{
    private static GlobalDataManager instance;
    public static GlobalDataManager Instance { get => instance; set => instance = value; }

    public string userType;

    [Header("SONIDOS")]
    public AudioSource click;
    public AudioSource win;
    public AudioSource lose;

    [Header("INDUCTOR")]
    public string idUserInductor;
    public string idRoomByInductor;
    public int currentSizeRoom;
    public string nameInductor;

    [Header("STUDENT")]
    public string idUserStudent;
    public string idInductorByStudent;
    public string idRoomByStudent;
    
    public static string idBuildingPalmas = "y5rUPG7xeARjgWB0ZqzC";
    public static string idBuildingLago = "aDbrb5YIcSNH63HK0NMW";
    public static string idBuildingRaulPosada = "nLErPD8l2t0NqrnjspDb";
    public static string idBuildingGuayacanes = "vPQxABwviGBG9ZryX79Z";
    public static string nameBuildingPalmas = "Las Palmas";
    public static string nameBuildingLago = "El Lago";
    public static string nameBuildingRaulPosada = "Raúl Posada S.J.";
    public static string nameBuildingGuayacanes = "Los Guayacanes";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Button[] buttons = GameObject.FindObjectsOfType<Button>();
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(()=>{
                click.Play();
            });
        }
    }

    public void SetUserType(string userType) { this.userType = userType; }
}
