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

    string hintName = "";

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
        Dictionary<string, object> hint = await HintsManager.instance.GetHintByName(hintText);
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
        string nameButton = button.name;
        if (nameButton == "HintNameEditButton")
        {
            imageEditName.enabled = false;
            imageCancelName.enabled = true;
            imageSaveName.enabled = true;
            inputHintNameDetail.interactable = true;
        }
        else if (nameButton == "HintDescriptionEditButton")
        {
            imageEditDescription.enabled = false;
            imageCancelDescription.enabled = true;
            imageSaveDescription.enabled = true;
            inputHintDescriptionDetail.interactable = true;
        }
        else if (nameButton == "HintAnswerEditButton")
        {
            imageEditAnswer.enabled = false;
            imageCancelAnswer.enabled = true;
            imageSaveAnswer.enabled = true;
            inputHintAnswerDetail.interactable = true;
        }
    }

    public void DisableInput(GameObject button)
    {
        string nameButton = button.name;
        if (nameButton == "HintNameSaveButton" || nameButton == "HintNameCancelButton")
        {
            imageEditName.enabled = true;
            imageCancelName.enabled = false;
            imageSaveName.enabled = false;
            inputHintNameDetail.interactable = false;
        }
        else if (nameButton == "HintDescriptionSaveButton" || nameButton == "HintDescriptionCancelButton")
        {
            imageEditDescription.enabled = true;
            imageCancelDescription.enabled = false;
            imageSaveDescription.enabled = false;
            inputHintDescriptionDetail.interactable = false;
        }
        else if (nameButton == "HintAnswerSaveButton" || nameButton == "HintAnswerCancelButton")
        {
            imageEditAnswer.enabled = true;
            imageCancelAnswer.enabled = false;
            imageSaveAnswer.enabled = false;
            inputHintAnswerDetail.interactable = false;
        }
    }

    public async void SaveInput(GameObject button)
    {
        DisableInput(button);
        Dictionary<string, object> newHintData = new Dictionary<string, object>
        {
            { "name" , inputHintNameDetail.text},
            { "description" , inputHintDescriptionDetail.text},
            { "answer" , inputHintAnswerDetail.text}
        };
        await HintsManager.instance.PutHintAsync(hintName, newHintData);
    }
}
