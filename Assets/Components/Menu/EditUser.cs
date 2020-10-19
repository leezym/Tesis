using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditUser : MonoBehaviour
{
    public Text TextbuttonEdit;
    public void EditRoomName(InputField input) 
    {
        if (input.interactable)
        {
            input.interactable = false;
            TextbuttonEdit.text = "Editar";
            string userId = AuthManager.instance.GetIdUser();
            UsersManager.instance.PutUser("Inductors", userId, "room", input.text);
        }
        else 
        {
            input.interactable = true;
            TextbuttonEdit.text = "Guardar";            
        }
    }
}
