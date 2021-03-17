using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyProfile : MonoBehaviour
{
    [Header ("STUDENT")]
    public Canvas canvasPuntuaciones, canvasMyProfile;
    public Text textGroupName, textInductorName, content;
    List<string> NeoJaverianos = new List<string>();

    public void Refresh()
    {
        SearchGroupData();
        SearchStudents();
    }

    async void SearchGroupData()
    {
        object roomName = await DataBaseManager.Instance.SearchAttribute("Rooms", GlobalDataManager.Instance.idRoomByStudent, "room");
        object inductorName = await DataBaseManager.Instance.SearchAttribute("Inductors", GlobalDataManager.Instance.idInductorByStudent, "name");

        if(roomName != null)
            textGroupName.text = roomName.ToString();
        if (inductorName != null)
            textInductorName.text = inductorName.ToString();    
    }

    async void SearchStudents()
    {
        NeoJaverianos = await GroupManager.Instance.ListNameStudents(GlobalDataManager.Instance.idUserInductor);
        content.text = "";
        foreach(string name in NeoJaverianos)
        {
            content.text += name + "\n\n";
        }
    }
}
