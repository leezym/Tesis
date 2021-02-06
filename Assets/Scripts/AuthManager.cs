using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    // Firebase
    protected Firebase.Auth.FirebaseAuth authFirebase;
    protected Firebase.Auth.FirebaseUser userFirebase;
    private bool signedIn;

    // DataBase
    private Dictionary<string, object> snapshot;

    // Canvas Iniciar Sesion
    public Canvas canvasGeneralSessions;
    public Canvas canvasLoginInductor, canvasNombreInductor, canvasMenuInductor;
    public Canvas canvasLoginStudent, canvasMenuStudent, canvasPuntuacionesStudent;
    public InputField inputFieldUser, inputFieldPassword, inputRoomName, inputInductorRoomSize;
    public InputField inputFieldDocument, inputFieldName;
    //public GameObject buttonChangeMapNeos;


    // UserData
    [HideInInspector]
    public string userType;
    string idInductor;
    bool triviaInProgress;
    float currentLatitude = 0, currentLongitude = 0, newLatitude = -1, newLongitude = -1;

    public Dictionary<string, object> GetSnapshot() { return snapshot; }
    public void SetSnapshot(Dictionary<string, object> snapshot) { this.snapshot = snapshot; }

    public void SetUserType(string userType) { this.userType = userType; }
    public string GetUserType() { return userType; }

    private Dictionary<string, Inductor> inductorsData = new Dictionary<string, Inductor>();

    public void Awake() 
    {
        instance = this;
        InitializeFirebase();
        InitializeAtributes();       
    }

    public void InitializeAtributes() 
    {
        userType = "";
        idInductor = "";
        triviaInProgress = false;

        inputFieldUser.text = "";
        inputFieldPassword.text = "";
        inputFieldDocument.text = "";
        inputFieldName.text = "";
        inputRoomName.text = "";
        inputInductorRoomSize.text = "";

    }

    private void InitializeFirebase()
    {
        authFirebase = Firebase.Auth.FirebaseAuth.DefaultInstance;
        authFirebase.SignOut();
        authFirebase.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public async void Update()
    {
        if (signedIn)
        {   
            if (currentLatitude != newLatitude || currentLongitude != newLongitude)
            {
                /*Input.location.Start();
                newLatitude = Input.location.lastData.latitude;
                newLongitude = Input.location.lastData.longitude;
                Input.location.Stop();*/
                //punto A
                Input.location.Start();
                newLatitude = 3.3478998f;
                newLongitude = -76.5324315f;
                Input.location.Stop();

                if (GetUserType() == "student")
                {
                    // Mapa
                    string idStudent = GetUserId();
                    await UsersManager.instance.PutUserAsync("Students", idStudent, new Dictionary<string, object>{
                        {"latitude", currentLatitude},
                        {"longitude", currentLongitude}
                    });
                    //Aquí activo el botón de cambio de mapa para los neos//
                    //buttonChangeMapNeos.SetActive(true);
                }
                else if(GetUserType() == "inductor")
                { 
                    // Mapa
                    if(idInductor == "")
                        idInductor = await UsersManager.instance.GetInductorIdByAuth(GetUserId());
                    else
                    {
                        await UsersManager.instance.PutUserAsync("Inductors", idInductor, new Dictionary<string, object>{
                            {"latitude", currentLatitude},
                            {"longitude", currentLongitude}
                        });
                        //Aquí desactivo el botón de cambio de mapa para los neos//
                        //buttonChangeMapNeos.SetActive(false);
                        Debug.Log("actualiza locacion en la DB");
                    }
                }  

                currentLatitude = newLatitude;
                currentLongitude = newLongitude;
            }

            if (GetUserType() == "student")
            {                
                // Cerrar la sesión si al inductor elimina la sala
                LogOutStudent();

                // Mostrar las trivias
                ShowTriviasStudent();
            }            
        }
    }

    async void LogOutStudent()
    {
        SetSnapshot(await UsersManager.instance.GetUserAsync("Students", GetUserId()));
        if (GetSnapshot() == null)
        {
            Exit();
            GameObject.Find("PanelGeneralSessions").GetComponent<Canvas>().enabled = true;
        }
    }

    async void ShowTriviasStudent()
    {
        if(idInductor == "")
            idInductor = await UsersManager.instance.GetInductorByStudent(GetUserId());

        if (!triviaInProgress)
        {
            List<Dictionary<string, object>> listAvailableTrivias = await DataBaseManager.instance.SearchByAttribute("InductorTriviasChallenges", "idInductor", idInductor, "available", true);
            foreach(Dictionary<string, object> availableTrivia in listAvailableTrivias)
            {
                foreach(KeyValuePair<string, object> pair in availableTrivia)
                {
                    if(pair.Key == "idBuilding")
                    {
                        triviaInProgress = true;

                        ScenesManager.instance.LoadNewCanvas(LoadingScreenManager.instance.canvasTimerTriviaLoading);
                        ScenesManager.instance.DeleteCurrentCanvas(canvasMenuStudent);
                        ScenesManager.instance.DeleteCurrentCanvas(canvasPuntuacionesStudent);

                        LoadingScreenManager.instance.SetTimeTimerTrivia(LoadingScreenManager.instance.timer);
                        LoadingScreenManager.instance.SetIdTriviaBuilding(pair.Value.ToString());
                        LoadingScreenManager.instance.SetListTrivias(await TriviasManager.instance.GetTriviaByIdBuilding(pair.Value.ToString()));
                    }
                }
            }
        }
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
            signedIn = userFirebase != authFirebase.CurrentUser && authFirebase.CurrentUser != null;
            if (!signedIn && userFirebase != null)
            {
                Debug.Log("salir " + GetUserType());

                if (GetUserType() == "inductor")
                {
                    Debug.Log("Se salio el inductor");
                }
                else if (GetUserType() == "student")
                {
                    Debug.Log("Se salio el neo");
                }
            }
            userFirebase = authFirebase.CurrentUser;
            if (signedIn)
            {
                Debug.Log("inicio " + GetUserType());
                if (GetUserType() == "inductor")
                {
                    Debug.Log("spy un inductor");
                    ScenesManager.instance.DeleteCurrentCanvas(canvasLoginInductor);
                    ScenesManager.instance.LoadNewCanvas(canvasNombreInductor);
                    
                }
                else if (GetUserType() == "student")
                {
                    Debug.Log("spy un estu");
                    ScenesManager.instance.DeleteCurrentCanvas(canvasLoginStudent);
                    ScenesManager.instance.LoadNewCanvas(canvasMenuStudent);
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

        await authFirebase.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(async taskSignIn => {                
            if (taskSignIn.IsFaulted)
            {
                foreach (System.Exception exception in taskSignIn.Exception.InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                    string message = NotificationsManager.instance.GetErrorMessage(firebaseEx);
                    NotificationsManager.instance.SetFailureNotificationMessage(message);
                }
                return;
            }
            await UsersManager.instance.PostNewInductor(AuthManager.instance.GetUserId(), user);
        });
    }

    public async void SignInStudent()
    {
        string name = inputFieldName.text;
        string document = inputFieldDocument.text;
        string idRoom = null;

        if(name != "" && document != "")
        {
            if (!await UsersManager.instance.ExistUserByDocument("Students", document))
            {
                if (!await DataBaseManager.instance.IsEmptyTable("Rooms"))
                {
                    idRoom = await RoomsManager.instance.GetAvailableRoom();
                    if(idRoom != null)
                    {
                        await authFirebase.SignInAnonymouslyAsync().ContinueWith(async task =>
                        {
                            if (task.IsFaulted)
                            {
                                foreach (System.Exception exception in task.Exception.InnerExceptions)
                                {
                                    Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                                    string message = NotificationsManager.instance.GetErrorMessage(firebaseEx);
                                    NotificationsManager.instance.SetFailureNotificationMessage(message);
                                }
                                return;
                            }

                            await UsersManager.instance.PostNewStudent(userFirebase.UserId, name, document, idRoom);                            
                            //SetSnapshot(await UsersManager.instance.GetUserAsync("Students", userFirebase.UserId));

                        });
                    }else{
                        NotificationsManager.instance.SetFailureNotificationMessage("No hay salas disponibles. Pide ayuda a tu inductor más cercano.");
                    }
                }else{
                    NotificationsManager.instance.SetFailureNotificationMessage("No hay salas disponibles. Pide ayuda a tu inductor más cercano.");
                }
            }else{
                NotificationsManager.instance.SetFailureNotificationMessage("Ya existe un usuario con ese documento.");
            }
        }else{
            NotificationsManager.instance.SetFailureNotificationMessage("Por favor llena los campos.");
        }
    }

    public void LogOut() 
    {
        NotificationsManager.instance.SetQuestionNotificationMessage("¿Está seguro que desea cerrar sesión?");
        NotificationsManager.instance.acceptQuestionButton.onClick.AddListener(Exit);
    }

    public async void Exit()
    {
        userFirebase = authFirebase.CurrentUser;
        if (userFirebase != null)
        {
            if (GetUserType() == "inductor")
            {
                await UsersManager.instance.DeleteSession(userFirebase.UserId);
                ScenesManager.instance.DeleteCurrentCanvas(canvasMenuInductor);
            }
            else if (GetSnapshot() != null || GetUserType() == "student")
            {
                await RoomsManager.instance.DeleteStudentInRoom(userFirebase.UserId);
                ScenesManager.instance.DeleteCurrentCanvas(canvasMenuStudent);
            }
            
            ScenesManager.instance.LoadNewCanvas(canvasGeneralSessions);
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
                    await UsersManager.instance.DeleteSession(idUser);
                }
                else if (GetSnapshot() != null)
                {
                    await RoomsManager.instance.DeleteStudentInRoom(idUser);
                }
                
                //authFirebase = null;
                authFirebase.SignOut();
            });*/
        }        
    }

}
