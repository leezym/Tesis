using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

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
        string body = JsonUtility.ToJson(user);
        DataBaseManager.instace.InsertUser("Inductors", uid, body);
    }

    public void PostNewStudent(string uid, string name, string document)
    {
        Student user = new Student(uid, name, document);
        string body = JsonUtility.ToJson(user);
        DataBaseManager.instace.InsertUser("Students", uid, body);
    }

    public void PutUser(string db, string userId, string atribute, string value)
    {
        DataBaseManager.instace.UpdateUser(db, userId, atribute, value);
    }

    public DatabaseReference GetUser(string db)
    {
        return DataBaseManager.instace.SelectUserById(db);
    }

}

public class Inductor
{
    public string id;
    public string room;
    public string email;

    public Inductor() { }

    public Inductor(string id, string room, string email)
    {
        this.id = id;
        this.room = room;
        this.email = email;
    }
}

public class Student
{
    public string id;
    public string name;
    public string document;

    public Student() { }

    public Student(string id, string name, string document)
    {
        this.id = id;
        this.name = name;
        this.document = document;
    }
}

