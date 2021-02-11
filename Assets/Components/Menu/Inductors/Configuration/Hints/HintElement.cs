using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HintElement : MonoBehaviour
{
    string score = "", position = "";

    public void SelectHint()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
        ScenesManager.Instance.LoadNewCanvas(GameObject.Find("PanelHintsDetails").GetComponent<Canvas>());
        ScenesManager.Instance.DeleteCurrentCanvas(GameObject.Find("PanelConfigHints").GetComponent<Canvas>());
        GameObject.FindObjectOfType<EditHints>().InitializeAtributes();
        GameObject.FindObjectOfType<EditHints>().SearchHintDetails(hintText.text);
    }

    public void FinishHint()
    {
        score = this.transform.Find("HintScoreInput").GetComponent<InputField>().text;
        position = this.transform.Find("HintPositionNumberInput").GetComponent<InputField>().text;

        if (score == "" || position == "")
        {
            NotificationsManager.Instance.SetFailureNotificationMessage("Por favor, llena los campos posición y puntuación.");
        }
        else
        {
            NotificationsManager.Instance.SetQuestionNotificationMessage("¿Está seguro que desea finalizar la pista? Una vez finalizada, no se pueden restaurar los cambios.");
            NotificationsManager.Instance.acceptQuestionButton.onClick.AddListener(SaveHint);
        }
    }

    async void SaveHint()
    {
        GameObject.FindObjectOfType<EditGroup>().countHints ++;
        Debug.Log(GameObject.FindObjectOfType<EditGroup>().countHints);
        
        Text hourText = this.transform.Find("HintTimeLabel").GetComponent<Text>();
        string currentTime = System.DateTime.Now.ToString(("HH:mm"));
        hourText.text = currentTime;

        this.transform.Find("HintScoreInput").GetComponent<InputField>().interactable = false;
        this.transform.Find("HintPositionNumberInput").GetComponent<InputField>().interactable = false;

        string hintName = this.transform.Find("HintNameLabel").GetComponent<Text>().text;
        string hour = this.transform.Find("HintTimeLabel").GetComponent<Text>().text;
        

        // Crear la pista con puntuaciones
        string idRoom = await RoomsManager.Instance.GetRoomByInductor(await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId()));
        string idHint = await HintsManager.Instance.GetIdHintByName(hintName);
        await HintsChallengesManager.Instance.PostNewHintChallenge(idRoom, idHint, Convert.ToInt32(score), Convert.ToInt32(position), hour);

        // Asignar puntuaciones totales a la sala
        object currentRoomScore = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom, "score");
        await RoomsManager.Instance.PutRoomAsync(idRoom, new Dictionary<string, object> {
            {"score", Convert.ToInt32(currentRoomScore) + score}
        });
        
        if(this.transform.Find("HintTimeLabel").GetComponent<Text>().text != "")
            this.transform.Find("FinishButton").GetComponent<Button>().interactable = false;
        else
            this.transform.Find("FinishButton").GetComponent<Button>().interactable = true;
}
}
