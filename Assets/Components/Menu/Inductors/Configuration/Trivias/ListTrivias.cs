using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Firebase.Firestore;
using System;

public class ListTrivias : MonoBehaviour
{
    public GameObject triviaPrefab;
    public Transform triviaContent;
    public Canvas canvasConfigTrivias, canvasAddTrivias;
    public Dropdown buildingsDropdown;
    public Button addTrivia;

    List<Dictionary<string,object>> triviasList = new List<Dictionary<string,object>>();
    public List<GameObject> currentTrivias = new List<GameObject>();

    public InputField inputTriviaQuestion, inputTriviaAnswerOne, inputTriviaAnswerTwo, inputTriviaAnswerThree;
    public Toggle toggleTriviaAnswerOne, toggleTriviaAnswerTwo, toggleTriviaAnswerThree;
    public Text textTriviaAnswerOne, textTriviaAnswerTwo, textTriviaAnswerThree;
    public Text placeholderTriviaAnswerOne, placeholderTriviaAnswerTwo, placeholderTriviaAnswerThree;
    public Sprite backgroundCorrectAnswer, backgroundRegularAnswer;
    
    [HideInInspector]
    public string buildingName = "", idBuilding = "";
    public int currentAmongQuestions = 0;

    void Start()
    {
        InitializeAtributes();
        /*buildingsDropdown.onValueChanged.AddListener(delegate {
            SelectBuilding();
        });*/
    }

    public void InitializeAtributes() 
    {
        inputTriviaQuestion.text = "";
        inputTriviaAnswerOne.text = "";
        inputTriviaAnswerTwo.text = "";
        inputTriviaAnswerThree.text = "";
        toggleTriviaAnswerOne.isOn = true;
        toggleTriviaAnswerTwo.isOn = false;
        toggleTriviaAnswerThree.isOn = false;
    }

    /*public async void SearchBuilding(){
        List<DocumentSnapshot> buildingsList = await DataBaseManager.Instance.SearchByCollection("Buildings");
        foreach(DocumentSnapshot buildings in buildingsList){
            foreach(KeyValuePair<string,object> pair in buildings.ToDictionary()){
                if(pair.Key == "name"){
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = pair.Value.ToString();
                    buildingsDropdown.options.Add(option);
                }
            }
        }
    }*/

    public async void SelectBuilding()
    {
        int valueBuilding = buildingsDropdown.value;
        buildingName = buildingsDropdown.options[valueBuilding].text;
        triviasList = await TriviasManager.Instance.GetTriviaByBuilding(buildingName);

        if(buildingsDropdown.value != 0)
            SearchTrivias();
        else
            NotificationsManager.Instance.SetFailureNotificationMessage("Por favor selecciona un edificio para agregar una pregunta.");
    }

    void ClearCurrentTrivias()
    {
        // Vaciar lista y borrar pistas actuales
        foreach(GameObject hint in currentTrivias)
        {
            Destroy(hint);
        }
        currentTrivias.Clear();
    }

    public void EnableToggle(Toggle toggle)
    {
        if (toggle.gameObject == toggleTriviaAnswerOne.gameObject && toggleTriviaAnswerOne.isOn)
        {
            inputTriviaAnswerOne.image.sprite = backgroundCorrectAnswer;
            textTriviaAnswerOne.color = Color.black;
            placeholderTriviaAnswerOne.color = Color.black;
            toggleTriviaAnswerOne.interactable = false;
            toggleTriviaAnswerTwo.isOn = false;
            toggleTriviaAnswerThree.isOn = false;
        }
        if (toggle.gameObject == toggleTriviaAnswerTwo.gameObject && toggleTriviaAnswerTwo.isOn)
        {
            inputTriviaAnswerTwo.image.sprite = backgroundCorrectAnswer;
            textTriviaAnswerTwo.color = Color.black;
            placeholderTriviaAnswerTwo.color = Color.black;
            toggleTriviaAnswerTwo.interactable = false;
            toggleTriviaAnswerOne.isOn = false;
            toggleTriviaAnswerThree.isOn = false;
        }
        if (toggle.gameObject == toggleTriviaAnswerThree.gameObject && toggleTriviaAnswerThree.isOn)
        {
            inputTriviaAnswerThree.image.sprite = backgroundCorrectAnswer;
            textTriviaAnswerThree.color = Color.black;
            placeholderTriviaAnswerThree.color = Color.black;
            toggleTriviaAnswerThree.interactable = false;
            toggleTriviaAnswerOne.isOn = false;
            toggleTriviaAnswerTwo.isOn = false;
        }
    }

