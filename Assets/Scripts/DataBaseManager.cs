using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Firestore;
using System.Threading.Tasks;
using System;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    FirebaseFirestore reference;

    public void Awake()
    {
        instance = this;
    }

    [System.Obsolete]
    void Start() {
        reference = FirebaseFirestore.DefaultInstance;
        /*InsertUser("Inductors", "null", new Dictionary<string, object>() {
            { " ", " " }
        });*/
    }

    public async Task<Dictionary<string, object>> SelectUserByIdAsync(string db, string userId)
    {
        DocumentReference docRef = reference.Collection(db).Document(userId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();
            return data;
        }
        return null;
    }

    // Recuperar las puntuaciones https://firebase.google.com/docs/database/unity/retrieve-data?hl=es
    
    public void InsertUser(string db, string userId, Dictionary<string, object> json)
    {
        CollectionReference colRef = reference.Collection(db);
        colRef.Document(userId).SetAsync(json, SetOptions.MergeAll);
    }

    public async Task UpdateUserAsync(string db, string userId, string attribute, string value)
    {
        DocumentReference docRef = reference.Collection(db).Document(userId);
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { attribute, value }
        };
        await docRef.UpdateAsync(data);
    }

    public async Task DeleteUserAsync(string db, string userId) 
    {
        DocumentReference docRef = reference.Collection(db).Document(userId);
        await docRef.DeleteAsync();
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

    public void SearchData(string db)
    {

    }

    public async Task<List<string>> SearchNeoJaveriansAsync(string db, string idInductor)
    {  
        Query RoomMembersQuery = reference.Collection(db).WhereEqualTo("idInductor", idInductor);
        QuerySnapshot RoomMembersQuerySnapshot = await RoomMembersQuery.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshotRooms in RoomMembersQuerySnapshot.Documents)
        {
            Console.WriteLine("Document data for {0} document:", documentSnapshotRooms.Id);
            var RoomId = documentSnapshotRooms.Id;

            Query MembersQuery = reference.Collection("Students").WhereEqualTo("idRoom", RoomId);
            QuerySnapshot MembersQuerySnapshot = await MembersQuery.GetSnapshotAsync();

            List<string> Estudiantes = new List<string>();

            foreach (DocumentSnapshot documentSnapshotMembers in MembersQuerySnapshot.Documents)
            {
                

                Dictionary<string, object> NeoJaverian = documentSnapshotMembers.ToDictionary();
                foreach (KeyValuePair<string, object> pair in NeoJaverian)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                    if (pair.Key == "name")
                    {
                        Estudiantes.Add(pair.Value.ToString());
                    }
           
                }
            }
            return Estudiantes;
            /*
            Dictionary<string, object> city = documentSnapshot.ToDictionary();
            foreach (KeyValuePair<string, object> pair in city)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
            Console.WriteLine("");
            */
        }
        return null;
        
    }

}
