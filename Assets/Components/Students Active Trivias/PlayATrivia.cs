using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PlayATrivia : MonoBehaviour
{
    public Canvas canvasPodiumStudent;
    public Button[] answerButtons;
    public Text question, finalScore;
    public Sprite wrongAnswer, rightAnswer;
    public GameObject[] placeLabels;
    string idStudent = "";

    public async void DetectAnswer(Button button)
    {
        string answer = button.GetComponentInChildren<Text>().text;
        
        string idTrivia = await TriviasManager.Instance.GetIdTriviaByQuestion(question.text);
        object correctAnswer = await DataBaseManager.Instance.SearchAttribute("Trivias", idTrivia, "correctAnswer");

        if(answer == correctAnswer.ToString())
        {
            button.image.sprite = rightAnswer;
            LoadingScreenManager.Instance.SetKevinQuote(true);
            RegisterPoints(idTrivia);
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
        }
        ScenesManager.Instance.LoadNewCanvas(LoadingScreenManager.Instance.canvasKevinNotification);
    }

    async void RegisterPoints(string idTrivia)
    {
        idStudent = AuthManager.Instance.GetUserId();

        // Crear trivia resuelta
        object points = await DataBaseManager.Instance.SearchAttribute("Trivias", idTrivia, "points");
        await TriviasChallengesManager.Instance.PostNewStudentTriviaChallenge(idStudent, idTrivia, Convert.ToInt32(points));
        
        // Asignar puntuaciones totales al estudiante
        object currentScore = await DataBaseManager.Instance.SearchAttribute("Students", idStudent, "score");
        int score = Convert.ToInt32(currentScore) + Convert.ToInt32(points);
        await UsersManager.Instance.PutUserAsync("Students", idStudent, new Dictionary<string, object>{
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
        object idRoom = await DataBaseManager.Instance.SearchAttribute("Students", idStudent, "idRoom");
        List<Dictionary<string, object>> rankingList = await UsersManager.Instance.GetFinalTriviasRanking(idRoom.ToString());
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
