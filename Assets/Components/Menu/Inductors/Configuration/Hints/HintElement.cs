using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HintElement : MonoBehaviour
{
    public void SelectHint()
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
        string hour = this.transform.Find("HintTimeLabel").GetComponent<Text>().text;
        int score = Convert.ToInt32(this.transform.Find("HintScoreInput").GetComponent<InputField>().text);
        int  position = Convert.ToInt32(this.transform.Find("HintPositionNumberInput").GetComponent<InputField>().text);

        // Crear la pista con puntuaciones
        string idRoom = await RoomsManager.instance.GetRoomByInductor(await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()));
        string idHint = await HintsManager.instance.GetIdHintByName(hintName);
        await HintsChallengesManager.instance.PostNewHintChallenge(idRoom, idHint, score, position, hour);

        // Asignar puntuaciones totales a la sala
        object currentRoomScore = await DataBaseManager.instance.SearchAttribute("Rooms", idRoom, "score");
        await RoomsManager.instance.PutRoomAsync(idRoom, new Dictionary<string, object> {
            {"score", Convert.ToInt32(currentRoomScore) + score}
        });
        
        if(this.transform.Find("HintTimeLabel").GetComponent<Text>().text != "")
            this.transform.Find("FinishButton").GetComponent<Button>().interactable = false;
        else
            this.transform.Find("FinishButton").GetComponent<Button>().interactable = true;
}
}
