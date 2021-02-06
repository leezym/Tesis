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

        if(countTrivias == await DataBaseManager.instance.SizeTable("Buildings") && countHints == await DataBaseManager.instance.SizeTable("Hints"))
        {
            finishButton.interactable = true;
            string idInductor = await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId());
            string idRoom = await DataBaseManager.instance.GetRoomByInductor(idInductor);
            await RoomsManager.instance.PutRoomAsync(idRoom, new Dictionary<string, object>{
                {"finished", true}
            });

            int sizeRoomsTable = await DataBaseManager.instance.SizeTable("Rooms");
            int sizeFinishedRoomsTable = await DataBaseManager.instance.SizeTable("Rooms", "finished", true);
            if (sizeFinishedRoomsTable == sizeRoomsTable)
            {
                LoadingScreenManager.instance.ShowFinalRankingYincana();
                ScenesManager.instance.LoadNewCanvas(LoadingScreenManager.instance.canvasRankingFinal);
            }
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
            string idInductor = await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId());

            await UsersManager.instance.PutUserAsync("Inductors", idInductor, new Dictionary<string, object>{
                {"name", inputInductorName.text}
            });
            await RoomsManager.instance.PostNewRoom("Grupo de " + inputInductorName.text, Convert.ToInt32(inputRoomSize.text), idInductor);
            
            ScenesManager.instance.DeleteCurrentCanvas(canvasNombreInductor);
            ScenesManager.instance.LoadNewCanvas(canvasMenuInductor);

        }
        else
        {
            NotificationsManager.instance.SetFailureNotificationMessage("Por favor llene los campos.");
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

            await RoomsManager.instance.PutRoomAsync(roomId, newRoomData);
            NotificationsManager.instance.SetSuccessNotificationMessage("El grupo "+newInputRoomName.text+" fue editado.");
        }
        else
        {
            NotificationsManager.instance.SetFailureNotificationMessage("El tamaño del grupo no puede ser menor que el tamaño actual.");
        }
    }
}
