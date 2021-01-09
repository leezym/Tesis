using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TriviaElement : MonoBehaviour
{
    // 3seg Contador, 30seg Pregunta, 10seg 
    int timer = 3;
    int question = 30;
    int answer = 10;

    public async void SelectTrivia()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
        ScenesManager.instance.LoadNewCanvas(GameObject.Find("PanelTriviasDetails").GetComponent<Canvas>());
        ScenesManager.instance.DeleteCurrentCanvas(GameObject.Find("PanelConfigTrivias").GetComponent<Canvas>());
        GameObject.FindObjectOfType<EditTrivias>().InitializeAtributes();
        GameObject.FindObjectOfType<EditTrivias>().SearchTriviaDetails(hintText.text);
    }

    public void StartTrivia()
    {
        int amoungQuestions = Convert.ToInt32(this.transform.Find("AmountQuestionsLabel").GetComponent<Text>().text);

        if(amoungQuestions != 0)
        {
            NotificationsManager.instance.SetQuestionNotificationMessage("¿Está seguro que desea iniciar la trivia? Una vez iniciada, no se pueden restaurar los cambios.");
            NotificationsManager.instance.acceptQuestionButton.onClick.AddListener(ExecuteTrivia);
        }
        else
        {
            NotificationsManager.instance.SetFailureNotificationMessage("No hay preguntas para este edificio. Agrega algunas e intenta nuevamente.");
        }
    }

    async void ExecuteTrivia()
    {
        GameObject canvasLoading = GameObject.Find("InductorLoadingTrivias");
        this.transform.Find("InitializeTriviaButton").GetComponent<Button>().interactable = false;

        // Datos cantidad de preguntas
        int amoungQuestions = Convert.ToInt32(this.transform.Find("AmountQuestionsLabel").GetComponent<Text>().text);
        float timeQuestions = amoungQuestions * (timer + question + answer);
        Text timeLabel = canvasLoading.transform.Find("TimeLabel").GetComponent<Text>();
        timeLabel.text = timeQuestions.ToString();
        
        // Datos edificio
        string buildingName = this.transform.Find("BuildingNameLabel").GetComponent<Text>().text;
        string idBuilding = await DataBaseManager.instance.SearchId("Buildings", "name", buildingName);
        

        await TriviasChallengesManager.instance.PostNewInductorTriviaChallenge(await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()), idBuilding, false);
        ScenesManager.instance.LoadNewCanvas(canvasLoading.GetComponent<Canvas>());
    }
}
