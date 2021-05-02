using System.Collections;
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

    public int timer = 3, question = 20;

    public float isOver = 1.5f, waiting = 1.5f;

    public bool triviaInProgress, finalRanking = true;

    public Canvas canvasInductorLoading, canvasTimerTriviaLoading, canvasQuestionLoading, canvasWaitingTriviaLoading, canvasPodiumStudent, canvasTimeOver, canvasKevinNotification, canvasRankingFinal;
    public Sprite normalAnswer;
    public Text kevinQuote;
    public GameObject backButtonRankingFinalInductor, backButtonRankingFinalEstudiante;

    //public Button backButtonRankingFinal;

    public GameObject[] placeIndividualLabels;
    public GameObject[] placeGrupalLabels;
    
    private List<string> SuccessQuotesList = new List<string>{"Buena esa, crack.", "Eres máquina, sigue así.", "Eres una fiera"};
    private List<string> FailQuotesList = new List<string>{"Mejor suerte para la próxima, crack.", "No te desanimes :c", "Llórelo."};

    float timeInductorLoading = 0, timeTimerTrivia = 0, timeWaitingTrivia = 0, timeIsOver = 0; 
    [HideInInspector]
    public float timeTriviaLoading = 0;
    
    string idBuilding = "";
    int index = 0;
    List<Dictionary<string,object>> listTrivias = new List<Dictionary<string,object>>();

    public void SetTimeInductorLoading(float timeInductorLoading) { this.timeInductorLoading = timeInductorLoading; }
    public void SetTimeTimerTrivia(float timeTimerTrivia) { this.timeTimerTrivia = timeTimerTrivia; }
    public void SetTimeTriviaLoading(float timeTriviaLoading) { this.timeTriviaLoading = timeTriviaLoading; }
    public void SetTimeOverLoading(float timeIsOver) { this.timeIsOver = timeIsOver; }

    public void SetIdTriviaBuilding(string idBuilding){ this.idBuilding = idBuilding; }
    public void SetListTrivias(List<Dictionary<string,object>> listTrivias){ this.listTrivias = listTrivias; }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canvasRankingFinal.enabled = false;

        canvasWaitingTriviaLoading.transform.Find("TimeSlider").GetComponent<Slider>().maxValue = waiting;
        
        // Ranking Final Advertencia
        /*LoadingScreenManager.Instance.backButtonRankingFinal.onClick.AddListener(()=>{
            NotificationsManager.Instance.SetQuestionNotificationMessage("Recuerda tomar captura de las puntuaciones individuales y grupales antes de salir. Una vez se cierra el ranking no puedes abrirlo nuevamente, ¿Seguro que deseas salir?.");
            NotificationsManager.Instance.acceptQuestionButton.onClick.AddListener(()=>{                           
                ScenesManager.Instance.DeleteCurrentCanvas(LoadingScreenManager.Instance.canvasRankingFinal);
                NotificationsManager.Instance.acceptQuestionButton.onClick.RemoveAllListeners();
            });
        });*/
    }
    
    void Update()
    {       
        if(canvasRankingFinal.enabled)
        {
            if(GlobalDataManager.Instance.userType == "student")
            {
                backButtonRankingFinalInductor.SetActive(false);
                backButtonRankingFinalEstudiante.SetActive(true);
            }
            else
            {
                backButtonRankingFinalInductor.SetActive(true);
                backButtonRankingFinalEstudiante.SetActive(false);
            }
        }
        
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

    void InductorLoading()
    {
        if (timeInductorLoading > 0)
        {
            timeInductorLoading -= Time.deltaTime;
            canvasInductorLoading.transform.Find("TimeLabel").GetComponent<Text>().text = timeInductorLoading.ToString("f0");
        }
        else if (timeInductorLoading < 0)
        {
            canvasInductorLoading.enabled = false;
            /*await TriviasChallengesManager.Instance.PutInductorTriviaChallengeAsync
            (
                GlobalDataManager.Instance.idUserInductor, 
                idBuilding,
                new Dictionary<string, object> () {
                    { "available", false }
                }
            );*/
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
            GlobalDataManager.Instance.lose.Play();
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
            GameObject.FindObjectOfType<PlayATrivia>().DetectAnswer();
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
            ShowTrivia();
        }
    }

    // Informacion de la pregunta
    void ShowTrivia()
    {
        // Configurar Pregunta 
        canvasQuestionLoading.transform.Find("AnswerOne").GetComponent<Button>().interactable = true;
        canvasQuestionLoading.transform.Find("AnswerTwo").GetComponent<Button>().interactable = true;
        canvasQuestionLoading.transform.Find("AnswerThree").GetComponent<Button>().interactable = true;

        canvasQuestionLoading.transform.Find("AnswerOne").GetComponent<Button>().image.sprite = normalAnswer;
        canvasQuestionLoading.transform.Find("AnswerTwo").GetComponent<Button>().image.sprite = normalAnswer;
        canvasQuestionLoading.transform.Find("AnswerThree").GetComponent<Button>().image.sprite = normalAnswer;

        string buildingName = "";

        if(idBuilding == GlobalDataManager.idBuildingPalmas)
        {
            buildingName = GlobalDataManager.nameBuildingPalmas;
        }
        else if(idBuilding == GlobalDataManager.idBuildingGuayacanes)
        {
            buildingName = GlobalDataManager.nameBuildingGuayacanes;
        }
        else if(idBuilding == GlobalDataManager.idBuildingLago)
        {
            buildingName = GlobalDataManager.nameBuildingLago;
        }
        else if(idBuilding == GlobalDataManager.idBuildingRaulPosada)
        {
            buildingName = GlobalDataManager.nameBuildingRaulPosada;
        }

        canvasQuestionLoading.transform.Find("BuildingNameLabel").GetComponent<Text>().text = buildingName;
        if(index < listTrivias.Count)
        {  
            foreach(KeyValuePair<string, object> pair in listTrivias[index])
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
        {
            triviaInProgress = false;
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
            GlobalDataManager.Instance.win.Play();
        }else{
            int randomQuote = random.Next(FailQuotesList.Count);
            kevinQuote.text = FailQuotesList[randomQuote];
            GlobalDataManager.Instance.lose.Play();
        }
    }

    public async void ShowFinalRankingYincana()
    {
        int sizeRoomsTable = await DataBaseManager.Instance.SizeTable("Rooms");
        int sizeFinishedRoomsTable = await DataBaseManager.Instance.SizeTable("Rooms", "finished", true);
        if (sizeFinishedRoomsTable == sizeRoomsTable && sizeRoomsTable != 0 && finalRanking)
        {
            IndividualRanking();
            GroupRanking();
            ScenesManager.Instance.LoadNewCanvas(LoadingScreenManager.Instance.canvasRankingFinal);
            finalRanking = false;
            
            /*if(GlobalDataManager.Instance.userType == "inductor")
                NotificationsManager.Instance.SetFailureNotificationMessage("Recuerda tomar captura de las puntuaciones individuales y grupales antes de salir. ¡No queremos que nuestros neojaverianos se pongan tristes!");
            else
                NotificationsManager.Instance.SetFailureNotificationMessage("Si lo deseas, puedes tomar captura de las puntuaciones individuales y grupales antes de salir.");*/
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