    public void DisableToggle(Toggle toggle)
    {
        if (toggle.gameObject == toggleTriviaAnswerOne.gameObject && !toggleTriviaAnswerOne.isOn)
        {
            toggleTriviaAnswerOne.interactable = true;
            inputTriviaAnswerOne.image.sprite = backgroundRegularAnswer;
            textTriviaAnswerOne.color = Color.white;
            placeholderTriviaAnswerOne.color = Color.white;
        }
        if (toggle.gameObject == toggleTriviaAnswerTwo.gameObject && !toggleTriviaAnswerTwo.isOn)
        {
            toggleTriviaAnswerTwo.interactable = true;
            inputTriviaAnswerTwo.image.sprite = backgroundRegularAnswer;
            textTriviaAnswerTwo.color = Color.white;
            placeholderTriviaAnswerTwo.color = Color.white;
        }
        if (toggle.gameObject == toggleTriviaAnswerThree.gameObject && !toggleTriviaAnswerThree.isOn)
        {
            toggleTriviaAnswerThree.interactable = true;
            inputTriviaAnswerThree.image.sprite = backgroundRegularAnswer;
            textTriviaAnswerThree.color = Color.white;
            placeholderTriviaAnswerThree.color = Color.white;
        }
    }

    void SearchTrivias()
    {
        ClearCurrentTrivias();

        foreach(Dictionary<string,object> trivia in triviasList)
        {
            foreach(KeyValuePair<string,object> pair in trivia)
            {
                if(pair.Key == "question"){
                    // Instanciar prefab
                    GameObject triviaElement = Instantiate (triviaPrefab, new Vector3(triviaContent.position.x, triviaContent.position.y, triviaContent.position.z) , Quaternion.identity);
                    triviaElement.transform.parent = triviaContent.transform;
                    
                    // Editar texto
                    Text triviaText = triviaElement.GetComponentInChildren<Text>();
                    triviaText.text = pair.Value.ToString();

                    // Añadir a Lista
                    currentTrivias.Add(triviaElement);
                }
            }
        }
    }
    

    string DetectCorrectAnswer()
    {
        string answer = "";
        if (toggleTriviaAnswerOne.isOn)
            answer = inputTriviaAnswerOne.text;
        if (toggleTriviaAnswerTwo.isOn)
            answer = inputTriviaAnswerTwo.text;
        if (toggleTriviaAnswerThree.isOn)
            answer = inputTriviaAnswerThree.text;

        return answer;
    }

    public bool CheckNewData()
    {
        if( inputTriviaQuestion.text != "" && inputTriviaAnswerOne.text != "" && inputTriviaAnswerTwo.text != "" && inputTriviaAnswerThree.text != "")
        {            
            return true;
        }
        return false;
    }

    public async void SaveTrivia()
    {
        if (CheckNewData())
        {
            List<Dictionary<string, object>> newTrivia = await TriviasManager.Instance.GetTriviaByQuestion(inputTriviaQuestion.text);
            if (newTrivia.Count == 0)
            {
                if(buildingName == GlobalDataManager.nameBuildingPalmas)
                {
                    idBuilding = GlobalDataManager.idBuildingPalmas;
                }
                else if(buildingName == GlobalDataManager.nameBuildingGuayacanes)
                {
                    idBuilding = GlobalDataManager.idBuildingGuayacanes;
                }
                else if(buildingName == GlobalDataManager.nameBuildingLago)
                {
                    idBuilding = GlobalDataManager.idBuildingLago;
                }
                else if(buildingName == GlobalDataManager.nameBuildingRaulPosada)
                {
                    idBuilding = GlobalDataManager.idBuildingRaulPosada;
                }
                
                currentAmongQuestions = Convert.ToInt32(await DataBaseManager.Instance.SearchAttribute("Buildings", idBuilding, "amongQuestions"));
                // Actualizar Edificios
                Dictionary<string, object> newBuildingData = new Dictionary<string, object>
                {
                    {"amongQuestions", currentAmongQuestions + 1 }
                };
                await DataBaseManager.Instance.UpdateAsync("Buildings", idBuilding, newBuildingData);

                // Actualizar Trivias
                await TriviasManager.Instance.PostNewTrivia(idBuilding, inputTriviaQuestion.text, inputTriviaAnswerOne.text, inputTriviaAnswerTwo.text, inputTriviaAnswerThree.text, DetectCorrectAnswer());
                ScenesManager.Instance.LoadNewCanvas(canvasConfigTrivias);
                ScenesManager.Instance.DeleteCurrentCanvas(canvasAddTrivias);
                ClearCurrentTrivias();
                NotificationsManager.Instance.SetSuccessNotificationMessage("Pregunta creada.");
            }
            else
                NotificationsManager.Instance.SetFailureNotificationMessage("Ya existe esa pregunta. Por favor intenta con otra.");
        }else
            NotificationsManager.Instance.SetFailureNotificationMessage("Campos Incompletos. Por favor llene todos los campos.");
    }   
}
