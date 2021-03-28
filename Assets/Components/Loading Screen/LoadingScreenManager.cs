﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random=System.Random;

public class LoadingScreenManager : MonoBehaviour
{
    private static LoadingScreenManager instance;
    public static LoadingScreenManager Instance { get => instance; set => instance = value; }

    // Contadores
    [HideInInspector]
    public int timer = 3, question = 20;
    [HideInInspector]
    public float isOver = 1.5f, waiting = 1.5f;

    public Canvas canvasInductorLoading, canvasTimerTriviaLoading, canvasQuestionLoading, canvasWaitingTriviaLoading, canvasPodiumStudent, canvasTimeOver, canvasKevinNotification, canvasRankingFinal;
    public Sprite normalAnswer;
    public Text kevinQuote;

    public GameObject[] placeIndividualLabels;
    public GameObject[] placeGrupalLabels;
    
    private List<string> SuccessQuotesList = new List<string>{"Buena esa, crack.", "Eres un Máquina", "Eres un Fiera", "Un Titán", "Caballo"};
    private List<string> FailQuotesList = new List<string>{"Mejor suerte para la próxima, crack.", "Lamenteibol.", "Llórelo papá."};

    float timeInductorLoading = 0, timeTimerTrivia = 0, timeTriviaLoading = 0, timeWaitingTrivia = 0, timeIsOver = 0;
    string idBuilding = "";
    int index = 0;
    Dictionary<string,object>[] listTrivias;

    public void SetTimeInductorLoading(float timeInductorLoading) { this.timeInductorLoading = timeInductorLoading; }
    public void SetTimeTimerTrivia(float timeTimerTrivia) { this.timeTimerTrivia = timeTimerTrivia; }
    public void SetTimeTriviaLoading(float timeTriviaLoading) { this.timeTriviaLoading = timeTriviaLoading; }
    public void SetTimeOverLoading(float timeIsOver) { this.timeIsOver = timeIsOver; }

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
        // Carga del inductor
        if (canvasInductorLoading.enabled)
            InductorLoading();
        else
            timeInductorLoading = 0;

        // Carga del inicio
        if (canvasTimerTriviaLoading.enabled)
            TimerTriviaLoading();

        // Pregunta
        if (canvasQuestionLoading.enabled)
            TriviaLoading();

        // El tiempo se acabó
        if (canvasTimeOver.enabled)
            TimeOverLoading();

        // Puntaje de la pregunta
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
            await TriviasChallengesManager.Instance.PutInductorTriviaChallengeAsync
            (
                GlobalDataManager.Instance.idUserInductor, 
                idBuilding,
                new Dictionary<string, object> () {
                    { "available", false }
                }
            );
        }
    }

    // Carga del inicio
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

    // Pregunta
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
            timeIsOver = isOver;
            kevinQuote.text = "";
            canvasTimeOver.enabled = true;
            canvasKevinNotification.enabled = false;
        }
    }

    // Se acabó el tiempo para responder
    void TimeOverLoading()
    {
        if (timeIsOver > 0)
        {
            timeIsOver -= Time.deltaTime;
        }
        else if (timeIsOver < 0)
        {
            timeIsOver = 0;
            timeWaitingTrivia = 0;
            canvasTimeOver.enabled = false;
            canvasQuestionLoading.enabled = false;
            canvasWaitingTriviaLoading.enabled = true;
        }
    }

    // Carga entre preguntas
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

    // Informacion de la pregunta
    async void ShowTrivia()
    {
        object buildingName = await DataBaseManager.Instance.SearchById("Buildings", idBuilding,"name");
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
        else{
            GameObject.FindObjectOfType<PlayATrivia>().ShowFinalRanking();
            ScenesManager.Instance.LoadNewCanvas(canvasPodiumStudent);
        }
    }

    // Kevin Alerta
    public void SetKevinQuote(bool correctAnswer){
        var random = new Random();
        if(correctAnswer){
            int randomQuote = random.Next(SuccessQuotesList.Count);
            kevinQuote.text = SuccessQuotesList[randomQuote];
        }else{
            int randomQuote = random.Next(FailQuotesList.Count);
            kevinQuote.text = FailQuotesList[randomQuote];
        }
    }

    public async void ShowFinalRankingYincana()
    {
        int sizeRoomsTable = await DataBaseManager.Instance.SizeTable("Rooms");
        int sizeFinishedRoomsTable = await DataBaseManager.Instance.SizeTable("Rooms", "finished", true);
        if (sizeFinishedRoomsTable == sizeRoomsTable && sizeRoomsTable != 0)
        {
            IndividualRanking();
            GroupRanking();
            ScenesManager.Instance.LoadNewCanvas(LoadingScreenManager.Instance.canvasRankingFinal);
        }
    }

    public async void IndividualRanking()
    {
        List<Dictionary<string, object>> rankingList = await DataBaseManager.Instance.SearchByOrderDescendingAndLimit("Students", "score", 3);
        foreach(Dictionary<string, object> ranking in rankingList)
        {
            foreach(KeyValuePair<string, object> pair in ranking)
            {
                if(pair.Key == "name")
                    placeIndividualLabels[rankingList.IndexOf(ranking)].transform.Find("NameLabel").GetComponent<Text>().text = pair.Value.ToString();
                if(pair.Key == "score")
                    placeIndividualLabels[rankingList.IndexOf(ranking)].transform.Find("PointsLabel").GetComponent<Text>().text = pair.Value.ToString();
            }
        }
    }

    public async void GroupRanking()
    {
        List<Dictionary<string, object>> rankingList = await DataBaseManager.Instance.SearchByOrderDescendingAndLimit("Rooms", "score", 3);
        foreach(Dictionary<string, object> ranking in rankingList)
        {
            foreach(KeyValuePair<string, object> pair in ranking)
            {
                if(pair.Key == "room")
                    placeGrupalLabels[rankingList.IndexOf(ranking)].transform.Find("NameLabel").GetComponent<Text>().text = pair.Value.ToString();
                if(pair.Key == "score")
                    placeGrupalLabels[rankingList.IndexOf(ranking)].transform.Find("PointsLabel").GetComponent<Text>().text = pair.Value.ToString();
            }
        }
    }
}
