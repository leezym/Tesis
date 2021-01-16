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

    public async void DetectAnswer(Button button)
    {
        string answer = button.GetComponentInChildren<Text>().text;
        
        string idTrivia = await TriviasManager.instance.GetIdTriviaByQuestion(question.text);
        object correctAnswer = await DataBaseManager.instance.SearchAttribute("Trivias", idTrivia, "correctAnswer");

        if(answer == correctAnswer.ToString())
        {
            button.image.sprite = rightAnswer;
            RegisterPoints(idTrivia);
        }
        else
        {
            button.image.sprite = wrongAnswer;
            foreach(Button answerButton in answerButtons)
            {
                if(answerButton.GetComponentInChildren<Text>().text == correctAnswer.ToString())
                    answerButton.image.sprite = rightAnswer;
            }
        }

        // Inhabilitar botones
        foreach(Button answerButton in answerButtons)
        {
            answerButton.interactable = false;
        }
    }

    async void RegisterPoints(string idTrivia)
    {
        string idStudent = AuthManager.instance.GetUserId();

        // Crear trivia resuelta
        object points = await DataBaseManager.instance.SearchAttribute("Trivias", idTrivia, "points");
        await TriviasChallengesManager.instance.PostNewStudentTriviaChallenge(idStudent, idTrivia, Convert.ToInt32(points));
        
        // Asignar puntuaciones totales al estudiante
        object currentScore = await DataBaseManager.instance.SearchAttribute("Students", idStudent, "score");
        int score = Convert.ToInt32(currentScore) + Convert.ToInt32(points);
        await UsersManager.instance.PutUserAsync("Students", idStudent, new Dictionary<string, object>{
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
        List<Dictionary<string, object>> rankingList = await UsersManager.instance.GetFinalTriviasRanking();
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
