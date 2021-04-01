using Random=System.Random;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class KevinNotification : MonoBehaviour
{
    public static KevinNotification instance;
    public static KevinNotification Instance { get => instance; set => instance = value; }

    public Canvas canvasKevinQuotes;
    public Text kevinQuote;

    public int correctAnswer;
    private string quote;

    private List<string> SuccessQuotesList = new List<string>{"Buena esa, crack.", "Eres un Máquina", "Eres un Fiera", "Un Titán", "Caballo"};
    private List<string> FailQuotesList = new List<string>{"Mejor suerte para la próxima, crack.", "Lamenteibol.", "Llórelo papá."};


    void awake()
    {
        instance = this;
    }

    private void Start()
    {
        kevinQuote.text = "";
        correctAnswer = 0; //cambiar cuando ya se acomode lo de las respuestas correctas e incorrectas
        quote = "";
    }

    private void Update()
    {
        if(kevinQuote.text != ""){
            LoadKevinQuoteCanvas();
        }
    }

    public void LoadKevinQuoteCanvas(){
        canvasKevinQuotes.enabled = false;
        canvasKevinQuotes.enabled = true;
        ScenesManager.Instance.LoadNewCanvas(canvasKevinQuotes);
    }
}