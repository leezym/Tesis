using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TriviasManager : MonoBehaviour
{
    public static TriviasManager instance;

    void Awake() {
        instance = this;
    }

    public async Task PostNewTrivia(string idBuilding, string question, string answerOne, string answerTwo, string answerThree, string correctAnswer)
    {
        Trivia element = new Trivia(idBuilding, question, answerOne, answerTwo, answerThree, correctAnswer);
        await DataBaseManager.instance.InsertWithoutId("Trivias", element.ConvertJson());
    }

    public async Task PutTriviaAsync(string idTrivia, Dictionary<string,object> data)
    {
        //string idTrivia = await DataBaseManager.instance.SearchId("Trivias", "name", nameTrivia);
        await DataBaseManager.instance.UpdateAsync("Trivias", idTrivia, data);
    }

    public async Task<Dictionary<string, object>> GetTriviaAsync(string idTrivia)
    {
        return await DataBaseManager.instance.SearchById("Trivias", idTrivia);
    }

    public async Task<List<Dictionary<string, object>>> GetTriviaByBuilding(string name)
    {
        string idBuilding = await DataBaseManager.instance.SearchId("Buildings", "name", name);
        return await DataBaseManager.instance.SearchByAttribute("Trivias", "idBuilding", idBuilding);
    }

    public async Task<List<Dictionary<string, object>>> GetTriviaByQuestion(string question)
    {
        return await DataBaseManager.instance.SearchByAttribute("Trivias", "question", question);
    }

    public async Task DeleteTrivia(string idTrivia)
    {
        await DataBaseManager.instance.DeleteAsync("Trivias", idTrivia);
    }
}

public class Trivia
{
    public string idBuilding;
    public string question;
    public int score = 0;
    public string answerOne;
    public string answerTwo;
    public string answerThree;
    public string correctAnswer;

    public Trivia() { }

    public Trivia(string idBuilding, string question, string answerOne, string answerTwo, string answerThree, string correctAnswer)
    {
        this.idBuilding = idBuilding;
        this.question = question;
        this.answerOne = answerOne;
        this.answerTwo = answerTwo;
        this.answerThree = answerThree;
        this.correctAnswer = correctAnswer;
    }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "idBuilding", this.idBuilding },
            { "question", this.question },
            { "answerOne", this.answerOne },
            { "answerTwo", this.answerTwo },
            { "answerThree", this.answerThree },
            { "correctAnswer", this.correctAnswer },
            { "score", this.score }
        };
    }
}