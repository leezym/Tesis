using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class EditGroup : MonoBehaviour
{ 
    private int roomCurrentSize;
    public Text groupNameLabel, inductorNameLabel;
    public Canvas canvasMyGroup, canvasEditGroup, canvasNombreInductor, canvasMenuInductor;
    public InputField inputRoomSize, inputInductorName;
    public InputField newInputRoomSize, newInputRoomName;
    
    void Start()
    {
        InitializeAttributes();
    }

    public void InitializeAttributes()
    {
        groupNameLabel.text = "";
        inductorNameLabel.text = "";
        inputRoomSize.text = "";
        inputInductorName.text = "";
        newInputRoomSize.text = "";
        newInputRoomName.text = "";
    }

    void Update()
    {
        if(canvasMyGroup.enabled && !canvasEditGroup.enabled)
        {
            ShowRoomData();
            ShowInductorData();           
        }
    }   

    async void ShowRoomData ()
    {
        Dictionary<string, object> datosRoom = await GroupManager.instance.GetRoomDataAsync();
        if (datosRoom != null)
        {
            foreach (KeyValuePair<string, object> pair in datosRoom)
            {
                if (pair.Key == "room")
                {
                    groupNameLabel.text = pair.Value.ToString();
                    newInputRoomName.text = pair.Value.ToString();
                }
                else if (pair.Key == "size")
                {
                    inputRoomSize.text = pair.Value.ToString();
                    newInputRoomSize.text = pair.Value.ToString();
                }
                else if (pair.Key == "currentsize")
                {
                    roomCurrentSize = Convert.ToInt32(pair.Value);
                }
            }
        }
    }

    async void ShowInductorData()
    {
        Dictionary<string,object> datosInductor = await GroupManager.instance.GetInductorDataAsync(inductorNameLabel);
        if (datosInductor != null){
            foreach(KeyValuePair<string, object> pair in datosInductor)
            {
                if (pair.Key == "name")
                {
                    inductorNameLabel.text = pair.Value.ToString();
                }
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

    public async void SendInductorName()
    {
        if (CheckNewData())
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "name" , inputInductorName.text}
            };
            await UsersManager.instance.PutUserAsync("Inductors", await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()), data);
            await RoomsManager.instance.PostNewRoom("Sala de " + inputInductorName.text, Convert.ToInt32(inputRoomSize.text), await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()));
            ScenesManager.instance.DeleteCurrentCanvas(canvasNombreInductor);
            ScenesManager.instance.LoadNewCanvas(canvasMenuInductor);
        }
    }

    public async void SendNewRoomInfo()
    {
        if (Convert.ToInt32(newInputRoomSize.text) >= roomCurrentSize)
        {
            string inductorId = await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId());
            string roomId = await RoomsManager.instance.GetRoomByInductor(inductorId);

            Dictionary<string, object> newRoomData = new Dictionary<string, object>
            {
                { "size" , Convert.ToInt32(newInputRoomSize.text)},
                { "room" , newInputRoomName.text}
            };
            Debug.Log("New room name: " + newInputRoomName.text);

            await RoomsManager.instance.PutRoomAsync(roomId, newRoomData);
        }
        /*else
        {
            advertencia = cosas de advertencia;
            WarningGenerator(advertencia);
            ScenesManager.instance.DeleteCurrentCanvas(canvasNombreInductor);
            ScenesManager.instance.LoadNewCanvas(canvasMenuInductor);
        }*/
    }
}
