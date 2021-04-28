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

    public void Refresh()
    {
        SearchGroupData();
        SearchStudents();
    }

    async void SearchGroupData()
    {
        object roomName = await DataBaseManager.Instance.SearchAttribute("Rooms", GlobalDataManager.Instance.idRoomByStudent, "room");
        string inductorName = GlobalDataManager.Instance.nameInductor;

        textGroupName.text = roomName.ToString();
        textInductorName.text = inductorName;    
    }

    async void SearchStudents()
    {
        List<string> NeoJaverianos = await DataBaseManager.Instance.ListStudentsByGroup(GlobalDataManager.Instance.idRoomByStudent);
        content.text = "";
        foreach(string name in NeoJaverianos)
        {
            content.text += name + "\n\n";
        }
    }
}
