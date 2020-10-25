using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

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
    public Canvas canvasLoginInductor, canvasMenuInductor;
    public Canvas canvasLoginStudent, canvasMenuStudent;
    public InputField inputFieldUser, inputFieldPassword;
    public InputField inputFieldDocument, inputFieldName;
    public InputField textRoomName;
    public Text textUserName;

    // UserData
    private bool isInductor;

    public Dictionary<string, object> GetSnapshot() { return snapshot; }
    public void SetSnapshot(Dictionary<string, object> snapshot) { this.snapshot = snapshot; }

    public bool GetIsInductor() { return isInductor; }
    public void SetIsInductor(bool isInductor) { this.isInductor = isInductor; }

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
    }

    private void InitializeFirebase()
    {
        authFirebase = Firebase.Auth.FirebaseAuth.DefaultInstance;
        authFirebase.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void Update()
    {    

        if (signedIn && GetSnapshot() != null)
        {
            if (GetIsInductor()){
                textRoomName.text = GetSnapshot()["room"].ToString();
            }
            else
            {
                //textUserName.text = GetSnapshot()["name"].ToString();
            }

            SetSnapshot(null);
        }
    }

    public string GetIdUser()
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
                Debug.Log("Signed out " + userFirebase.Email);
                //UsersManager.instance.DeleteUser("Inductors", userFirebase.UserId);
            }
            userFirebase = authFirebase.CurrentUser;
            if (signedIn)
            {
                if (GetIsInductor())
                {
                    ScenesManager.instance.DeleteCurrentCanvas(canvasLoginInductor);
                    ScenesManager.instance.LoadNewCanvas(canvasMenuInductor);
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
        //auth = null;
        DeleteUser();
    }

    public void SignInInductor() {
        string user = inputFieldUser.text;
        string password = inputFieldPassword.text;
        string email = user + "@javerianacali.edu.co";
        authFirebase.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                //Firebase.FirebaseException error 
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            
            UsersManager.instance.PostNewInductor(userFirebase.UserId, "Sala de "+user, userFirebase.Email);
        });

        authFirebase.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(async task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            
            SetSnapshot(await UsersManager.instance.GetUserAsync("Inductors", userFirebase.UserId));
        });
    }

    public void SignInStudent()
    {
        string name = inputFieldName.text;
        string document = inputFieldDocument.text;

        authFirebase.SignInAnonymouslyAsync().ContinueWith(async task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            
            UsersManager.instance.PostNewStudent(userFirebase.UserId, name, document);
            SetSnapshot(await UsersManager.instance.GetUserAsync("Students", userFirebase.UserId));
        });
    }


    public void DeleteUser() {
        userFirebase = authFirebase.CurrentUser;
        if (userFirebase != null)
        {
            userFirebase.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                authFirebase.SignOut(); // cerrar sesion
                Debug.Log("User deleted successfully.");
            });
        }
    }
}
