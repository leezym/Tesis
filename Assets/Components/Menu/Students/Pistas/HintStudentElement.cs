using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintStudentElement : MonoBehaviour
{
    public void SelectHint()
    {
        Text hintTitle = this.gameObject.GetComponentInChildren<Text>();

        GameObject canvasDescription = GameObject.Find("PanelPistas").transform.Find("Description").GetComponent<GameObject>();
        Text hintText = canvasDescription.GetComponentInChildren<Text>();
        foreach(InfoHint hint in GameObject.FindObjectOfType<ListStudentHints>().currentHints)
        {
            if(hint.name == hintTitle.text)
                hintText.text = hint.description;
        }
        canvasDescription.SetActive(true);
    }
}

