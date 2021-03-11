using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance { get => instance; set => instance = value; }

    // Firebase
    protected Firebase.Auth.FirebaseAuth authFirebase;
    protected Firebase.Auth.FirebaseUser userFirebase;
    private bool signedIn;

    // DataBase
    private Dictionary<string, object> snapshot;

    public Canvas canvasGeneralSessions;
    [Header("CANVAS INDUCTOR")]
    public Canvas canvasLoginInductor, canvasNombreInductor, canvasMenuInductor;
    public InputField inputFieldUser, inputFieldPassword, inputRoomName, inputInductorRoomSize;

    [Header("CANVAS ESTUDIANTES")]
    public Canvas canvasLoginStudent, canvasMenuStudent, canvasPuntuacionesStudent, canvasGeoMap;
    public CanvasGroup canvasARMap;
    public InputField inputFieldDocument, inputFieldName;
    



    // UserData
    [HideInInspector]
    public string userType;
    string idInductor;
    bool triviaInProgress;
    float currentLatitude = 0, currentLongitude = 0, newLatitude = -1, newLongitude = 1;

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

    async void Update()
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

            if (GetUserType() == "student")
            {                
                if(idInductor == "")
                    idInductor = await UsersManager.Instance.GetInductorByStudent(GetUserId());
                else
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
        SetSnapshot(await UsersManager.Instance.GetUserAsync("Inductors", idInductor));

        if (GetSnapshot() == null)
        {
            Debug.Log("saca al neo porque no existe sala");
            InitializeAtributes();
            authFirebase.SignOut();
        }
    }    

    void ShowTrivias()
    {
        TriviasChallengesManager.Instance.ShowTriviasStudent(idInductor, triviaInProgress, canvasMenuStudent, canvasPuntuacionesStudent);
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
                ScenesManager.Instance.LoadNewCanvas(canvasGeneralSessions);
                
                if (GetUserType() == "inductor")
                {
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasMenuInductor);
                }
                else if (GetUserType() == "student")
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
                Debug.Log("inicio " + GetUserId());
                if (GetUserType() == "inductor")
                {
                    ScenesManager.Instance.DeleteCurrentCanvas(canvasLoginInductor);
                    ScenesManager.Instance.LoadNewCanvas(canvasNombreInductor);
                    
                }
                else if (GetUserType() == "student")
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
        });
    }

    public async void SignInStudent()
    {
        string name = inputFieldName.text;
        string document = inputFieldDocument.text;
        string idRoom = null;

        if(name != "" && document != "")
        {
            if (!await UsersManager.Instance.ExistUserByDocument("Students", document))
            {
                if (!await DataBaseManager.Instance.IsEmptyTable("Rooms"))
                {
                    idRoom = await RoomsManager.Instance.GetAvailableRoom();
                    if(idRoom != null)
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

                            await UsersManager.Instance.PostNewStudent(userFirebase.UserId, name, document, idRoom);                            
                            //SetSnapshot(await UsersManager.Instance.GetUserAsync("Students", userFirebase.UserId));

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
            if (GetUserType() == "inductor")
                await UsersManager.Instance.DeleteSession(userFirebase.UserId);
            else if (GetUserType() == "student")
                await RoomsManager.Instance.DeleteStudentInRoom(userFirebase.UserId);

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
