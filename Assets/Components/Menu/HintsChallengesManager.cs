using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class HintsChallengesManager : MonoBehaviour
{
    private static HintsChallengesManager instance;
    public static HintsChallengesManager Instance { get => instance; set => instance = value; }

    void Awake() {
        instance = this;
    }

    public async Task PostNewHintChallenge(string idRoom, string idHint, int score, int position, string hour)
    {
        HintChallenge element = new HintChallenge(idRoom, idHint, score, position, hour);
        await DataBaseManager.Instance.InsertWithoutId("HintsChallenges", element.ConvertJson());
    }

    public async Task<Dictionary<string, object>> GetHintChallengeAsync(string idHintChallenge)
    {
        return await DataBaseManager.Instance.SearchById("HintsChallenges", idHintChallenge);
    }

    public async Task<List<Dictionary<string, object>>> GetHintChallengeByRoom(string idRoom)
    {
        return await DataBaseManager.Instance.SearchHintDataByRoom(idRoom);
    }

    public async Task DeleteHintChallenge(string idHintChallenge)
    {
        await DataBaseManager.Instance.DeleteAsync("HintsChallenges", idHintChallenge);
    }
}

public class HintChallenge
{
    public string idRoom;
    public string idHint;
    public int score;
    public int position;
    public string hour;

    public HintChallenge() { }

    public HintChallenge(string idRoom, string idHint, int score, int position, string hour)
    {
        this.idRoom = idRoom;
        this.idHint = idHint;
        this.score = score;
        this.position = position;
        this.hour = hour;
    }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "idRoom", this.idRoom },
            { "idHint", this.idHint },
            { "score", this.score },
            { "position", this.position },
            { "hour", this.hour}
        };
    }
}
