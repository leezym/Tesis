using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class EditGroup : MonoBehaviour
{
    private string roomId, sizeRoom;
    private Dictionary<string, object> datosRoom, datosInductor;
    public Text groupName, inductorNameLabel;
    public Canvas canvasNombreInductor, canvasMenuInductor;
    public InputField inputRoomSize, inputInductorName;
    // Start is called before the first frame update
    void Start()
    {
        sizeRoom = "";
        inputRoomSize.text = "";
        inputInductorName.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Canvas>().enabled)
        {
            //GetRoomDataAsync();
            //GetInductorName();
        }
    }

    async void GetRoomDataAsync()
    {
        roomId = await RoomsManager.instance.SearchRoomByInductor("Rooms", AuthManager.instance.GetUserId());
        datosRoom = await RoomsManager.instance.GetRoomAsync("Rooms", roomId);
        foreach (KeyValuePair<string, object> pair in datosRoom)
        {
            if (pair.Key == "room")
            {
                groupName.text = pair.Value.ToString();
            }
            else if (pair.Key == "currentSize")
            {
                sizeRoom = pair.Value.ToString();
            }
        }
    }

    async void GetInductorName()
    {
        datosInductor = await UsersManager.instance.GetUserAsync("Inductors", AuthManager.instance.GetUserId());
        foreach(KeyValuePair<string, object> pair in datosInductor)
        {
            Debug.Log(pair.Key + " " + pair.Value);
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
            //UsersManager.instance.PutUserAsync("Inductors", AuthManager.instance.GetUserId(), "name", inputInductorName.text);
            //RoomsManager.instance.PutRoomAsync("Rooms", roomId, "size", inputRoomSize.text);
            return true;
        }

        return false;
    }

    public void SendInductorName()
    {
        //Debug.Log(CheckNewData());
        //if (CheckNewData())
        //{
            UsersManager.instance.PutUserAsync("Inductors", AuthManager.instance.GetUserId(), "name", inputInductorName.text);
            RoomsManager.instance.PostNewRoom("Sala de " + inputInductorName.text, Convert.ToInt32(inputRoomSize.text), AuthManager.instance.GetUserId());
            ScenesManager.instance.DeleteCurrentCanvas(canvasNombreInductor);
            ScenesManager.instance.LoadNewCanvas(canvasMenuInductor);
        //}
    }
}
