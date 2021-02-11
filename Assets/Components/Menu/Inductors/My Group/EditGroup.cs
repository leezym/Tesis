using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

public class EditGroup : MonoBehaviour
{ 
    private int roomCurrentSize;
    public Text groupNameLabel, inductorNameLabel;
    public Canvas canvasMyGroup, canvasEditGroup, canvasNombreInductor, canvasMenuInductor;
    public InputField inputRoomSize, inputInductorName;
    public InputField newInputRoomSize, newInputRoomName;
    public Button finishButton;
    [HideInInspector]
    public int countTrivias = 0, countHints = 0;
    
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

    async void Update()
    {
        if(canvasMyGroup.enabled && !canvasEditGroup.enabled)
        {
            ShowRoomData();
            ShowInductorData();           
        }

        if(countTrivias == 1 && countHints == await DataBaseManager.Instance.SizeTable("Hints"))
            finishButton.interactable = true;     
    }   

    async void ShowRoomData ()
    {
        Dictionary<string, object> datosRoom = await GroupManager.Instance.GetRoomDataAsync();
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
        Dictionary<string,object> datosInductor = await GroupManager.Instance.GetInductorDataAsync(inductorNameLabel);
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
            string idInductor = await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId());

            await UsersManager.Instance.PutUserAsync("Inductors", idInductor, new Dictionary<string, object>{
                {"name", inputInductorName.text}
            });
            await RoomsManager.Instance.PostNewRoom("Grupo de " + inputInductorName.text, Convert.ToInt32(inputRoomSize.text), idInductor);
            
            ScenesManager.Instance.DeleteCurrentCanvas(canvasNombreInductor);
            ScenesManager.Instance.LoadNewCanvas(canvasMenuInductor);

        }
        else
        {
            NotificationsManager.Instance.SetFailureNotificationMessage("Por favor llene los campos.");
        }
    }

    public async void SendNewRoomInfo()
    {
        if (Convert.ToInt32(newInputRoomSize.text) >= roomCurrentSize)
        {
            string inductorId = await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId());
            string roomId = await RoomsManager.Instance.GetRoomByInductor(inductorId);

            Dictionary<string, object> newRoomData = new Dictionary<string, object>
            {
                { "size" , Convert.ToInt32(newInputRoomSize.text)},
                { "room" , newInputRoomName.text}
            };

            await RoomsManager.Instance.PutRoomAsync(roomId, newRoomData);
            NotificationsManager.Instance.SetSuccessNotificationMessage("El grupo "+newInputRoomName.text+" fue editado.");
        }
        else
        {
            NotificationsManager.Instance.SetFailureNotificationMessage("El tamaño del grupo no puede ser menor que el tamaño actual.");
        }
    }

    public async void FinishedGroup()
    {
        string idInductor = await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId());
        string idRoom = await DataBaseManager.Instance.GetRoomByInductor(idInductor);
        await RoomsManager.Instance.PutRoomAsync(idRoom, new Dictionary<string, object>{
            {"finished", true}
        });  
    }
}
