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
    object idRoom, idInductor;

    public async void Refresh()
    {
        if (idRoom == null)
        {
            idRoom = await DataBaseManager.Instance.SearchAttribute("Students", AuthManager.Instance.GetUserId(), "idRoom");
            if (idInductor == null)
            {
                idInductor = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom.ToString(), "idInductor");
            }
        }

        if (idRoom != null && idInductor != null)
                SearchGroupData();
        if (idInductor != null)
            SearchStudents();
    }

    async void SearchGroupData()
    {
        object roomName = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom.ToString(), "room");
        object inductorName = await DataBaseManager.Instance.SearchAttribute("Inductors", idInductor.ToString(), "name");

        if(roomName != null)
            textGroupName.text = roomName.ToString();
        if (inductorName != null)
            textInductorName.text = inductorName.ToString();    
    }

    async void SearchStudents()
    {
        NeoJaverianos = await GroupManager.Instance.ListNameStudents(idInductor.ToString());
        content.text = "";
        foreach(string name in NeoJaverianos)
        {
            content.text += name + "\n\n";
        }
    }
}
