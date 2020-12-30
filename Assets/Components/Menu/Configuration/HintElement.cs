using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void SaveHint()
    {
        Text hourText = this.transform.Find("HintTimeLabel").GetComponent<Text>();
        //string dateAndTimeVar = System.DateTime.Now.ToString(("yyyy/mm/dd HH:mm:ss"));
        string currentTime = System.DateTime.Now.ToString(("HH:mm"));
        hourText.text = currentTime;

        this.transform.Find("HintScoreInput").GetComponent<InputField>().interactable = false;
        this.transform.Find("HintPositionNumberInput").GetComponent<InputField>().interactable = false;
    }
}
