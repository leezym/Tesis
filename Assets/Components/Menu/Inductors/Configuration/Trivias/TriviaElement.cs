using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class TriviaElement : MonoBehaviour
{
    string idBuilding = "";

    public void SelectTrivia()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
        ScenesManager.Instance.LoadNewCanvas(GameObject.Find("PanelTriviasDetails").GetComponent<Canvas>());
        ScenesManager.Instance.DeleteCurrentCanvas(GameObject.Find("PanelConfigTrivias").GetComponent<Canvas>());
        GameObject.FindObjectOfType<EditTrivias>().InitializeAtributes();
        GameObject.FindObjectOfType<EditTrivias>().SearchTriviaDetails(hintText.text);
    }

    public void StartTrivia()
    {
        int amoungQuestions = Convert.ToInt32(this.transform.Find("AmountQuestionsLabel").GetComponent<Text>().text);

        if(amoungQuestions != 0)
        {
            NotificationsManager.Instance.SetQuestionNotificationMessage("¿Está seguro que desea iniciar la trivia? Una vez iniciada, no se pueden restaurar los cambios.");
            NotificationsManager.Instance.acceptQuestionButton.onClick.AddListener(ExecuteTrivia);
        }
        else
        {
            NotificationsManager.Instance.SetFailureNotificationMessage("No hay preguntas para este edificio. Agrega algunas e intenta nuevamente.");
        }
    }

    async void ExecuteTrivia()
    {
        GameObject.FindObjectOfType<EditGroup>().countTrivias ++;
        Debug.Log(GameObject.FindObjectOfType<EditGroup>().countTrivias);
        
        this.transform.Find("InitializeTriviaButton").GetComponent<Button>().interactable = false;

        // Datos cantidad de preguntas
        int amoungQuestions = Convert.ToInt32(this.transform.Find("AmountQuestionsLabel").GetComponent<Text>().text);
        float timeQuestions = amoungQuestions * (LoadingScreenManager.Instance.timer + 
                                                LoadingScreenManager.Instance.question + 
                                                LoadingScreenManager.Instance.waiting +
                                                LoadingScreenManager.Instance.isOver);
        
        // Datos edificio
        string buildingName = this.transform.Find("BuildingNameLabel").GetComponent<Text>().text;
        idBuilding = await DataBaseManager.Instance.SearchId("Buildings", "name", buildingName);
        

        await TriviasChallengesManager.Instance.PostNewInductorTriviaChallenge(await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId()), idBuilding, true);
        ScenesManager.Instance.LoadNewCanvas(LoadingScreenManager.Instance.canvasInductorLoading);
        
        LoadingScreenManager.Instance.SetTimeInductorLoading(timeQuestions);
        LoadingScreenManager.Instance.SetIdTriviaBuilding(idBuilding);
    }
}
