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

    private void Awake()
    {
        instance = this;
    }

    public string GetErrorMessage(Exception exception)
    {
        Debug.Log("Estoy en GetErrorMessage");
        Debug.Log(exception.ToString());
        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
        if (firebaseEx != null)
        {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            return TranslateErrorMessage(errorCode);
        }

        return exception.ToString();
    }

    private static string TranslateErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Ya existe la cuenta con credenciales diferentes";
                break;
            case AuthError.MissingPassword:
                message = "Hace falta el Password";
                break;
            case AuthError.WeakPassword:
                message = "El password es debil";
                break;
            case AuthError.WrongPassword:
                message = "El password es Incorrecto";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Ya existe la cuenta con ese correo electrónico";
                break;
            case AuthError.InvalidEmail:
                message = "Correo electronico invalido";
                break;
            case AuthError.MissingEmail:
                message = "Hace falta el correo electrónico";
                break;
            default:
                message = "Ocurrió un error";
                break;
        }
        return message;
    }
}
