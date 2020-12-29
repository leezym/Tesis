using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public class Hint
{
    public string name;
    public string description;
    public string answer;
    //public DateTime = new System.DateTime(0);

    public Hint() { }

    public Hint(string name, string description, string answer)
    {
        this.name = name;
        this.description = description;
        this.answer = answer;
        //this.idInductor = idInductor;
    }

    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "name", this.name },
            { "description", this.description },
            { "answer", this.answer },
            //{ "hour", this.hour }
        };
    }
}