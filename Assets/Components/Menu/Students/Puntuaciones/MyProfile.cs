using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyProfile : MonoBehaviour
{
    public Canvas canvasPuntuaciones, canvasMyProfile;
    public Text textGroupName, textInductorName, content;
    List<string> NeoJaverianos = new List<string>();
    int currentSizeStudents = 0, newSizeStudents = 0;
    object idRoom, idInductor;

    async void Update()
    {
        if (canvasPuntuaciones.enabled && canvasMyProfile.enabled)
        {
            if (idRoom == null)
            {
                idRoom = await DataBaseManager.instance.SearchAttribute("Students", AuthManager.instance.GetUserId(), "idRoom");
                if (idInductor == null)
                {
                    idInductor = await DataBaseManager.instance.SearchAttribute("Rooms", idRoom.ToString(), "idInductor");
                }
            }
            if (idRoom != null && idInductor != null)
                SearchGroupData();
            if (idInductor != null)
                SearchStudents();
        }
        else
        {
            idRoom = null;
            idInductor = null;
        }
    }

    async void SearchGroupData()
    {
        object roomName = await DataBaseManager.instance.SearchAttribute("Rooms", idRoom.ToString(), "room");
        object inductorName = await DataBaseManager.instance.SearchAttribute("Inductors", idInductor.ToString(), "name");

        textGroupName.text = roomName.ToString();
        textInductorName.text = inductorName.ToString();    
    }

    async void SearchStudents()
    {
        NeoJaverianos = await GroupManager.instance.ListNameStudents(idInductor.ToString());
        newSizeStudents = NeoJaverianos.Count;

        if (currentSizeStudents != newSizeStudents)
        {
            content.text = "";
            foreach(string name in NeoJaverianos)
            {
                content.text += name + "\n\n";
            }
            currentSizeStudents = newSizeStudents;
        }
    }
}
