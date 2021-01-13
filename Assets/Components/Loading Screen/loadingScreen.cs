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
            InductorLoading();
            if (time <= 0)
                canvasinductorLoading.enabled = false;  
        }
        else
        {
            initTime = 0;
            time = 0;
        }        

        // canvasTimerTrivia
        if (canvasTimerTrivia.enabled)
        {
            TimerTriviaLoading();
            if (time <= 0)
                canvasTimerTrivia.enabled = false;  
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
            canvasTimerTrivia.transform.Find("TimeLabel").GetComponent<Text>().text = time.ToString("f0"); //PENDIENTE
        }
    }
}
