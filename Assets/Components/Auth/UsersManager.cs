using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UsersManager : MonoBehaviour
{
    private static UsersManager instance;
    public static UsersManager Instance { get => instance; set => instance = value; }

    public void Awake()
    {
        instance = this;
    }

    public async Task PostNewInductor(string uid, string name, string user)
    {
        Inductor element = new Inductor(uid, name, user);
        await DataBaseManager.Instance.InsertWithoutId("Inductors", element.ConvertJson());
    }

    public async Task PostNewStudent(string uid, string name, string document, string idRoom)
    {
        Student element = new Student(name, document, idRoom);
        await DataBaseManager.Instance.InsertWithId("Students", uid, element.ConvertJson());
    }

    public async Task PutUserAsync(string db, string userId, Dictionary<string,object> data)
    {
        await DataBaseManager.Instance.UpdateAsync(db, userId, data);
    }

    public async Task<Dictionary<string, object>> GetUserAsync(string db, string userId)
    {
        return await DataBaseManager.Instance.SearchById(db, userId);
    }

    public async Task<string> GetInductorIdByAuth(string idAuth)
    {
        return await DataBaseManager.Instance.SearchId("Inductors", "idAuth", idAuth);
    }

    public async Task<List<Dictionary<string, object>>> GetStudentsByOrderOfScore()
    {
        return await DataBaseManager.Instance.SearchByOrderDescending("Students", "score");
    }

    public async Task<List<Dictionary<string, object>>> GetFinalTriviasRanking(string idRoom)
    {
        return await DataBaseManager.Instance.SearchByOrderDescendingAndLimit("Students", "score", 3, "idRoom", idRoom);
    }

    public async Task<bool> ExistUserByDocument(string db, string document)
    {
        List<Dictionary<string, object>> data = await DataBaseManager.Instance.SearchByAttribute(db, "document", document);
        if (data.Count != 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> ExistUserByUser(string db, string user)
    {
        List<Dictionary<string, object>> data = await DataBaseManager.Instance.SearchByAttribute(db, "user", user);
        if (data.Count != 0)
        {
            return true;
        }
        return false;
    }

    /*public async Task DeleteAsync(string db, string userId) 
    {
        await DataBaseManager.Instance.DeleteAsync(db, userId);
    }  */

    public async Task DeleteSession() 
    {
        await DataBaseManager.Instance.DeleteSession();
    }
}

public class Inductor
{
    public string idAuth;
    public string user;
    public string name;
    public float latitude = 0;
    public float longitude = 0;

    public Inductor() { }

    public Inductor(string idAuth, string name, string user)
    {
        this.idAuth = idAuth;
        this.name = name;
        this.user = user;
    }
    public Dictionary<string, object> ConvertJson()
    {
        return new Dictionary<string, object>() {
            { "idAuth", this.idAuth },
            { "user", this.user },
            { "name", this.name },
            { "latitude", this.latitude },
            { "longitude", this.longitude }
        };
    }
}

public class Student
{
    public string name;
    public string document;
    public int score = 0;
    public string idRoom;
    public float latitude = 0;
    public float longitude = 0;

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
            { "score", this.score},
            { "idRoom", this.idRoom },
            { "latitude", this.latitude },
            { "longitude", this.longitude }
        };
    }
}

