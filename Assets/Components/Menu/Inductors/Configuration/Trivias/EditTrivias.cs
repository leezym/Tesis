using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTrivias : MonoBehaviour
{
    public Canvas canvasConfigTrivias, canvasTriviaDetail;
    public InputField inputTriviaQuestionDetail, inputTriviaAnswerOneDetail, inputTriviaAnswerTwoDetail, inputTriviaAnswerThreeDetail; 
    public Text textTriviaAnswerOneDetail, textTriviaAnswerTwoDetail, textTriviaAnswerThreeDetail;
    public Text placeholderTriviaAnswerOneDetail, placeholderTriviaAnswerTwoDetail, placeholderTriviaAnswerThreeDetail;
    public Toggle toggleTriviaAnswerOneDetail, toggleTriviaAnswerTwoDetail, toggleTriviaAnswerThreeDetail;
    public Image imageSaveQuestion, imageSaveAnswer;
    public Image imageCancelQuestion, imageCancelAnswer;
    public Image imageEditQuestion, imageEditAnswer;
    public Sprite backgroundCorrectAnswer, backgroundRegularAnswer;
    
    string idTrivia = "";
    string triviaQuestion = "";
    

    void Start() 
    {
        InitializeAtributes();
    }

    public void InitializeAtributes() 
    {
        inputTriviaQuestionDetail.text = "";
        inputTriviaAnswerOneDetail.text = ""; 
        inputTriviaAnswerTwoDetail.text = "";
        inputTriviaAnswerThreeDetail.text = "";
        imageCancelAnswer.enabled = false;
        imageCancelQuestion.enabled = false;
        imageSaveAnswer.enabled = false;
        imageSaveQuestion.enabled = false;
        toggleTriviaAnswerOneDetail.interactable = false;
        toggleTriviaAnswerTwoDetail.interactable = false;
        toggleTriviaAnswerThreeDetail.interactable = false;
        GameObject.FindObjectOfType<ListTrivias>().buildingsDropdown.value = 0;

    }

    public void EnableToggle(Toggle toggle)
    {
        if (toggle.gameObject == toggleTriviaAnswerOneDetail.gameObject && toggleTriviaAnswerOneDetail.isOn)
        {
            inputTriviaAnswerOneDetail.image.sprite = backgroundCorrectAnswer;
            textTriviaAnswerOneDetail.color = Color.black;
            placeholderTriviaAnswerOneDetail.color = Color.black;
            toggleTriviaAnswerOneDetail.interactable = false;
            toggleTriviaAnswerTwoDetail.isOn = false;
            toggleTriviaAnswerThreeDetail.isOn = false;
        }
        if (toggle.gameObject == toggleTriviaAnswerTwoDetail.gameObject && toggleTriviaAnswerTwoDetail.isOn)
        {
            inputTriviaAnswerTwoDetail.image.sprite = backgroundCorrectAnswer;
            textTriviaAnswerTwoDetail.color = Color.black;
            placeholderTriviaAnswerTwoDetail.color = Color.black;
            toggleTriviaAnswerTwoDetail.interactable = false;
            toggleTriviaAnswerOneDetail.isOn = false;
            toggleTriviaAnswerThreeDetail.isOn = false;
        }
        if (toggle.gameObject == toggleTriviaAnswerThreeDetail.gameObject && toggleTriviaAnswerThreeDetail.isOn)
        {
            inputTriviaAnswerThreeDetail.image.sprite = backgroundCorrectAnswer;
            textTriviaAnswerThreeDetail.color = Color.black;
            placeholderTriviaAnswerThreeDetail.color = Color.black;
            toggleTriviaAnswerThreeDetail.interactable = false;
            toggleTriviaAnswerOneDetail.isOn = false;
            toggleTriviaAnswerTwoDetail.isOn = false;
        }
    }

    public void DisableToggle(Toggle toggle)
    {
        if (toggle.gameObject == toggleTriviaAnswerOneDetail.gameObject && !toggleTriviaAnswerOneDetail.isOn)
        {
            toggleTriviaAnswerOneDetail.interactable = true;
            inputTriviaAnswerOneDetail.image.sprite = backgroundRegularAnswer;
            textTriviaAnswerOneDetail.color = Color.white;
            placeholderTriviaAnswerOneDetail.color = Color.white;
        }
        if (toggle.gameObject == toggleTriviaAnswerTwoDetail.gameObject && !toggleTriviaAnswerTwoDetail.isOn)
        {
            toggleTriviaAnswerTwoDetail.interactable = true;
            inputTriviaAnswerTwoDetail.image.sprite = backgroundRegularAnswer;
            textTriviaAnswerTwoDetail.color = Color.white;
            placeholderTriviaAnswerTwoDetail.color = Color.white;
        }
        if (toggle.gameObject == toggleTriviaAnswerThreeDetail.gameObject && !toggleTriviaAnswerThreeDetail.isOn)
        {
            toggleTriviaAnswerThreeDetail.interactable = true;
            inputTriviaAnswerThreeDetail.image.sprite = backgroundRegularAnswer;
            textTriviaAnswerThreeDetail.color = Color.white;
            placeholderTriviaAnswerThreeDetail.color = Color.white;
        }
    }

    public async void SearchTriviaDetails(string triviaText)
    {        
        idTrivia = await TriviasManager.Instance.GetIdTriviaByQuestion(triviaText);
        Dictionary<string, object> trivia = await TriviasManager.Instance.GetTriviaAsync(idTrivia);
        foreach(KeyValuePair<string, object> pair in trivia)
        {
            if (pair.Key == "question")
                inputTriviaQuestionDetail.text = pair.Value.ToString();
            if (pair.Key == "answerOne")
                inputTriviaAnswerOneDetail.text = pair.Value.ToString();
            if (pair.Key == "answerTwo")
                inputTriviaAnswerTwoDetail.text = pair.Value.ToString();
            if (pair.Key == "answerThree")
                inputTriviaAnswerThreeDetail.text = pair.Value.ToString();
            if (pair.Key == "correctAnswer")
            {
                if (inputTriviaAnswerOneDetail.text == pair.Value.ToString())
                {
                    inputTriviaAnswerOneDetail.image.sprite = backgroundCorrectAnswer;
                    textTriviaAnswerOneDetail.color = Color.black;
                    toggleTriviaAnswerOneDetail.isOn = true;
                }
                if (inputTriviaAnswerTwoDetail.text == pair.Value.ToString())
                {
                    inputTriviaAnswerTwoDetail.image.sprite = backgroundCorrectAnswer;
                    textTriviaAnswerTwoDetail.color = Color.black;
                    toggleTriviaAnswerTwoDetail.isOn = true;
                }
                if (inputTriviaAnswerThreeDetail.text == pair.Value.ToString())
                {
                    inputTriviaAnswerThreeDetail.image.sprite = backgroundCorrectAnswer;
                    textTriviaAnswerThreeDetail.color = Color.black;
                    toggleTriviaAnswerThreeDetail.isOn = true;
                }
            }
        }
        triviaQuestion = inputTriviaQuestionDetail.text;
    }

    public void EnableInput(GameObject button)
    {
        if (button == imageEditQuestion.gameObject)
        {
            imageEditQuestion.enabled = false;
            imageCancelQuestion.enabled = true;
            imageSaveQuestion.enabled = true;
            inputTriviaQuestionDetail.interactable = true;
        }
        else if (button == imageEditAnswer.gameObject)
        {
            imageEditAnswer.enabled = false;
            imageCancelAnswer.enabled = true;
            imageSaveAnswer.enabled = true;
            inputTriviaAnswerOneDetail.interactable = true;
            inputTriviaAnswerTwoDetail.interactable = true;
            inputTriviaAnswerThreeDetail.interactable = true;
            toggleTriviaAnswerOneDetail.interactable = true;
            toggleTriviaAnswerTwoDetail.interactable = true;
            toggleTriviaAnswerThreeDetail.interactable = true;
        }
    }

    public void DisableInput(GameObject button)
    {
        if (button == imageSaveQuestion.gameObject || button == imageCancelQuestion.gameObject)
        {
            imageEditQuestion.enabled = true;
            imageCancelQuestion.enabled = false;
            imageSaveQuestion.enabled = false;
            inputTriviaQuestionDetail.interactable = false;
        }
        else if (button == imageSaveAnswer.gameObject || button == imageCancelAnswer.gameObject)
        {
            imageEditAnswer.enabled = true;
            imageCancelAnswer.enabled = false;
            imageSaveAnswer.enabled = false;
            inputTriviaAnswerOneDetail.interactable = false;
            inputTriviaAnswerTwoDetail.interactable = false;
            inputTriviaAnswerThreeDetail.interactable = false;
            toggleTriviaAnswerOneDetail.interactable = false;
            toggleTriviaAnswerTwoDetail.interactable = false;
            toggleTriviaAnswerThreeDetail.interactable = false;
        }
    }

    string DetectCorrectAnswer()
    {
        string answer = "";
        if (toggleTriviaAnswerOneDetail.isOn)
            answer = inputTriviaAnswerOneDetail.text;
        if (toggleTriviaAnswerTwoDetail.isOn)
            answer = inputTriviaAnswerTwoDetail.text;
        if (toggleTriviaAnswerThreeDetail.isOn)
            answer = inputTriviaAnswerThreeDetail.text;

        return answer;
    }

    public bool CheckNewData()
    {
        if( inputTriviaQuestionDetail.text != "" && inputTriviaAnswerOneDetail.text != "" && inputTriviaAnswerTwoDetail.text != "" && inputTriviaAnswerThreeDetail.text != "") 
        {            
            return true;
        }
        return false;
    }

    public async void SaveInput(GameObject button)
    {
        List<Dictionary<string, object>> trivia = new List<Dictionary<string, object>>();
        if (inputTriviaQuestionDetail.text != triviaQuestion)
            trivia = await TriviasManager.Instance.GetTriviaByQuestion(inputTriviaQuestionDetail.text);
        
        if (trivia.Count == 0)
        {
            if (CheckNewData())
            {
                DisableInput(button);
                Dictionary<string, object> newTriviaData = new Dictionary<string, object>
                {
                    { "question" , inputTriviaQuestionDetail.text},
                    { "answerOne" , inputTriviaAnswerOneDetail.text},
                    { "answerTwo" , inputTriviaAnswerTwoDetail.text},
                    { "answerThree" , inputTriviaAnswerThreeDetail.text},
                    { "correctAnswer", DetectCorrectAnswer()}
                };
                await TriviasManager.Instance.PutTriviaAsync(idTrivia, newTriviaData);
                NotificationsManager.Instance.SetSuccessNotificationMessage("Datos guardados.");
                triviaQuestion = inputTriviaQuestionDetail.text;
            }
            else
                NotificationsManager.Instance.SetFailureNotificationMessage("Campos Incompletos. Por favor llene todos los campos.");
        }
        else
            NotificationsManager.Instance.SetFailureNotificationMessage("Ya existe esa pregunta. Por favor intenta con otra.");
    }

    public void DeleteTrivia()
    {
        NotificationsManager.Instance.SetQuestionNotificationMessage("¿Está seguro que desea eliminar esta pregunta?");
        NotificationsManager.Instance.acceptQuestionButton.onClick.AddListener(Delete);
    }

    async void Delete()
    {
        await TriviasManager.Instance.DeleteTrivia(idTrivia);
        ScenesManager.Instance.LoadNewCanvas(canvasConfigTrivias);
        ScenesManager.Instance.DeleteCurrentCanvas(canvasTriviaDetail);
    }
}
