using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class HintsManager : MonoBehaviour
{
    public static HintsManager instance;

    void Awake() {
        instance = this;
    }

    public void PostNewHint(string name, string description, string answer)
    {
        Hint element = new Hint(name, description, answer);
        DataBaseManager.instance.InsertWithoutId("Hints", element.ConvertJson());
    }

    public async Task PutHintAsync(string nameHint, Dictionary<string,object> data)
    {
        string idHint = await DataBaseManager.instance.SearchId("Hints", "name", nameHint);
        await DataBaseManager.instance.UpdateAsync("Hints", idHint, data);
    }

    public async Task<Dictionary<string, object>> GetHintAsync(string idHint)
    {
        return await DataBaseManager.instance.SearchById("Hints", idHint);
    }

    public async Task<Dictionary<string, object>> GetHintByName(string name)
    {
        return await DataBaseManager.instance.SearchByAttribute("Hints", "name", name);
    }

    public async Task DeleteHint(string idHint)
    {
        await DataBaseManager.instance.DeleteAsync("Hints", idHint);
    }
}

public class Hint
{
    public string name;
    public string description;
    public string answer;
    public string hour = "";
    public int score;
    public int position;

    public Hint() { }

    public Hint(string name, string description, string answer)
    {
        this.name = name;
        this.description = description;
        this.answer = answer;
    }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "name", this.name },
            { "description", this.description },
            { "answer", this.answer },
            { "hour", this.hour },
            { "score", this.score },
            { "position", this.position }
        };
    }
}