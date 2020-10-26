using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UsersManager : MonoBehaviour
{
    public static UsersManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PostNewInductor(string uid, string room, string email)
    {
        Inductor user = new Inductor(uid, room, email);
        //string body = JsonUtility.ToJson(user);
        DataBaseManager.instance.InsertUser("Inductors", uid, user.ConvertJson());
    }

    public void PostNewStudent(string uid, string name, string document)
    {
        Student user = new Student(uid, name, document);
        //string body = JsonUtility.ToJson(user);
        DataBaseManager.instance.InsertUser("Students", uid, user.ConvertJson());
    }

    public async Task PutUserAsync(string db, string userId, string atribute, string value)
    {
        await DataBaseManager.instance.UpdateUserAsync(db, userId, atribute, value);
    }

    public async Task<Dictionary<string, object>> GetUserAsync(string db, string userId)
    {
        return await DataBaseManager.instance.SelectUserByIdAsync(db, userId);
    }

    public async Task DeleteUserAsync(string db, string userId) 
    {
        await DataBaseManager.instance.DeleteUserAsync(db, userId);

    }

    public async Task SearchDataAsync(string db) 
    {
        await DataBaseManager.instance.SearchDataAsync(db);
    }
    
}

public class Inductor
{
    public string room;
    public string email;

    public Inductor() { }

    public Inductor(string id, string room, string email)
    {
        this.room = room;
        this.email = email;
    }
    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "room", this.room },
            { "email", this.email }
        };
    }
}

public class Student
{
    public string name;
    public string document;

    public Student() { }

    public Student(string id, string name, string document)
    {
        this.name = name;
        this.document = document;
    }

    public Dictionary<string, object> ConvertJson(){
        return new Dictionary<string, object>() {
            { "name", this.name },
            { "document", this.document }
        };
    }
}

