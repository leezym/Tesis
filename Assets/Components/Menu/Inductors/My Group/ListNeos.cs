using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;


public class ListNeos : MonoBehaviour
{
    public Text content;
    public Canvas canvasMyGroup;
    int currentSizeStudents = 0, newSizeStudents = 0;

    async void Update()
    {
        if (canvasMyGroup.enabled)
            await DetectStudent();
        else
            currentSizeStudents = 0;
    }

    async Task DetectStudent()
    {
        newSizeStudents = GlobalDataManager.Instance.currentSizeRoom;
        if (currentSizeStudents != newSizeStudents)
        {
            List<string> studentsList = await DataBaseManager.Instance.ListStudentsByGroup(GlobalDataManager.Instance.idRoomByInductor);
            if(studentsList.Count != 0)
            {
                content.text = "";
                foreach(string name in studentsList)
                {
                    content.text += name + "\n\n";
                }            
                currentSizeStudents = newSizeStudents;
            }
        }
    }
}
