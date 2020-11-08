using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class EditGroup : MonoBehaviour
{
    public Text groupNameLabel, inductorNameLabel;
    public Canvas canvasMyGroup, canvasNombreInductor, canvasMenuInductor;
    public InputField inputRoomSize, inputInductorName;
    //public InputField newInputRoomSize, newInputRoomName;
    
    void Start()
    {
        initializeAttributes();
    }
    public void initializeAttributes()
    {
        groupNameLabel.text = "";
        inductorNameLabel.text = "";
        inputRoomSize.text = "";
        inputInductorName.text = "";
    }

    void Update()
    {
        if(canvasMyGroup.enabled)
        {
            ShowRoomData();
            ShowInductorData();
        }
    }   

    async void ShowRoomData ()
    {
        Dictionary<string, object> datosRoom = await GroupManager.instance.GetRoomDataAsync();
        foreach (KeyValuePair<string, object> pair in datosRoom)
        {
            if (pair.Key == "room")
            {
                groupNameLabel.text = pair.Value.ToString();
            }
            else if (pair.Key == "currentSize")
            {
                inputInductorName.text = pair.Value.ToString();
            }
        }
    }

    async void ShowInductorData()
    {
        Dictionary<string,object> datosInductor = await GroupManager.instance.GetInductorDataAsync(inductorNameLabel);
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
