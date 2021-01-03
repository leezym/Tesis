using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriviaElement : MonoBehaviour
{
    public async void SelectTrivia()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
        ScenesManager.instance.LoadNewCanvas(GameObject.Find("PanelTriviasDetails").GetComponent<Canvas>());
        ScenesManager.instance.DeleteCurrentCanvas(GameObject.Find("PanelConfigTrivias").GetComponent<Canvas>());
        GameObject.FindObjectOfType<EditTrivias>().InitializeAtributes();
        GameObject.FindObjectOfType<EditTrivias>().SearchTriviaDetails(hintText.text);
    }
}
