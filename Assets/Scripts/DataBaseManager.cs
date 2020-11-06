using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Firestore;
using System.Threading.Tasks;
using System;
using System.Linq;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    FirebaseFirestore reference;

    public void Awake()
    {
        instance = this;
    }

    public void Start() {
        reference = FirebaseFirestore.DefaultInstance;
    }

    public async Task<Dictionary<string, object>> SearchById(string db, string id)
    {
        DocumentReference docRef = reference.Collection(db).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();
            return data;
        }
        return null;
    }

    // Recuperar las puntuaciones https://firebase.google.com/docs/database/unity/retrieve-data?hl=es
    
    public void InsertWithId(string db, string id, Dictionary<string, object> json)
    {
        CollectionReference colRef = reference.Collection(db);
        colRef.Document(id).SetAsync(json, SetOptions.MergeAll);
    }

    public void InsertWithoutId(string db, Dictionary<string, object> json)
    {
        CollectionReference colRef = reference.Collection(db);
        colRef.AddAsync(json);
    }

    public async Task UpdateUserAsync(string db, string id, string attribute, string value)
    {
        DocumentReference docRef = reference.Collection(db).Document(id);
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { attribute, value }
        };
        await docRef.UpdateAsync(data);
    }

    public async Task DeleteUserAsync(string db, string id) 
    {
        DocumentReference docRef = reference.Collection(db).Document(id);
        await docRef.DeleteAsync();
    }

    public async Task DeleteRoomAsync(string db, string id)
    {
        Query queryValue = reference.Collection(db).WhereEqualTo("idInductor", id);
        QuerySnapshot querySnapshot = await queryValue.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            DocumentReference docRef = documentSnapshot.Reference;
            await docRef.DeleteAsync();
        }
    }

    public async Task<int> SizeTable(string db)
    {
        Query queryCol = reference.Collection(db);
        QuerySnapshot querySnapshot = await queryCol.GetSnapshotAsync();
        return querySnapshot.Count;
    }

    public async Task<bool> IsEmptyTable(string db)
    {
        if (await SizeTable(db) == 0) return true;
        else return false;
    }

    public async Task<string> SearchRoom(string db) 
    {
        Query queryCol = reference.Collection(db);
        QuerySnapshot querySnapshot = await queryCol.GetSnapshotAsync();

        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            Dictionary<string, object> data = documentSnapshot.ToDictionary();
            int size = 0, currentSize = 0;
            foreach (KeyValuePair<string, object> pair in data)
            {
                if (pair.Key == "size")
                {
                    size = (int)pair.Value;
                }
                else if (pair.Key == "currentSize") 
                {
                    currentSize = (int)pair.Value;
                }
            }

            if (currentSize < size) 
            {
                return documentSnapshot.Id;
            }
        }
        return null;
    }
}
