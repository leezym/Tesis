﻿using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance { get => instance; set => instance = value; }

    // Firebase
    protected Firebase.Auth.FirebaseAuth authFirebase;
    protected Firebase.Auth.FirebaseUser userFirebase;
    protected Firebase.FirebaseApp app;
    private bool signedIn;

    // DataBase
    private Dictionary<string, object> snapshot;

    public Canvas canvasGeneralSessions;
    [Header("CANVAS INDUCTOR")]
    public Canvas canvasLoginInductor;
    public Canvas canvasNombreInductor; 
    public Canvas canvasMenuInductor;
    public InputField inputFieldUser;
    public InputField inputFieldPassword;
    public InputField inputRoomName;
    public InputField inputInductorRoomSize;

    [Header("CANVAS ESTUDIANTES")]
    public Canvas canvasLoginStudent;
    public Canvas canvasMenuStudent;
    public Canvas canvasPuntuacionesStudent;
    public Canvas canvasGeoMap;
    public CanvasGroup canvasARMap;
    public InputField inputFieldDocument;
    public InputField inputFieldName;
    
    // UserData
    bool triviaInProgress;
    float currentLatitude = 0, currentLongitude = 0, newLatitude = -1, newLongitude = 1;

    public Dictionary<string, object> GetSnapshot() { return snapshot; }
    public void SetSnapshot(Dictionary<string, object> snapshot) { this.snapshot = snapshot; }

    private Dictionary<string, Inductor> inductorsData = new Dictionary<string, Inductor>();

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        InitializeAtributes();       
    }

    public void InitializeAtributes() 
    {
        GlobalDataManager.Instance.userType = "";
        GlobalDataManager.Instance.idInductorByStudent = "";
        triviaInProgress = false;

        inputFieldUser.text = "";
        inputFieldPassword.text = "";
        inputFieldDocument.text = "";
        inputFieldName.text = "";
        inputRoomName.text = "";
        inputInductorRoomSize.text = "";

    }

    public void InitializeAuthFirebase()
    {
        authFirebase = Firebase.Auth.FirebaseAuth.DefaultInstance;
        authFirebase.SignOut();
        authFirebase.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void Update()
    {
        if (signedIn)
        {         
            StartCoroutine(MapManager.Instance.Location());
            
            if(currentLatitude != newLatitude || currentLongitude != newLongitude)
            {
                MapManager.Instance.PutLocation();
                currentLatitude = newLatitude;
                currentLongitude = newLongitude;
            }

            if (GlobalDataManager.Instance.userType == "student")
            {                
                if(GlobalDataManager.Instance.idInductorByStudent != "")
                {
                    // Cerrar la sesión si el inductor elimina la sala
                    LogOutStudent();

                    // Mostrar las trivias cuando el inductor las active
                    ShowTrivias();
                }
            }            
        }
    }

    async void LogOutStudent()
    {
        SetSnapshot(await UsersManager.Instance.GetUserAsync("Inductors", GlobalDataManager.Instance.idInductorByStudent));

        if (GetSnapshot() == null)
        {
            Debug.Log("saca al neo porque no existe sala");
            InitializeAtributes();
            authFirebase.SignOut();
        }
    }    

    void ShowTrivias()
    {
        TriviasChallengesManager.Instance.ShowTriviasStudent(GlobalDataManager.Instance.idInductorByStudent, triviaInProgress, canvasMenuStudent, canvasPuntuacionesStudent);
    }

    public string GetUserId()
    {
        userFirebase = authFirebase.CurrentUser;
        if (userFirebase != null)
        {
            return userFirebase.UserId;
        }
        return null;
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (authFirebase.CurrentUser != userFirebase)
        {
            Debug.Log("authFirebase.CurrentUser != userFirebase");
            signedIn = userFirebase != authFirebase.CurrentUser && authFirebase.CurrentUser != null;
            if (!signedIn && userFirebase != null)
            {
                Debug.Log("salir " + GlobalDataManager.Instance.userType);
                ScenesManager.Instance.LoadNewCanvas(canvasGeneralSessions);
                
                if (GlobalDataManager.Instance.userType == "inductor")
                {
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasMenuInductor);
                }
                else if (GlobalDataManager.Instance.userType == "student")
                {
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasMenuStudent);
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasPuntuacionesStudent);
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasGeoMap);
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasARMap);
                }
            }
            userFirebase = authFirebase.CurrentUser;
            if (signedIn)
            {
                Debug.Log("inicio " + GlobalDataManager.Instance.userType);
                if (GlobalDataManager.Instance.userType == "inductor")
                {
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasLoginInductor);
                    ScenesManager.Instance.LoadNewCanvas(canvasNombreInductor);
                    
                }
                else if (GlobalDataManager.Instance.userType == "student")
                {
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasLoginStudent);
                    ScenesManager.Instance.LoadNewCanvas(canvasMenuStudent);
                }     
                
            }
        }
    }

    void OnDestroy()
    {
        authFirebase.StateChanged -= AuthStateChanged;
        //authFirebase = null;
        Exit();
    }

    public async void SignInInductor() {
        string user = inputFieldUser.text;
        string password = inputFieldPassword.text;
        string email = user + "@javerianacali.edu.co";
        
        if(password != "" && email != "")
        {
            await authFirebase.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(async task => {                
                if (task.IsFaulted)
                {
                    foreach (System.Exception exception in task.Exception.InnerExceptions)
                    {
                        Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                        string message = NotificationsManager.Instance.GetErrorMessage(firebaseEx);
                        NotificationsManager.Instance.SetFailureNotificationMessage(message);
                    }
                    return;
                }
                
                await UsersManager.Instance.PostNewInductor(AuthManager.Instance.GetUserId(), user);
                GlobalDataManager.Instance.idUserInductor = await UsersManager.Instance.GetInductorIdByAuth(AuthManager.Instance.GetUserId());
                GameObject.FindObjectOfType<ListTrivias>().SearchBuilding();

            });

        }else{
            NotificationsManager.Instance.SetFailureNotificationMessage("Por favor llena los campos.");
        }

    }

    public async void SignInStudent()
    {
        string name = inputFieldName.text;
        string document = inputFieldDocument.text;

        if(name != "" && document != "")
        {
            if (!await UsersManager.Instance.ExistUserByDocument("Students", document))
            {
                if (!await DataBaseManager.Instance.IsEmptyTable("Rooms"))
                {
                    GlobalDataManager.Instance.idRoomByStudent = await RoomsManager.Instance.GetAvailableRoom();
                    if(GlobalDataManager.Instance.idRoomByStudent != null)
                    {
                        await authFirebase.SignInAnonymouslyAsync().ContinueWith(async task => {
                            if (task.IsFaulted)
                            {
                                foreach (System.Exception exception in task.Exception.InnerExceptions)
                                {
                                    Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                                    string message = NotificationsManager.Instance.GetErrorMessage(firebaseEx);
                                    NotificationsManager.Instance.SetFailureNotificationMessage(message);
                                }
                                return;
                            }
                            await UsersManager.Instance.PostNewStudent(userFirebase.UserId, name, document, GlobalDataManager.Instance.idRoomByStudent);
                            GlobalDataManager.Instance.idUserStudent = userFirebase.UserId;
                            GlobalDataManager.Instance.idInductorByStudent = (await DataBaseManager.Instance.SearchAttribute("Rooms", GlobalDataManager.Instance.idRoomByStudent, "idInductor")).ToString();
                        });
                    }else{
                        NotificationsManager.Instance.SetFailureNotificationMessage("No hay salas disponibles. Pide ayuda a tu inductor más cercano.");
                    }
                }else{
                    NotificationsManager.Instance.SetFailureNotificationMessage("No hay salas disponibles. Pide ayuda a tu inductor más cercano.");
                }
            }else{
                NotificationsManager.Instance.SetFailureNotificationMessage("Ya existe un usuario con ese documento.");
            }
        }else{
            NotificationsManager.Instance.SetFailureNotificationMessage("Por favor llena los campos.");
        }
    }

    public void LogOut() 
    {
        NotificationsManager.Instance.SetQuestionNotificationMessage("¿Está seguro que desea cerrar sesión?");
        NotificationsManager.Instance.acceptQuestionButton.onClick.AddListener(Exit);
    }

    public async void Exit()
    {
        userFirebase = authFirebase.CurrentUser;
        if (userFirebase != null)
        {            
            if (GlobalDataManager.Instance.userType == "inductor")
                await UsersManager.Instance.DeleteSession();
            else if (GlobalDataManager.Instance.userType == "student")
                await RoomsManager.Instance.DeleteStudentInRoom();

            InitializeAtributes();
            authFirebase.SignOut();
           
                //authFirebase = null;
            /*await userFirebase.DeleteAsync().ContinueWith(async task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }


                if (GetIsInductor())
                {
                    await UsersManager.Instance.DeleteSession(idUser);
                }
                else if (GetSnapshot() != null)
                {
                    await RoomsManager.Instance.DeleteStudentInRoom(idUser);
                }
                
                //authFirebase = null;
                authFirebase.SignOut();
            });*/
        }        
    }

}
