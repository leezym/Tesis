using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TriviasChallengesManager : MonoBehaviour
{
    public static TriviasChallengesManager instance;
    void Awake() 
    {
        instance = this;
    }

    public async Task PostNewInductorTriviaChallenge(string idInductor, string idBuilding, bool available)
    {
        InductorTriviaChallenge element = new InductorTriviaChallenge(idInductor, idBuilding, available);
        await DataBaseManager.instance.InsertWithoutId("InductorTriviasChallenges", element.ConvertJson());
    }

    public async Task PostNewStudentTriviaChallenge(string idStudent, string idTrivia, int score)
    {
        StudentTriviaChallenge element = new StudentTriviaChallenge(idStudent, idTrivia, score);
        await DataBaseManager.instance.InsertWithoutId("StudentTriviasChallenges", element.ConvertJson());
    }

    public async Task PutInductorTriviaChallengeAsync(string idInductor, string idBuilding, Dictionary<string,object> data)
    {
        string idTriviaChallenge = await DataBaseManager.instance.SearchId("InductorTriviasChallenges", "idInductor", idInductor, "idBuilding", idBuilding);
        await DataBaseManager.instance.UpdateAsync("InductorTriviasChallenges", idTriviaChallenge, data);
    }

    public async Task<Dictionary<string, object>> GetTriviaChallengeAsync(string db, string idHintChallenge)
    {
        return await DataBaseManager.instance.SearchById(db, idHintChallenge);
    }

    public async Task<List<Dictionary<string, object>>> GetTriviaChallengeByBuilding(string idInductor)
    {
        return await DataBaseManager.instance.SearchTriviaDataByBuilding(idInductor);
    }

    public async Task DeleteTriviaChallenge(string db, string idHintChallenge)
    {
        await DataBaseManager.instance.DeleteAsync(db, idHintChallenge);
    }
}

public class InductorTriviaChallenge
{
    public string idInductor;
    public string idBuilding;
    public bool available = true;

    public InductorTriviaChallenge() { }

    public InductorTriviaChallenge(string idInductor, string idBuilding, bool available)
    {
        this.idInductor = idInductor;
        this.idBuilding = idBuilding;
        this.available = available;
    }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "idInductor", this.idInductor },
            { "idBuilding", this.idBuilding },
            { "available", this.available }
        };
    }
}

public class StudentTriviaChallenge
{
    public string idStudent;
    public string idTrivia;
    public int score = 0;

    public StudentTriviaChallenge() { }

    public StudentTriviaChallenge(string idStudent, string idTrivia, int score)
    {
        this.idStudent = idStudent;
        this.idTrivia = idTrivia;
        this.score = score;
    }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "idStudent", this.idStudent },
            { "idTrivia", this.idTrivia },
            { "score", this.score }
        };
    }
}
