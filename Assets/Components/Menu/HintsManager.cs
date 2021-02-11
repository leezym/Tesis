using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class HintsManager : MonoBehaviour
{
    public static HintsManager instance;
    public static HintsManager Instance { get => instance; set => instance = value; }

    void Awake() {
        instance = this;
    }

    public async Task PostNewHint(string name, string description, string answer)
    {
        Hint element = new Hint(name, description, answer);
        await DataBaseManager.Instance.InsertWithoutId("Hints", element.ConvertJson());
    }

    public async Task PutHintAsync(string idHint, Dictionary<string,object> data)
    {
        await DataBaseManager.Instance.UpdateAsync("Hints", idHint, data);
    }

    public async Task<Dictionary<string, object>> GetHintAsync(string idHint)
    {
        return await DataBaseManager.Instance.SearchById("Hints", idHint);
    }

    public async Task<List<Dictionary<string, object>>> GetHintByName(string name)
    {
        return await DataBaseManager.Instance.SearchByAttribute("Hints", "name", name);
    }

    public async Task<string> GetIdHintByName(string name)
    {
        return await DataBaseManager.Instance.SearchId("Hints", "name", name);
    }

    public async Task DeleteHint(string idHint)
    {
        await DataBaseManager.Instance.DeleteAsync("Hints", idHint);
    }
}

public class Hint
{
    public string name;
    public string description;
    public string answer;

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
            { "answer", this.answer }
        };
    }
}