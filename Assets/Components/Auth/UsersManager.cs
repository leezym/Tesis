﻿using System.Collections;
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

    public async Task PostNewInductor(string uid, string user, string email, string name)
    {
        Inductor element = new Inductor(uid, user, email, name);
        await DataBaseManager.instance.InsertWithoutId("Inductors", element.ConvertJson());
    }

    public async Task PostNewStudent(string uid, string name, string document, string idRoom)
    {
        Student element = new Student(name, document, idRoom);
        await DataBaseManager.instance.InsertWithId("Students", uid, element.ConvertJson());
    }

    public async Task PutUserAsync(string db, string userId, Dictionary<string,object> data)
    {
        await DataBaseManager.instance.UpdateAsync(db, userId, data);
    }

    public async Task<Dictionary<string, object>> GetUserAsync(string db, string userId)
    {
        return await DataBaseManager.instance.SearchById(db, userId);
    }

    public async Task<string> GetInductorIdByAuth(string idAuth)
    {
        return await DataBaseManager.instance.SearchId("Inductors", "idAuth", idAuth);
    }

    public async Task<bool> ExistUserByDocument(string db, string document)
    {
        List<Dictionary<string, object>> data = await DataBaseManager.instance.SearchByAttribute(db, "document", document);
        if (data.Count != 0)
        {
            return true;
        }
        return false;
    }

    public async Task DeleteAsync(string db, string userId) 
    {
        await DataBaseManager.instance.DeleteAsync(db, userId);
    }    

    public async Task DeleteSession(string idInductor) 
    {
        await DataBaseManager.instance.DeleteSession(idInductor);
    }
}

public class Inductor
{
    public string idAuth;
    public string user;
    public string email;
    public string name;

    public Inductor() { }

    public Inductor(string idAuth, string user, string email, string name)
    {
        this.idAuth = idAuth;
        this.user = user;
        this.email = email;
        this.name = name;
    }
    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "idAuth", this.idAuth },
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

