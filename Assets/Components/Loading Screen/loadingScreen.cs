using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class loadingScreen : MonoBehaviour
{
    public Canvas canvasinductorLoading;
    float initTime = 0, time = 0;

    void Update()
    {   
        if (canvasinductorLoading.enabled)
            InductorLoading();
        else
        {
            initTime = 0;
            time = 0;
        }

        if (time <= 0)
        {
            canvasinductorLoading.enabled = false;
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
}
