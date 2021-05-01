using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PlayATrivia : MonoBehaviour
{
    [Header ("STUDENT")]
    public Canvas canvasPodiumStudent;
    public Button[] answerButtons;
    public Text question, finalScore;
    public Sprite wrongAnswer, rightAnswer;
    public GameObject[] placeLabels;

    [HideInInspector]
    public int points = 0;
    string correctAnswer = "";


    public async void DetectAnswer(Button button)
    {
        string answer = button.GetComponentInChildren<Text>().text;        
        string idTrivia = await TriviasManager.Instance.GetIdTriviaByQuestion(question.text);


        Dictionary<string, object> trivia = await TriviasManager.Instance.GetTriviaAsync(idTrivia);        
        foreach(KeyValuePair<string, object> pair in trivia)
        {
            if(pair.Key == "correctAnswer")
                correctAnswer = pair.Value.ToString();
            else if(pair.Key == "points")
                points = Convert.ToInt32(pair.Value);
        }

        if(answer == correctAnswer.ToString())
        {
            button.image.sprite = rightAnswer;
            LoadingScreenManager.Instance.SetKevinQuote(true);
            points += Convert.ToInt32(LoadingScreenManager.Instance.timeTriviaLoading);
        }
        else
        {
            button.image.sprite = wrongAnswer;
            LoadingScreenManager.Instance.SetKevinQuote(false);
            foreach(Button answerButton in answerButtons)
            {
                if(answerButton.GetComponentInChildren<Text>().text == correctAnswer.ToString())
                    answerButton.image.sprite = rightAnswer;
            }
            points = 0;
        }
        RegisterPoints(idTrivia);
        ScenesManager.Instance.LoadNewCanvas(LoadingScreenManager.Instance.canvasKevinNotification);
    }

    public async void DetectAnswer()
    {    
        string idTrivia = await TriviasManager.Instance.GetIdTriviaByQuestion(question.text);
        points = 0;
        RegisterPoints(idTrivia);
    }

    async void RegisterPoints(string idTrivia)
    {
        // Crear trivia resuelta
        await TriviasChallengesManager.Instance.PostNewStudentTriviaChallenge(GlobalDataManager.Instance.idUserStudent, idTrivia, points);
        
        // Asignar puntuaciones totales al estudiante
        object currentScore = await DataBaseManager.Instance.SearchAttribute("Students", GlobalDataManager.Instance.idUserStudent, "score");
        int score = Convert.ToInt32(currentScore) + Convert.ToInt32(points);
        await UsersManager.Instance.PutUserAsync("Students", GlobalDataManager.Instance.idUserStudent, new Dictionary<string, object>{
            {"score", score}
        });

        // Mostrar score
        if (score >= 0 && score <= 9)
            finalScore.text = "000"+score.ToString();
        else if (score >= 10 && score <= 99)
            finalScore.text = "00"+score.ToString();
        else if (score >= 100 && score <= 999)
            finalScore.text = "0"+score.ToString();
        else if (score >= 1000 && score <= 9999)
            finalScore.text = score.ToString();
            
    }

    public async void ShowFinalRanking()
    {
        List<Dictionary<string, object>> rankingList = await UsersManager.Instance.GetFinalTriviasRanking(GlobalDataManager.Instance.idRoomByStudent);
        foreach(Dictionary<string, object> ranking in rankingList)
        {
            foreach(KeyValuePair<string, object> pair in ranking)
            {
                if(pair.Key == "name")
                    placeLabels[rankingList.IndexOf(ranking)].transform.Find("NameLabel").GetComponent<Text>().text = pair.Value.ToString();
                if(pair.Key == "score")
                    placeLabels[rankingList.IndexOf(ranking)].transform.Find("ScoreLabel").GetComponent<Text>().text = pair.Value.ToString();
            }
        }
    }
}
