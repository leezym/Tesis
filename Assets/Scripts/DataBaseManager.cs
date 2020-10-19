using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instace;

    DatabaseReference reference;

    private void Awake()
    {
        instace = this;
    }

    [System.Obsolete]
    void Start() {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://tesis-45e87.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public DatabaseReference SelectUserById(string db) 
    {
        return FirebaseDatabase.DefaultInstance.GetReference(db);
    }

    // Recuperar las puntuaciones https://firebase.google.com/docs/database/unity/retrieve-data?hl=es
    

    public void InsertUser(string db, string userId, string json)
    {
        reference.Child(db).Child(userId).SetRawJsonValueAsync(json);
    }

    public void UpdateUser(string db, string userId, string atribute, string value)
    {
        reference.Child(db).Child(userId).Child(atribute).SetValueAsync(value);

    }

    public void DeleteUser(string db, string userId) 
    {
        reference.Child(db).UpdateChildrenAsync(null);
    }
}
