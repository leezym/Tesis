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
    private string idInductor;
    public Canvas canvasMyGroup;
    List<string> NeoJaverianos = new List<string>();
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
        if (idInductor == null){
            idInductor = await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId());
            currentSizeStudents = await GroupManager.Instance.SearchCurrentSizeRoom(idInductor);
        }

        NeoJaverianos = await GroupManager.Instance.ListNameStudents(idInductor);
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
