using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class EditHints : MonoBehaviour
{
    public InputField inputHintNameDetail, inputHintDescriptionDetail, inputHintAnswerDetail;
    public Image imageSaveName, imageSaveDescription, imageSaveAnswer;
    public Image imageCancelName, imageCancelDescription, imageCancelAnswer;
    public Image imageEditName, imageEditDescription, imageEditAnswer;

    string idHint = "";
    string hintName= "";

    void Start() 
    {
        InitializeAtributes();
    }

    public void InitializeAtributes() 
    {
        inputHintNameDetail.text = "";
        inputHintDescriptionDetail.text = ""; 
        inputHintAnswerDetail.text = "";
        imageCancelAnswer.enabled = false;
        imageCancelName.enabled = false;
        imageCancelDescription.enabled = false;
        imageSaveAnswer.enabled = false;
        imageSaveName.enabled = false;
        imageSaveDescription.enabled = false;
    }

    public async void SearchHintDetails(string hintText)
    {        
        idHint = await DataBaseManager.instance.SearchId("Hints", "name", hintText);
        Dictionary<string, object> hint = await HintsManager.instance.GetHintAsync(idHint);
        foreach(KeyValuePair<string, object> pair in hint)
        {
            if (pair.Key == "name")
                inputHintNameDetail.text = pair.Value.ToString();
            if (pair.Key == "description")
                inputHintDescriptionDetail.text = pair.Value.ToString();
            if (pair.Key == "answer")
                inputHintAnswerDetail.text = pair.Value.ToString();
        }
        hintName = inputHintNameDetail.text;
    }

    public void EnableInput(GameObject button)
    {
        if (button == imageEditName.gameObject)
        {
            imageEditName.enabled = false;
            imageCancelName.enabled = true;
            imageSaveName.enabled = true;
            inputHintNameDetail.interactable = true;
        }
        else if (button == imageEditDescription.gameObject)
        {
            imageEditDescription.enabled = false;
            imageCancelDescription.enabled = true;
            imageSaveDescription.enabled = true;
            inputHintDescriptionDetail.interactable = true;
        }
        else if (button == imageEditAnswer.gameObject)
        {
            imageEditAnswer.enabled = false;
            imageCancelAnswer.enabled = true;
            imageSaveAnswer.enabled = true;
            inputHintAnswerDetail.interactable = true;
        }
    }

    public void DisableInput(GameObject button)
    {
        if (button == imageSaveName.gameObject || button == imageCancelName.gameObject)
        {
            imageEditName.enabled = true;
            imageCancelName.enabled = false;
            imageSaveName.enabled = false;
            inputHintNameDetail.interactable = false;
        }
        else if (button == imageSaveDescription.gameObject || button == imageCancelDescription.gameObject)
        {
            imageEditDescription.enabled = true;
            imageCancelDescription.enabled = false;
            imageSaveDescription.enabled = false;
            inputHintDescriptionDetail.interactable = false;
        }
        else if (button == imageSaveAnswer.gameObject || button == imageCancelAnswer.gameObject)
        {
            imageEditAnswer.enabled = true;
            imageCancelAnswer.enabled = false;
            imageSaveAnswer.enabled = false;
            inputHintAnswerDetail.interactable = false;
        }
    }

    public bool CheckNewData()
    {
        if( inputHintNameDetail.text != "" && inputHintDescriptionDetail.text != "" && inputHintAnswerDetail.text != "")
        {            
            return true;
        }
        return false;
    }

    public async void SaveInput(GameObject button)
    {
        List<Dictionary<string, object>> hint = new List<Dictionary<string, object>>();
        if (inputHintNameDetail.text != hintName)
            hint = await HintsManager.instance.GetHintByName(inputHintNameDetail.text);

        if (hint.Count == 0)
        {
            if (CheckNewData())
            {
                DisableInput(button);
                Dictionary<string, object> newHintData = new Dictionary<string, object>
                {
                    { "name" , inputHintNameDetail.text},
                    { "description" , inputHintDescriptionDetail.text},
                    { "answer" , inputHintAnswerDetail.text}
                };
                await HintsManager.instance.PutHintAsync(idHint, newHintData);
                NotificationsManager.instance.SetSuccessNotificationMessage("Datos guardados.");
                hintName = inputHintNameDetail.text;
            }
            else
                NotificationsManager.instance.SetFailureNotificationMessage("Campos Incompletos. Por favor llene todos los campos.");
        }
        else 
            NotificationsManager.instance.SetFailureNotificationMessage("Ya existe una pista con ese nombre. Por favor intenta con otro.");
    }
}
