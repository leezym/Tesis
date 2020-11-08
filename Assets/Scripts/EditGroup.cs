using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class EditGroup : MonoBehaviour
{
    private string roomId;
    private Dictionary<string, object> datosRoom, datosInductor;
    public Text groupNameLabel, inductorNameLabel;
    public Canvas canvasNombreInductor, canvasMenuInductor;
    public InputField inputRoomSize, inputInductorName, sizeRoomLabel;
    // Start is called before the first frame update
    void Start()
    {
        sizeRoomLabel.text = "";
        inputRoomSize.text = "";
        inputInductorName.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Canvas>().enabled)
        {
            GetRoomDataAsync();
            GetInductorDataAsync();
        }
    }

    async void  GetRoomDataAsync()
    {
        roomId = await RoomsManager.instance.SearchRoomByInductor("Rooms", AuthManager.instance.GetUserId());
        datosRoom = await RoomsManager.instance.GetRoomAsync("Rooms", roomId);
        foreach (KeyValuePair<string, object> pair in datosRoom)
        {
            if (pair.Key == "room")
            {
                groupNameLabel.text = pair.Value.ToString();
            }
            else if (pair.Key == "currentSize")
            {
                sizeRoomLabel.text = pair.Value.ToString();
            }
        }
    }

    async void GetInductorDataAsync()
    {
        datosInductor = await UsersManager.instance.GetUserAsync("Inductors", AuthManager.instance.GetUserId());
        foreach(KeyValuePair<string, object> pair in datosInductor)
        {
            if (pair.Key == "name")
            {
                inductorNameLabel.text = pair.Value.ToString();
            }
        }
    }

    public bool CheckNewData()
    {
        if( inputInductorName.text != "" && inputRoomSize.text != "")
        {            
            return true;
        }

        return false;
    }

    public void SendInductorName()
    {
        if (CheckNewData())
        {
            UsersManager.instance.PutUserAsync("Inductors", AuthManager.instance.GetUserId(), "name", inputInductorName.text);
            RoomsManager.instance.PostNewRoom("Sala de " + inputInductorName.text, Convert.ToInt32(inputRoomSize.text), AuthManager.instance.GetUserId());
            ScenesManager.instance.DeleteCurrentCanvas(canvasNombreInductor);
            ScenesManager.instance.LoadNewCanvas(canvasMenuInductor);
        }
    }
}
