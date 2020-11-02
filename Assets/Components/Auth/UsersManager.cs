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

    public void PostNewInductor(string uid, string user, string email)
    {
        Inductor element = new Inductor(user, email);
        DataBaseManager.instance.InsertWithId("Inductors", uid, element.ConvertJson());
    }

    public void PostNewStudent(string uid, string name, string document, string iduser)
    {
        Student element = new Student(name, document, iduser);
        DataBaseManager.instance.InsertWithId("Students", uid, element.ConvertJson());
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
}

public class Inductor
{
    public string user;
    public string email;

    public Inductor() { }

    public Inductor(string user, string email)
    {
        this.user = user;
        this.email = email;
    }
    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "user", this.user },
            { "email", this.email }
        };
    }
}

public class Student
{
    public string name;
    public string document;
    public string iduser;

    public Student() { }

    public Student(string name, string document, string iduser)
    {
        this.name = name;
        this.document = document;
        this.iduser = iduser;
    }

    public Dictionary<string, object> ConvertJson(){
        return new Dictionary<string, object>() {
            { "name", this.name },
            { "document", this.document },
            { "iduser", this.iduser }
        };
    }
}

