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

    public Inductor(string id, string username, string email)
    {
        this.id = id;
        this.room = username;
        this.email = email;
    }

    /*public string GetUsername() { return this.username; }
    public string GetEmail() { return this.email; }

    public void SetUsername(string username) { this.username = username; }
    public void SetEmail(string email) { this.email = email; }*/
}
