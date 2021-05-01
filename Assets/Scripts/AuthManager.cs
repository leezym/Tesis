using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance { get => instance; set => instance = value; }

    // Firebase
    protected Firebase.Auth.FirebaseAuth authFirebase;
    protected Firebase.Auth.FirebaseUser userFirebase;
    Firebase.FirebaseApp app;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    private bool signedIn;
    GoogleMap googleMap;

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
    public Button signInInductor;

    [Header("CANVAS ESTUDIANTES")]
    public Canvas canvasLoginStudent;
    public Canvas canvasMenuStudent;
    public Canvas canvasPuntuacionesStudent;
    public Canvas canvasGeoMap;
    public CanvasGroup canvasARMap;
    public Canvas[] canvasTriviasStudent;
    public InputField inputFieldDocument;
    public InputField inputFieldName;
    public Button signInStudent;
    
    // UserData
    float currentLatitude = 0, currentLongitude = 0;
    [HideInInspector]
    public float newLatitude = 0, newLongitude = 0;

    public Dictionary<string, object> GetSnapshot() { return snapshot; }
    public void SetSnapshot(Dictionary<string, object> snapshot) { this.snapshot = snapshot; }

    private Dictionary<string, Inductor> inductorsData = new Dictionary<string, Inductor>();

    void Awake() 
    {
        instance = this;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                DataBaseManager.Instance.reference = FirebaseFirestore.DefaultInstance;
                InitializeFirebase();
                //ScenesManager.Instance.LoadNewCanvas(canvasGeneralSessions);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                // Firebase Unity SDK is not safe to use here.
                NotificationsManager.Instance.SetFailureNotificationMessage("No se puede inicializar Firebase");
            }
        });
    }

    void Start()
    { 
        InitializeAtributes();
        googleMap = GameObject.FindObjectOfType<GoogleMap>();
    }

    protected virtual void InitializeFirebase()
    {
        authFirebase = FirebaseAuth.DefaultInstance;
        //authFirebase.SignOut();
        authFirebase.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void InitializeAtributes() 
    {
        GlobalDataManager.Instance.userType = "";
        GlobalDataManager.Instance.idInductorByStudent = "";

        inputFieldUser.text = "";
        inputFieldPassword.text = "";
        inputFieldDocument.text = "";
        inputFieldName.text = "";
        inputRoomName.text = "";
        inputInductorRoomSize.text = "";
    }

    async void Update()
    {
        // Cerrar sesión con le botón de atrás del dispositivo
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            // Cerrar sesion o Salir
            userFirebase = authFirebase.CurrentUser;
            if (userFirebase != null)
                LogOut();
            else
                Application.Quit();
        }
        
        // Sacar a un inductor o neojaveriano por si cerró la aplicación y dejó la sesión abierta
        if(canvasGeneralSessions.enabled)
        {
            userFirebase = authFirebase.CurrentUser;
            if (userFirebase != null)
            {   
                GlobalDataManager.Instance.idUserInductor = await UsersManager.Instance.GetInductorIdByAuth(userFirebase.UserId);
                if(GlobalDataManager.Instance.idUserInductor != null){
                    GlobalDataManager.Instance.idRoomByInductor = await DataBaseManager.Instance.SearchId("Rooms", "idInductor", GlobalDataManager.Instance.idUserInductor);
                    await UsersManager.Instance.DeleteSession();
                }
                /*else
                {
                    GlobalDataManager.Instance.idUserStudent = userFirebase.UserId;
                    if(GlobalDataManager.Instance.idUserStudent != null)
                        await RoomsManager.Instance.DeleteStudentInRoom();
                }*/

                InitializeAtributes();
                authFirebase.SignOut();
            }
        }

        if (signedIn)
        {                 
            LoadingScreenManager.Instance.ShowFinalRankingYincana();
            
            // Actualizar la latitud y longitud en la DB
            if (GlobalDataManager.Instance.idUserStudent != "" || GlobalDataManager.Instance.idUserInductor != "")
            {
                StartCoroutine(MapManager.Instance.Location());
                newLatitude = googleMap.centerLocation.latitude;
                newLongitude = googleMap.centerLocation.longitude;

                if(currentLatitude != newLatitude || currentLongitude != newLongitude)
                {
                    MapManager.Instance.PutLocation(newLatitude, newLongitude);
                    currentLatitude = newLatitude;
                    currentLongitude = newLongitude;
                }
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
            InitializeAtributes();
            authFirebase.SignOut();
        }
    }    

    void ShowTrivias()
    {
        TriviasChallengesManager.Instance.ShowTriviasStudent(GlobalDataManager.Instance.idInductorByStudent);
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
                Debug.Log("salir " + GlobalDataManager.Instance.userType);
                ScenesManager.Instance.DeleteCurrentCanvas(LoadingScreenManager.Instance.canvasRankingFinal);
                
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
                    foreach(Canvas canvas in canvasTriviasStudent)
                    {
                        ScenesManager.Instance.DeleteCurrentCanvas(canvas);
                    }
                }
                ScenesManager.Instance.LoadNewCanvas(canvasGeneralSessions);
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
        if (authFirebase != null) {
            authFirebase.StateChanged -= AuthStateChanged;
            //authFirebase = null;
            Exit();            
        }
    }

    public async void SignInInductor() {
        string user = inputFieldUser.text.ToLower();
        string password = inputFieldPassword.text;
        string email = user + "@javerianacali.edu.co";
        
        if(password != "" && email != "")
        {
            if (!await UsersManager.Instance.ExistUserByUser("Inductors", user))
            {
                signInInductor.interactable = false;
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
                    
                    await UsersManager.Instance.PostNewInductor(GetUserId(), "", user);
                    GlobalDataManager.Instance.idUserInductor = await UsersManager.Instance.GetInductorIdByAuth(GetUserId());
                    //GameObject.FindObjectOfType<ListTrivias>().SearchBuilding();
                    signInInductor.interactable = true;
                });
            }else{
                NotificationsManager.Instance.SetFailureNotificationMessage("Ya inició sesión un usuario con esas credenciales.");
            }

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
                        signInStudent.interactable = false;
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
                            GlobalDataManager.Instance.nameInductor = (await DataBaseManager.Instance.SearchAttribute("Inductors", GlobalDataManager.Instance.idInductorByStudent, "name")).ToString();
                            signInStudent.interactable = true;
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
            Application.Quit();
           
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
        NotificationsManager.Instance.acceptQuestionButton.onClick.RemoveAllListeners();     
    }

}
