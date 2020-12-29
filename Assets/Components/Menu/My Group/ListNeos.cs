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
    public List<string> NeoJaverianos = new List<string>();
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
            idInductor = AuthManager.instance.GetUserId();
            currentSizeStudents = await GroupManager.instance.SearchCurrentSizeRoom(idInductor);
        }

        NeoJaverianos = await GroupManager.instance.ListNameStudents(idInductor);
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
