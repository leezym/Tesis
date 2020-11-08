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

    public async Task<string> SearchAvailableRoom(string db) 
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
                    size = Convert.ToInt32(pair.Value);
                }
                else if (pair.Key == "currentSize") 
                {
                    currentSize = Convert.ToInt32(pair.Value);
                }
            }

            if (currentSize < size) 
            {
                await reference.Collection(db).Document(documentSnapshot.Id).UpdateAsync(new Dictionary<string, object>
                {
                    { "currentSize", currentSize+1 }
                });

                Debug.Log("DataBaseManager.SearchRoom " + documentSnapshot.Id);
                return documentSnapshot.Id;
            }
        }
        return null;
    }

    
    public async Task<string> SearchRoomByInductor(string db, string idInductor)
    {
        Query RoomMembersQuery = reference.Collection(db).WhereEqualTo("idInductor", idInductor);
        QuerySnapshot RoomMembersQuerySnapshot = await RoomMembersQuery.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshotRooms in RoomMembersQuerySnapshot.Documents)
        {
            return documentSnapshotRooms.Id;
        }
        return null;
    }

    public async Task<List<string>> ListNameStudents(string db, string idInductor)
    {  
        string RoomId = await SearchRoomByInductor("Rooms", idInductor);        

        Query MembersQuery = reference.Collection(db).WhereEqualTo("idRoom", RoomId);
        QuerySnapshot MembersQuerySnapshot = await MembersQuery.GetSnapshotAsync();

        List<string> Estudiantes = new List<string>();

        foreach (DocumentSnapshot documentSnapshotMembers in MembersQuerySnapshot.Documents)
        {
            Dictionary<string, object> NeoJaverian = documentSnapshotMembers.ToDictionary();
            foreach (KeyValuePair<string, object> pair in NeoJaverian)
            {
                if (pair.Key == "name")
                {
                    Estudiantes.Add(pair.Value.ToString());
                }
        
            }
        }
        return Estudiantes;     
    }

    public async Task DeleteSession(string dbInductors, string idInductor, string dbRooms, string dbStudents)
    {
        // Eliminar inductor
        DocumentReference docRefInductor = reference.Collection(dbInductors).Document(idInductor);
        await docRefInductor.DeleteAsync();

        // Eliminar sala
        Query queryValue = reference.Collection(dbRooms).WhereEqualTo("idInductor", idInductor);
        QuerySnapshot querySnapshotRoom = await queryValue.GetSnapshotAsync();
        string idRoom = "";
        foreach (DocumentSnapshot documentSnapshot in querySnapshotRoom.Documents)
        {
            idRoom = documentSnapshot.Id;
            DocumentReference docRefRoom = documentSnapshot.Reference;
            await docRefRoom.DeleteAsync();
        }

        // Eliminar estudiantes de la sala
        Query queryCol = reference.Collection(dbStudents).WhereEqualTo("idRoom", idRoom);
        QuerySnapshot querySnapshotStudent = await queryCol.GetSnapshotAsync();

        foreach (DocumentSnapshot documentSnapshot in querySnapshotStudent.Documents)
        {
            DocumentReference docRefStudent = documentSnapshot.Reference;
            await docRefStudent.DeleteAsync();
        }
    }
}
