using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class loadingScreen : MonoBehaviour
{
    public Canvas canvasinductorLoading, canvasTimerTrivia;
    float initTime = 0, time = 0;

    void Update()
    {   
        // canvasinductorLoading
        if (canvasinductorLoading.enabled)
        {
            Debug.Log("inductorloading");
            InductorLoading();
        }
        else
        {
            initTime = 0;
            time = 0;
        }        

        // canvasTimerTrivia
        if (canvasTimerTrivia.enabled)
        {
            Debug.Log("timer");
            TimerTriviaLoading();             
        }
        else
        {
            initTime = 0;
            time = 0;
        }

    }

    void InductorLoading()
    {
        if (initTime == 0)
        {
            initTime = (float)Convert.ToDecimal(canvasinductorLoading.transform.Find("TimeLabel").GetComponent<Text>().text);
            time = initTime;
        }

        if (time > 0)
        {
            time -= Time.deltaTime;
            canvasinductorLoading.transform.Find("TimeLabel").GetComponent<Text>().text = time.ToString("f0");
        }
        else
            canvasinductorLoading.enabled = false;
    }

    void TimerTriviaLoading()
    {
        if (initTime == 0)
        {
            initTime = 3;
            time = initTime;
        }

        if (time > 0)
        {
            time -= Time.deltaTime;
            //canvasTimerTrivia.transform.Find("TimeLabel").GetComponent<Text>().text = time.ToString("f0"); //PENDIENTE
        }
        else
            canvasTimerTrivia.enabled = false;
    }
}
