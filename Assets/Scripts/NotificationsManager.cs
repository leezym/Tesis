using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Firebase.Auth;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager instance;
    public Canvas canvasNotificationFailure, canvasNotificationSuccess;
    public Text NotificationFailureText, NotificationSuccessText;
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        NotificationFailureText.text = "";
        NotificationSuccessText.text = "";
    }

    private void Update()
    {
        if (NotificationFailureText.text!="")
        {
            LoadFailureNotificationCanvas(NotificationFailureText.text);
        }
        if (NotificationSuccessText.text != "")
        {
            LoadSuccessNotificationCanvas(NotificationSuccessText.text);
        }
    }
    public void SetFailureNotificationMessage(string message)
    {
        NotificationFailureText.text = message;
    }
    public void SetSuccessNotificationMessage(string message)
    {
        NotificationSuccessText.text = message;
    }

    public string GetErrorMessage(Firebase.FirebaseException firebaseEx)
    {
        if (firebaseEx != null)
        {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            return TranslateErrorMessage(errorCode);
        }
        return firebaseEx.ToString();
    }

    private static string TranslateErrorMessage(AuthError errorCode)
    {
        string message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Ya existe la cuenta con credenciales diferentes";
                break;
            case AuthError.MissingPassword:
                message = "Hace falta la contraseña";
                break;
            case AuthError.WeakPassword:
                message = "La contraseña es débil";
                break;
            case AuthError.WrongPassword:
                message = "La contraseña es incorrecta";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Ya existe la cuenta con ese correo electrónico";
                break;
            case AuthError.InvalidEmail:
                message = "Correo electrónico inválido";
                break;
            case AuthError.MissingEmail:
                message = "Hace falta el correo electrónico";
                break;
            case AuthError.InvalidCredential:
                message = "Credencial inválida";
                break;
            default:
                message = "Ocurrió un error";
                break;
        }
        return message;
    }

    public void LoadFailureNotificationCanvas(string message){
        NotificationFailureText.enabled = false;
        NotificationFailureText.enabled = true;
        NotificationFailureText.text = message;
        ScenesManager.instance.LoadNewCanvas(canvasNotificationFailure);
    }

    public void LoadSuccessNotificationCanvas(string message){
        NotificationSuccessText.enabled = false;
        NotificationSuccessText.enabled = true;
        NotificationSuccessText.text = message;
        ScenesManager.instance.LoadNewCanvas(canvasNotificationSuccess);
    }
}
