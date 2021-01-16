using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager instance;

    // 3seg Contador, 30seg Pregunta, 10seg 
    [HideInInspector]
    public int timer = 3, question = 30, waiting = 5;

    public Canvas canvasInductorLoading, canvasTimerTriviaLoading, canvasQuestionLoading, canvasWaitingTriviaLoading, canvasPodiumStudent;
    public Sprite normalAnswer;

    float timeInductorLoading = 0, timeTimerTrivia = 0, timeTriviaLoading = 0, timeWaitingTrivia = 0;
    string idBuilding = "";
    int index = 0;
    Dictionary<string,object>[] listTrivias;

    //public float GetTimeInductorLoading() { return timeInductorLoading; }
    public void SetTimeInductorLoading(float timeInductorLoading) { this.timeInductorLoading = timeInductorLoading; }

    //public float GetTimeTimerTrivia() { return timeTimerTrivia; }
    public void SetTimeTimerTrivia(float timeTimerTrivia) { this.timeTimerTrivia = timeTimerTrivia; }

    //public float GetTimeTriviaLoading() { return timeTriviaLoading; }
    public void SetTimeTriviaLoading(float timeTriviaLoading) { this.timeTriviaLoading = timeTriviaLoading; }

    public void SetIdTriviaBuilding(string idBuilding){ this.idBuilding = idBuilding; }
    public void SetListTrivias(List<Dictionary<string,object>> listTrivias){ this.listTrivias = listTrivias.ToArray(); }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canvasWaitingTriviaLoading.transform.Find("TimeSlider").GetComponent<Slider>().maxValue = waiting;
    }
    
    void Update()
    {   
        // canvasinductorLoading
        if (canvasInductorLoading.enabled)
            InductorLoading();
        else
            timeInductorLoading = 0;

        // canvasTimerTriviaLoadingLoading
        if (canvasTimerTriviaLoading.enabled)
            TimerTriviaLoading();

        // canvasQuestionLoasing
        if (canvasQuestionLoading.enabled)
            TriviaLoading();

        // canvasWaitingTriviaLoading
        if (canvasWaitingTriviaLoading.enabled)
            WaitingTriviaLoading();

    }

    async void InductorLoading()
    {
        if (timeInductorLoading > 0)
        {
            timeInductorLoading -= Time.deltaTime;
            canvasInductorLoading.transform.Find("TimeLabel").GetComponent<Text>().text = timeInductorLoading.ToString("f0");
        }
        else if (timeInductorLoading < 0)
        {
            canvasInductorLoading.enabled = false;
            await TriviasChallengesManager.instance.PutInductorTriviaChallengeAsync
            (
                await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId()), 
                idBuilding,
                new Dictionary<string, object> () {
                    { "available", false }
                }
            );
        }
    }

    void TimerTriviaLoading()
    {
        index = 0;

        if (timeTimerTrivia > 0)
        {
            timeTimerTrivia -= Time.deltaTime;
            canvasTimerTriviaLoading.transform.Find("LoadCircle").GetComponentInChildren<Text>().text = timeTimerTrivia.ToString("f0");
        }
        else if (timeTimerTrivia < 0)
        {
            timeTimerTrivia = 0;
            canvasTimerTriviaLoading.enabled = false;
            timeTriviaLoading = question;
            ShowTrivia();
        }
    }

    void TriviaLoading()
    {
        if (timeTriviaLoading > 0)
        {
            timeTriviaLoading -= Time.deltaTime;
            canvasQuestionLoading.transform.Find("Timer").GetComponentInChildren<Text>().text = timeTriviaLoading.ToString("f0");
        }
        else if (timeTriviaLoading < 0)
        {
            timeTriviaLoading = 0;
            canvasQuestionLoading.enabled = false;
            timeWaitingTrivia = 0;
            canvasWaitingTriviaLoading.enabled = true;
        }
    }

    void WaitingTriviaLoading()
    {
        if (timeWaitingTrivia < waiting)
        {
            timeWaitingTrivia += Time.deltaTime;
            canvasWaitingTriviaLoading.transform.Find("TimeSlider").GetComponent<Slider>().value = timeWaitingTrivia;

        }
        else if (timeWaitingTrivia > waiting)
        {
            timeWaitingTrivia = 0;
            canvasWaitingTriviaLoading.transform.Find("TimeSlider").GetComponent<Slider>().maxValue = waiting;
            canvasWaitingTriviaLoading.enabled = false;
            timeTriviaLoading = question;

            canvasQuestionLoading.transform.Find("AnswerOne").GetComponent<Button>().interactable = true;
            canvasQuestionLoading.transform.Find("AnswerTwo").GetComponent<Button>().interactable = true;
            canvasQuestionLoading.transform.Find("AnswerThree").GetComponent<Button>().interactable = true;

            canvasQuestionLoading.transform.Find("AnswerOne").GetComponent<Button>().image.sprite = normalAnswer;
            canvasQuestionLoading.transform.Find("AnswerTwo").GetComponent<Button>().image.sprite = normalAnswer;
            canvasQuestionLoading.transform.Find("AnswerThree").GetComponent<Button>().image.sprite = normalAnswer;
            ShowTrivia();
        }
    }

    async void ShowTrivia()
    {
        object buildingName = await DataBaseManager.instance.SearchById("Buildings", idBuilding,"name");
        canvasQuestionLoading.transform.Find("BuildingNameLabel").GetComponent<Text>().text = buildingName.ToString();            

        if(index < listTrivias.Length)
        {
            Dictionary<string, object> trivia = listTrivias[index];        
            foreach(KeyValuePair<string, object> pair in trivia)
            {
                if(pair.Key == "question")
                    canvasQuestionLoading.transform.Find("QuestionFieldLabel").GetComponent<Text>().text = pair.Value.ToString();
                if(pair.Key == "answerOne")
                    canvasQuestionLoading.transform.Find("AnswerOne").GetComponentInChildren<Text>().text = pair.Value.ToString();
                if(pair.Key == "answerTwo")
                    canvasQuestionLoading.transform.Find("AnswerTwo").GetComponentInChildren<Text>().text = pair.Value.ToString();
                if(pair.Key == "answerThree")
                    canvasQuestionLoading.transform.Find("AnswerThree").GetComponentInChildren<Text>().text = pair.Value.ToString();
            }
            index++;
            canvasQuestionLoading.enabled = true;
        }
        else
            ScenesManager.instance.LoadNewCanvas(canvasPodiumStudent);
    }
}
