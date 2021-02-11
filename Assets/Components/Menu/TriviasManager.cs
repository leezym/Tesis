using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TriviasManager : MonoBehaviour
{
    public static TriviasManager instance;
    public static TriviasManager Instance { get => instance; set => instance = value; }

    void Awake() {
        instance = this;
    }

    public async Task PostNewTrivia(string idBuilding, string question, string answerOne, string answerTwo, string answerThree, string correctAnswer)
    {
        Trivia element = new Trivia(idBuilding, question, answerOne, answerTwo, answerThree, correctAnswer);
        await DataBaseManager.Instance.InsertWithoutId("Trivias", element.ConvertJson());
    }

    public async Task PutTriviaAsync(string idTrivia, Dictionary<string,object> data)
    {
        await DataBaseManager.Instance.UpdateAsync("Trivias", idTrivia, data);
    }

    public async Task<Dictionary<string, object>> GetTriviaAsync(string idTrivia)
    {
        return await DataBaseManager.Instance.SearchById("Trivias", idTrivia);
    }

    public async Task<string> GetIdTriviaByQuestion(string question)
    {
        return await DataBaseManager.Instance.SearchId("Trivias", "question", question);
    }

    public async Task<List<Dictionary<string, object>>> GetTriviaByBuilding(string name)
    {
        string idBuilding = await DataBaseManager.Instance.SearchId("Buildings", "name", name);
        return await DataBaseManager.Instance.SearchByAttribute("Trivias", "idBuilding", idBuilding);
    }

    public async Task<List<Dictionary<string, object>>> GetTriviaByIdBuilding(string idBuilding)
    {
        return await DataBaseManager.Instance.SearchByAttribute("Trivias", "idBuilding", idBuilding);
    }

    public async Task<List<Dictionary<string, object>>> GetTriviaByQuestion(string question)
    {
        return await DataBaseManager.Instance.SearchByAttribute("Trivias", "question", question);
    }

    public async Task DeleteTrivia(string idTrivia)
    {
        await DataBaseManager.Instance.DeleteAsync("Trivias", idTrivia);
    }
}

public class Trivia
{
    public string idBuilding;
    public string question;
    public string answerOne;
    public string answerTwo;
    public string answerThree;
    public string correctAnswer;
    public int points = 10;
    public bool available = true;

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
            { "points", this.points },
            { "available", this.available }
        };
    }
}