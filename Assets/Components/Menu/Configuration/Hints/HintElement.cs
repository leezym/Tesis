using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HintElement : MonoBehaviour
{
    public async void SelectHint()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
        ScenesManager.instance.LoadNewCanvas(GameObject.Find("PanelHintsDetails").GetComponent<Canvas>());
        ScenesManager.instance.DeleteCurrentCanvas(GameObject.Find("PanelConfigHints").GetComponent<Canvas>());
        GameObject.FindObjectOfType<EditHints>().InitializeAtributes();
        GameObject.FindObjectOfType<EditHints>().SearchHintDetails(hintText.text);
    }

    public void FinishHint()
    {
        NotificationsManager.instance.SetQuestionNotificationMessage("¿Está seguro que desea finalizar la pista? Una vez finalizada, no se pueden restaurar los cambios.");
        NotificationsManager.instance.acceptQuestionButton.onClick.AddListener(SaveHint);
    }

    async void SaveHint()
    {
        Text hourText = this.transform.Find("HintTimeLabel").GetComponent<Text>();
        //string dateAndTimeVar = System.DateTime.Now.ToString(("yyyy/mm/dd HH:mm:ss"));
        string currentTime = System.DateTime.Now.ToString(("HH:mm"));
        hourText.text = currentTime;

        this.transform.Find("HintScoreInput").GetComponent<InputField>().interactable = false;
        this.transform.Find("HintPositionNumberInput").GetComponent<InputField>().interactable = false;

        string hintName = this.transform.Find("HintNameLabel").GetComponent<Text>().text;
        Dictionary<string, object> newCardData = new Dictionary<string, object>
        {
            {"hour", this.transform.Find("HintTimeLabel").GetComponent<Text>().text},
            {"score", Convert.ToInt32(this.transform.Find("HintScoreInput").GetComponent<InputField>().text)},
            {"position", Convert.ToInt32(this.transform.Find("HintPositionNumberInput").GetComponent<InputField>().text)}
        };

        await HintsManager.instance.PutHintAsync(hintName, newCardData);
        
        if(this.transform.Find("HintTimeLabel").GetComponent<Text>().text != "")
            this.transform.Find("FinishButton").GetComponent<Button>().interactable = false;
        else
            this.transform.Find("FinishButton").GetComponent<Button>().interactable = true;
}
}
