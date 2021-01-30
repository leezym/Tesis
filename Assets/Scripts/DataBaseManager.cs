﻿using System.Collections;
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

    public async Task<List<DocumentSnapshot>> SearchByCollection(string db)
    {
        CollectionReference colRef = reference.Collection(db);
        QuerySnapshot querySnapshot = await colRef.GetSnapshotAsync();
        List<DocumentSnapshot> data = new List<DocumentSnapshot>();

        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            data.Add(documentSnapshot);
        }
        return data;
    }

    public async Task<Dictionary<string, object>> SearchById(string db, string id)
    {
        DocumentReference docRef = reference.Collection(db).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            return snapshot.ToDictionary();
        }
        return null;
    }

    public async Task<object> SearchById(string db, string id, string attribute)
    {
        DocumentReference docRef = reference.Collection(db).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            foreach (KeyValuePair<string, object> pair in snapshot.ToDictionary())
            {
                if(pair.Key == attribute)
                    return pair.Value;
            }
        }
        return null;
    }

    public async Task<List<Dictionary<string, object>>> SearchByAttribute(string db, string attribute, object value)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryAttribute = colRef.WhereEqualTo(attribute, value);
        QuerySnapshot querySnapshot = await queryAttribute.GetSnapshotAsync();
        List<Dictionary<string,object>> data = new List<Dictionary<string, object>>();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            data.Add(documentSnapshot.ToDictionary());
        }
        return data;
    }

    public async Task<List<Dictionary<string, object>>> SearchByAttribute(string db, string attributeOne, object valueOne, string attributeTwo, object valueTwo)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryAttributes = colRef.WhereEqualTo(attributeOne, valueOne).WhereEqualTo(attributeTwo, valueTwo);
        QuerySnapshot querySnapshot = await queryAttributes.GetSnapshotAsync();
        List<Dictionary<string,object>> data = new List<Dictionary<string, object>>();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            data.Add(documentSnapshot.ToDictionary());
        }
        return data;
    }

    public async Task<string> SearchId(string db, string attribute, object value)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryDocument = colRef.WhereEqualTo(attribute, value);
        QuerySnapshot querySnapshot = await queryDocument.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            return documentSnapshot.Id;
        }
        return null;
    }

    public async Task<string> SearchId(string db, string attribute, object value, string attribute2, object value2)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryDocument = colRef.WhereEqualTo(attribute, value).WhereEqualTo(attribute2, value2);
        QuerySnapshot querySnapshot = await queryDocument.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            return documentSnapshot.Id;
        }
        return null;
    }

    public async Task<object> SearchAttribute(string db, string id, string attribute)
    {
        DocumentReference docRef = reference.Collection(db).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();
            foreach(KeyValuePair<string, object> pair in data)
            {
                if(pair.Key == attribute)
                    return pair.Value;
            }
        }
        return null;
    }

    public async Task<List<Dictionary<string, object>>> SearchByOrderDescending(string db, string attribute)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryAttribute = colRef.OrderByDescending(attribute);
        QuerySnapshot querySnapshot = await queryAttribute.GetSnapshotAsync();
        List<Dictionary<string,object>> data = new List<Dictionary<string, object>>();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            data.Add(documentSnapshot.ToDictionary());
        }
        return data;
    }

    public async Task<List<Dictionary<string, object>>> SearchByOrderDescendingAndLimit(string db, string attribute, int limit)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryAttribute = colRef.OrderByDescending(attribute).Limit(limit);
        QuerySnapshot querySnapshot = await queryAttribute.GetSnapshotAsync();
        List<Dictionary<string,object>> data = new List<Dictionary<string, object>>();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            data.Add(documentSnapshot.ToDictionary());
        }
        return data;
    }
    
    public async Task InsertWithId(string db, string id, Dictionary<string, object> json)
    {
        CollectionReference colRef = reference.Collection(db);
        await colRef.Document(id).SetAsync(json, SetOptions.MergeAll);
    }

    public async Task InsertWithoutId(string db, Dictionary<string, object> json)
    {
        CollectionReference colRef = reference.Collection(db);
        await colRef.AddAsync(json);
    }

    public async Task UpdateAsync(string db, string id, Dictionary<string,object> data)
    {
        DocumentReference docRef = reference.Collection(db).Document(id);        
        await docRef.UpdateAsync(data);
    }

    public async Task UpdateAsync(string db, string attributeOne, object valueOne, Dictionary<string,object> data)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryDocument = colRef.WhereEqualTo(attributeOne, valueOne);
        QuerySnapshot querySnapshot = await queryDocument.GetSnapshotAsync();
        string id = "";
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            id = documentSnapshot.Id;
        }

        DocumentReference docRef = reference.Collection(db).Document(id);        
        await docRef.UpdateAsync(data);
    }

    public async Task UpdateAsync(string db, string attributeOne, object valueOne, string attributeTwo, object valueTwo, Dictionary<string,object> data)
    {
        CollectionReference colRef = reference.Collection(db);
        Query queryDocument = colRef.WhereEqualTo(attributeOne, valueOne).WhereEqualTo(attributeTwo, valueTwo);
        QuerySnapshot querySnapshot = await queryDocument.GetSnapshotAsync();
        string id = "";
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            id = documentSnapshot.Id;
        }

        DocumentReference docRef = reference.Collection(db).Document(id);        
        await docRef.UpdateAsync(data);
    }

    public async Task DeleteAsync(string db, string id) 
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

    public async Task<List<Coords>> GetMyInductorLocation(string idStudent)
    {
        float latitude = 0, longitude = 0;
        List<Coords> coordsList = new List<Coords>();
        string idInductor = await UsersManager.instance.GetInductorByStudent(idStudent);        
        Dictionary<string, object> location = await SearchById("Inductors", idInductor);
        foreach (KeyValuePair<string, object> pair in location)
        {
            if(pair.Key == "latitude")
                latitude = (float)pair.Value;
            if(pair.Key == "longitude")
                longitude = (float)pair.Value;
        }
        coordsList.Add(new Coords(latitude, longitude));
        return coordsList;
    }

    public async Task<List<Coords>> GetOthersInductorsLocation(string idInductor)
    {
        float latitude = 0, longitude = 0;  
        List<Coords> coordsList = new List<Coords>();    
        List<DocumentSnapshot> locationList = await SearchByCollection("Inductors");
        foreach(DocumentSnapshot location in locationList)
        {
            if (location.Id != idInductor)
            {
                foreach (KeyValuePair<string, object> pair in location.ToDictionary())
                {
                    if(pair.Key == "latitude")
                        latitude = (float)Convert.ToDecimal(pair.Value);
                    if(pair.Key == "longitude")
                        longitude = (float)Convert.ToDecimal(pair.Value);                
                }
                coordsList.Add(new Coords(latitude, longitude));
            }
        }
        return coordsList;
    }

    public async Task<string> GetAvailableRoom(string dbRoom) 
    {
        Query queryCol = reference.Collection(dbRoom);
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
                await reference.Collection(dbRoom).Document(documentSnapshot.Id).UpdateAsync(new Dictionary<string, object>
                {
                    { "currentSize", currentSize+1 }
                });
                return documentSnapshot.Id;
            }
        }
        return null;
    }
    
    public async Task<string> GetRoomByInductor(string db, string idInductor)
    {
        Query RoomMembersQuery = reference.Collection(db).WhereEqualTo("idInductor", idInductor);
        QuerySnapshot RoomMembersQuerySnapshot = await RoomMembersQuery.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshotRooms in RoomMembersQuerySnapshot.Documents)
        {
            return documentSnapshotRooms.Id;
        }
        return null;
    }  

    public async Task<Dictionary<string, object>> ListStudentsByGroup(string db, string idInductor)
    {  
        string RoomId = await GetRoomByInductor("Rooms", idInductor);        

        Query MembersQuery = reference.Collection(db).WhereEqualTo("idRoom", RoomId);
        QuerySnapshot MembersQuerySnapshot = await MembersQuery.GetSnapshotAsync();
        
        foreach (DocumentSnapshot documentSnapshotMembers in MembersQuerySnapshot.Documents)
        {
            return documentSnapshotMembers.ToDictionary();            
        }
        return null;     
    }

    public async Task<List<Dictionary<string, object>>> SearchHintDataByRoom(string idRoom)
    {
        CollectionReference colRef = reference.Collection("Hints");
        QuerySnapshot querySnapshot = await colRef.GetSnapshotAsync();
        List<Dictionary<string, object>> dataHints = new List<Dictionary<string, object>>();

        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            string idHint = documentSnapshot.Id;
            Dictionary<string, object> hint = documentSnapshot.ToDictionary();
            
            List<Dictionary<string, object>> hintsChallengesList = await SearchByAttribute("HintsChallenges", "idRoom", idRoom, "idHint", idHint) ;
            foreach(Dictionary<string, object> hintChallenge in hintsChallengesList)
            {
                foreach(KeyValuePair<string, object> pair in hintChallenge)
                {
                    hint.Add(pair.Key, pair.Value);
                }
            }
            dataHints.Add(hint);
        }
        return dataHints;
    }

    public async Task<List<Dictionary<string, object>>> SearchTriviaDataByBuilding(string idInductor)
    {
        CollectionReference colRef = reference.Collection("Buildings");
        QuerySnapshot querySnapshot = await colRef.GetSnapshotAsync();
        List<Dictionary<string, object>> dataTrivias = new List<Dictionary<string, object>>();

        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            string idBuilding = documentSnapshot.Id;
            Dictionary<string, object> building = documentSnapshot.ToDictionary();
            
            List<Dictionary<string, object>> inductorTriviasChallengesList = await SearchByAttribute("InductorTriviasChallenges", "idInductor", idInductor, "idBuilding", idBuilding) ;
            foreach(Dictionary<string, object> triviaChallenge in inductorTriviasChallengesList)
            {
                foreach(KeyValuePair<string, object> pair in triviaChallenge)
                {
                    building.Add(pair.Key, pair.Value);
                }
            }
            dataTrivias.Add(building);
        }
        return dataTrivias;
    }

    public async Task DeleteSession(string idInductor)
    {
        // Eliminar inductor
        DocumentReference docRefInductor = reference.Collection("Inductors").Document(idInductor);
        await docRefInductor.DeleteAsync();

        // Eliminar sala
        Query queryValue = reference.Collection("Rooms").WhereEqualTo("idInductor", idInductor);
        QuerySnapshot querySnapshotRoom = await queryValue.GetSnapshotAsync();
        string idRoom = "";
        foreach (DocumentSnapshot documentSnapshot in querySnapshotRoom.Documents)
        {
            idRoom = documentSnapshot.Id;
            DocumentReference docRefRoom = documentSnapshot.Reference;
            await docRefRoom.DeleteAsync();
        }

        // Eliminar estudiantes de la sala
        Query queryCol = reference.Collection("Students").WhereEqualTo("idRoom", idRoom);
        QuerySnapshot querySnapshotStudent = await queryCol.GetSnapshotAsync();

        foreach (DocumentSnapshot documentSnapshot in querySnapshotStudent.Documents)
        {
            DocumentReference docRefStudent = documentSnapshot.Reference;
            await docRefStudent.DeleteAsync();
        }
    }

    public async Task DeleteStudentInRoom(string idStudent)
    {
        Dictionary<string, object> data = await SearchById("Students", idStudent);
        string idRoom = "";
        if (data != null)
        {
            foreach (KeyValuePair<string, object> pair in data)
            {
                if(pair.Key == "idRoom")      
                    idRoom = pair.Value.ToString();    
            }
        }

        DocumentReference docRef = reference.Collection("Students").Document(idStudent);
        await docRef.DeleteAsync();

        data = await SearchById("Rooms", idRoom);
        foreach (KeyValuePair<string, object> pair in data)
        {
            if(pair.Key == "currentSize")
                await reference.Collection("Rooms").Document(idRoom).UpdateAsync(new Dictionary<string, object>
                {
                    { "currentSize", Convert.ToInt32(pair.Value)-1 }
                });
        }
    }
}
