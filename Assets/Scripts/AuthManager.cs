using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    // Firebase
    protected Firebase.Auth.FirebaseAuth authFirebase;
    protected Firebase.Auth.FirebaseUser userFirebase;
    private bool signedIn;

    // DataBase
    private Dictionary<string, object> snapshot;

    // Canvas
    public Canvas canvasLoginInductor, canvasNombreInductor, canvasMenuInductor;
    public Canvas canvasLoginStudent, canvasMenuStudent;
    public InputField inputFieldUser, inputFieldPassword, inputRoomName;
    public InputField inputFieldDocument, inputFieldName, inputInductorRoomSize;
    public Text textUserName;

    // UserData
    private bool isInductor;

    public Dictionary<string, object> GetSnapshot() { return snapshot; }
    public void SetSnapshot(Dictionary<string, object> snapshot) { this.snapshot = snapshot; }

    public void SetIsInductor(bool isInductor) { this.isInductor = isInductor; }
    public bool GetIsInductor() { return isInductor; }

    private Dictionary<string, Inductor> inductorsData = new Dictionary<string, Inductor>();

    public void Awake() 
    {
        instance = this;
        InitializeFirebase();
        InitializeAtributes();
    }

    public void InitializeAtributes() 
    {
        inputFieldUser.text = "";
        inputFieldPassword.text = "";
        inputFieldDocument.text = "";
        inputFieldName.text = "";
        inputRoomName.text = "";
        inputInductorRoomSize.text = "0";
    }

    private void InitializeFirebase()
    {
        authFirebase = Firebase.Auth.FirebaseAuth.DefaultInstance;
        authFirebase.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public async void Update()
    {

        if (signedIn && !GetIsInductor())
        {
            SetSnapshot(await UsersManager.instance.GetUserAsync("Students", GetUserId()));
            if (GetSnapshot() == null)
            {
                DeleteUser();
                GameObject.Find("PanelGeneralSessions").GetComponent<Canvas>().enabled = true;
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
                if (GetIsInductor())
                {
                    Debug.Log("Se salio el inductor");
                }
                else
                {
                    Debug.Log("Se salio el neo");
                }
            }
            userFirebase = authFirebase.CurrentUser;
            if (signedIn)
            {
                if (GetIsInductor())
                {
                    ScenesManager.instance.DeleteCurrentCanvas(canvasLoginInductor);
                    ScenesManager.instance.LoadNewCanvas(canvasNombreInductor);
                }
                else
                {
                    ScenesManager.instance.DeleteCurrentCanvas(canvasLoginStudent);
                    ScenesManager.instance.LoadNewCanvas(canvasMenuStudent);
                }     
                
            }
        }
    }

    void OnDestroy()
    {
        authFirebase.StateChanged -= AuthStateChanged;
        authFirebase = null;
        //DeleteUser();
    }

    public void SignInInductor() {
        string user = inputFieldUser.text;
        string password = inputFieldPassword.text;
        string email = user + "@javeyrianacali.edu.co";

        authFirebase.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(taskSignIn => {                
            if (taskSignIn.IsFaulted)
            {
                foreach (System.Exception exception in taskSignIn.Exception.InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                    string message = NotificationsManager.instance.GetErrorMessage(firebaseEx);
                    Debug.Log("El error es: " + message);
                }
                return;
            } 
            if (taskSignIn.IsCompleted)
            {
                //SetSnapshot(await UsersManager.instance.GetUserAsync("Inductors", userFirebase.UserId));
                UsersManager.instance.PostNewInductor(userFirebase.UserId, user, userFirebase.Email, inputRoomName.text);       
                RoomsManager.instance.PostNewRoom("Grupo de " + user, Convert.ToInt32(inputInductorRoomSize.text), userFirebase.UserId);
            }
        });

        /*authFirebase.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(taskCreate => {
            if (taskCreate.IsFaulted)
            {
                foreach (System.Exception exception in taskCreate.Exception.InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                    string message = NotificationsManager.instance.GetErrorMessage(firebaseEx);
                    Debug.Log("El error es: " + message);
                }
                return;
            }

            if (taskCreate.IsCompleted)
            {
                Debug.Log("se supone que entra");
                UsersManager.instance.PostNewInductor(userFirebase.UserId, user, userFirebase.Email, inputRoomName.text);
                authFirebase.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(taskSignIn => {                
                    if (taskSignIn.IsFaulted)
                    {
                        foreach (System.Exception exception in taskSignIn.Exception.InnerExceptions)
                        {
                            Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                            string message = NotificationsManager.instance.GetErrorMessage(firebaseEx);
                            Debug.Log("El error es: " + message);
                        }
                        return;
                    } 
                    
                    //SetSnapshot(await UsersManager.instance.GetUserAsync("Inductors", userFirebase.UserId));
                    RoomsManager.instance.PostNewRoom("Grupo de " + user, Convert.ToInt32(inputInductorRoomSize.text), userFirebase.UserId);
                });
            }                                     
        });*/      
    }

    public async void SignInStudent()
    {
        string name = inputFieldName.text;
        string document = inputFieldDocument.text;
        string idRoom = null;

        if (!await DataBaseManager.instance.IsEmptyTable("Rooms"))
        {            
            idRoom = await RoomsManager.instance.SearchAvailableRoom();
            if(idRoom != null)
            {
                await authFirebase.SignInAnonymouslyAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        foreach (System.Exception exception in task.Exception.InnerExceptions)
                        {
                            Firebase.FirebaseException firebaseEx = exception.InnerException as Firebase.FirebaseException;
                            string message = NotificationsManager.instance.GetErrorMessage(firebaseEx);
                            Debug.Log("El error es: " + message);
                        }
                        return;
                    }
                    
                    UsersManager.instance.PostNewStudent(userFirebase.UserId, name, document, idRoom);
                    //SetSnapshot(await UsersManager.instance.GetUserAsync("Students", userFirebase.UserId));
                    
                    //ScenesManager.instance.DeleteCurrentCanvas(canvasLoginStudent);
                    //ScenesManager.instance.LoadNewCanvas(canvasMenuStudent);
                });
            }
        }
    }


    public async void DeleteUser() {
        userFirebase = authFirebase.CurrentUser;
        if (userFirebase != null)
        {
            string idUser = userFirebase.UserId;
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
