using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Firestore;
using System.Threading.Tasks;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    //DatabaseReference reference;
    FirebaseFirestore reference;

    private void Awake()
    {
        instance = this;
    }

    [System.Obsolete]
    void Start() {
        // Set this before calling into the realtime database.
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://tesis-45e87.firebaseio.com/");

        // Get the root reference location of the database.
        //reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference = FirebaseFirestore.DefaultInstance;
    }

    public DatabaseReference SelectUserById(string db) 
    {
        return FirebaseDatabase.DefaultInstance.GetReference(db);
    }

    // Recuperar las puntuaciones https://firebase.google.com/docs/database/unity/retrieve-data?hl=es
    

    public void InsertUser(string db, string userId, Dictionary<string, object> json)
    {
        //reference.Child(db).Child(userId).SetRawJsonValueAsync(json);
        CollectionReference colRef = reference.Collection(db);
        colRef.Document(userId).SetAsync(json);
    }

    public void UpdateUser(string db, string userId, string atribute, string value)
    {
        //reference.Child(db).Child(userId).Child(atribute).SetValueAsync(value);
        DocumentReference docRef = reference.Collection(db).Document(userId);

        ListenerRegistration listener = docRef.Listen(snapshot =>
        {
            Dictionary<string, object> data = snapshot.ToDictionary();
        });
    }

    public void DeleteUser(string db, string userId) 
    {
        //reference.Child(db).UpdateChildrenAsync(null);
        DocumentReference colRef = reference.Collection(db).Document(userId);
        colRef.DeleteAsync();
    }
}
