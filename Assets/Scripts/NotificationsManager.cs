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

    void Awake()
    {
        instance = this;
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
            /*case AuthError.AccountExistsWithDifferentCredentials:
                message = "Ya existe la cuenta con credenciales diferentes";
                break;*/
            case AuthError.MissingPassword:
                message = "Hace falta la contraseña";
                break;
            /*case AuthError.WeakPassword:
                message = "La contraseña es débil";
                break;*/
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
            default:
                message = "Ocurrió un error";
                break;
        }
        return message;
    }
}
