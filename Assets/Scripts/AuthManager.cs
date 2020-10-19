using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    // Firebase
    protected Firebase.Auth.FirebaseAuth authFirebase;
    protected Firebase.Auth.FirebaseUser userFirebase;
    private bool signedIn;

    // DataBase
    private DataSnapshot snapshot;

    // Canvas
    public Canvas canvasLoginInductor, canvasMenuInductor;
    public Canvas canvasLoginStudent, canvasMenuStudent;
    public InputField inputFieldUser, inputFieldPassword;
    public InputField inputFieldDocument, inputFieldName;
    public InputField textRoomName;
    public Text textUserName;

    // UserData
    private string user;
    private bool isInductor;

    public DataSnapshot GetSnapshot() { return snapshot; }
    public void SetSnapshot(DataSnapshot snapshot) { this.snapshot = snapshot; }

    public bool GetIsInductor() { return isInductor; }
    public void SetIsInductor(bool isInductor) { this.isInductor = isInductor; }

    private void Awake() 
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

    private void Update()
    {
        if (signedIn && GetSnapshot() != null)
        {
            if (GetIsInductor()){
                textRoomName.text = GetSnapshot().Child("room").GetValue(true).ToString();
            }
            else
            {

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

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string userId = AuthManager.instance.GetIdUser();
        SetSnapshot(args.Snapshot.Child(userId));
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (authFirebase.CurrentUser != userFirebase)
        {
            signedIn = userFirebase != authFirebase.CurrentUser && authFirebase.CurrentUser != null;
            if (!signedIn && userFirebase != null)
            {
                Debug.Log("Signed out " + userFirebase.Email);
                //DataBaseManager.instace.DeleteUser("Inductors", userFirebase.UserId);
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
        user = inputFieldUser.text;
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

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: ", newUser.Email);
            UsersManager.instance.PostNewInductor(newUser.UserId, "Sala de "+user, newUser.Email);
        });

        authFirebase.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
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
                
            UsersManager.instance.GetUser("Inductors").ValueChanged += HandleValueChanged;
        });
    }

    public void SignInStudent()
    {
        string document = inputFieldDocument.text;
        string name = inputFieldName.text;

        authFirebase.SignInAnonymouslyAsync().ContinueWith(task => {
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

            Firebase.Auth.FirebaseUser newUser = task.Result;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            //UsersManager.instance.PostNewInductor(userFirebase.UserId, "Sala de "+user, userFirebase.Email);
            
            UsersManager.instance.GetUser("Students").ValueChanged += HandleValueChanged;
            ScenesManager.instance.DeleteCurrentCanvas(canvasLoginStudent);
            ScenesManager.instance.LoadNewCanvas(canvasMenuStudent);
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
