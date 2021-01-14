using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager instance;

    public Canvas canvasInductorLoading, canvasTimerTrivia;
    float timeInductorLoading = 0, timeTimerTrivia = 0;
    string idBuilding = "";

    public float GetTimeInductorLoading() { return timeInductorLoading; }
    public void SetTimeInductorLoading(float timeInductorLoading) { this.timeInductorLoading = timeInductorLoading; }

    public float GetTimeTimerTrivia() { return timeTimerTrivia; }
    public void SetTimeTimerTrivia(float timeTimerTrivia) { this.timeTimerTrivia = timeTimerTrivia; }

    public void SetIdTriviaBuilding(string idBuilding){ this.idBuilding = idBuilding; }

    void Awake()
    {
        instance = this;
    }
    
    void Update()
    {   
        // canvasinductorLoading
        if (canvasInductorLoading.enabled)
            InductorLoading();
        else
            timeInductorLoading = 0;

        // canvasTimerTrivia
        if (canvasTimerTrivia.enabled)
            TimerTriviaLoading();
        else
            timeTimerTrivia = 0;

    }

    async void InductorLoading()
    {
        if (timeInductorLoading > 0)
        {
            timeInductorLoading -= Time.deltaTime;
            canvasInductorLoading.transform.Find("TimeLabel").GetComponent<Text>().text = timeInductorLoading.ToString("f0");
        }
        else
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
        if (timeTimerTrivia > 0)
        {
            timeTimerTrivia -= Time.deltaTime;
            //canvasTimerTrivia.transform.Find("TimeLabel").GetComponent<Text>().text = time.ToString("f0"); //PENDIENTE
        }
        else
        {
            canvasTimerTrivia.enabled = false;
            Debug.Log("Aqui comienza la trivia");
        }
    }
}
