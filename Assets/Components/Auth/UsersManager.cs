using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UsersManager : MonoBehaviour
{
    public static UsersManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void PostNewInductor(string uid, string user, string email, string name)
    {
        Inductor element = new Inductor(user, email, name);
        DataBaseManager.instance.InsertWithId("Inductors", uid, element.ConvertJson());
    }

    public void PostNewStudent(string uid, string name, string document, string idRoom)
    {
        Student element = new Student(name, document, idRoom);
        DataBaseManager.instance.InsertWithId("Students", uid, element.ConvertJson());
    }

    public async Task PutUserAsync(string db, string userId, Dictionary<string,object> data)
    {
        await DataBaseManager.instance.UpdateAsync(db, userId, data);
    }

    public async Task<Dictionary<string, object>> GetUserAsync(string db, string userId)
    {
        return await DataBaseManager.instance.SearchById(db, userId);
    }

    public async Task<bool> ExistUserByDocument(string db, string document)
    {
        return await DataBaseManager.instance.SearchByDocument(db, document);
    }

    public async Task DeleteUserAsync(string db, string userId) 
    {
        await DataBaseManager.instance.DeleteUserAsync(db, userId);

    }    

    public async Task DeleteSession(string idInductor) 
    {
        await DataBaseManager.instance.DeleteSession(idInductor);

    }
}

public class Inductor
{
    public string user;
    public string email;
    public string name;

    public Inductor() { }

    public Inductor(string user, string email, string name)
    {
        this.user = user;
        this.email = email;
        this.name = name;
    }
    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "user", this.user },
            { "email", this.email },
            { "name", this.name }
        };
    }
}

public class Student
{
    public string name;
    public string document;
    public string idRoom;

    public Student() { }

    public Student(string name, string document, string idRoom)
    {
        this.name = name;
        this.document = document;
        this.idRoom = idRoom;
    }

    public Dictionary<string, object> ConvertJson(){
        return new Dictionary<string, object>() {
            { "name", this.name },
            { "document", this.document },
            { "idRoom", this.idRoom }
        };
    }
}

