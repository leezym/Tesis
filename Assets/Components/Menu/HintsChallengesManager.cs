using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class HintsChallengesManager : MonoBehaviour
{
    public static HintsChallengesManager instance;

    void Awake() {
        instance = this;
    }

    public async Task PostNewHintChallenge(string idRoom, string idHint, int score, int position, string hour)
    {
        HintChallenge element = new HintChallenge(idRoom, idHint, score, position, hour);
        await DataBaseManager.instance.InsertWithoutId("HintsChallenges", element.ConvertJson());
    }

    /*public async Task PutHintChallengeAsync(string idRoom, string idHint, Dictionary<string,object> data)
    {
        await DataBaseManager.instance.UpdateByTwoAttributesAsync("HintsChallenges", "idRoom", idRoom, "idHint", idHint, data);
    }*/

    public async Task<Dictionary<string, object>> GetHintChallengeAsync(string idHintChallenge)
    {
        return await DataBaseManager.instance.SearchById("HintsChallenges", idHintChallenge);
    }

    public async Task<List<Dictionary<string, object>>> GetHintChallengeByIds(string idRoom, string idHint)
    {
        return await DataBaseManager.instance.SearchByTwoAttributes("HintsChallenges", "idRoom", idRoom, "idHint", idHint);
    }

    public async Task DeleteHintChallenge(string idHintChallenge)
    {
        await DataBaseManager.instance.DeleteAsync("HintsChallenges", idHintChallenge);
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
