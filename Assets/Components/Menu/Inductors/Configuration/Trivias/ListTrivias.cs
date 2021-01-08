using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ListTrivias : MonoBehaviour
{
    public GameObject triviaPrefab;
    public Transform triviaContent;
    public Canvas canvasConfigTrivias, canvasAddTrivias;
    public Dropdown buildingsDropdown;
    public Button addTrivia;
    List<Dictionary<string,object>> buildingsList = new List<Dictionary<string,object>>();

    List<Dictionary<string,object>> triviasList = new List<Dictionary<string,object>>();
    public List<GameObject> currentTrivias = new List<GameObject>();
    int currentSizeTrivias = 0, newSizeTrivias = 0;

    public InputField inputTriviaQuestion, inputTriviaAnswerOne, inputTriviaAnswerTwo, inputTriviaAnswerThree;
    public Toggle toggleTriviaAnswerOne, toggleTriviaAnswerTwo, toggleTriviaAnswerThree;
    public Text textTriviaAnswerOne, textTriviaAnswerTwo, textTriviaAnswerThree;
    public Text placeholderTriviaAnswerOne, placeholderTriviaAnswerTwo, placeholderTriviaAnswerThree;
    public Sprite backgroundCorrectAnswer, backgroundRegularAnswer;
    
    string buildingName = "";

    async void Start()
    {
        InitializeAtributes();
        await SearchBuilding();
        buildingsDropdown.onValueChanged.AddListener(delegate {
            SelectBuilding();
        });
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
        buildingsDropdown.value = 0;
    }

    void Update()
    {
        if(buildingsDropdown.value != 0 && canvasConfigTrivias.enabled)
        {
            SearchTrivias();
            addTrivia.enabled = true;
        }
        else
        {
            addTrivia.enabled = false;
        }
    }

    async Task SearchBuilding(){
        buildingsList = await DataBaseManager.instance.SearchByCollection("Buildings");
        foreach(Dictionary<string,object> buildings in buildingsList){
            foreach(KeyValuePair<string,object> pair in buildings){
                if(pair.Key == "name"){
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = pair.Value.ToString();
                    buildingsDropdown.options.Add(option);
                }
            }
        }
    }

    public void SelectBuilding()
    {
        int valueBuilding = buildingsDropdown.value;
        buildingName = buildingsDropdown.options[valueBuilding].text;
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

    public async void SearchTrivias()
    {
        triviasList = await TriviasManager.instance.GetTriviaByBuilding(buildingName);
        newSizeTrivias = triviasList.Count;

        if (currentSizeTrivias != newSizeTrivias)
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
            currentSizeTrivias = newSizeTrivias;
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
            List<Dictionary<string, object>> newTrivia = await TriviasManager.instance.GetTriviaByQuestion(inputTriviaQuestion.text);
            if (newTrivia.Count == 0)
            {
                string idBuilding = await DataBaseManager.instance.SearchId("Buildings", "name", buildingName);

                // Actualizar Edificios
                Dictionary<string, object> newBuildingData = new Dictionary<string, object>
                {
                    {"amongQuestions", currentSizeTrivias + 1 }
                };
                await DataBaseManager.instance.UpdateAsync("Buildings", idBuilding, newBuildingData);

                // Actualizar Trivias
                await TriviasManager.instance.PostNewTrivia(idBuilding, inputTriviaQuestion.text, inputTriviaAnswerOne.text, inputTriviaAnswerTwo.text, inputTriviaAnswerThree.text, DetectCorrectAnswer());
                ScenesManager.instance.LoadNewCanvas(canvasConfigTrivias);
                ScenesManager.instance.DeleteCurrentCanvas(canvasAddTrivias);
                ClearCurrentTrivias();
                NotificationsManager.instance.SetSuccessNotificationMessage("Pregunta creada.");
            }
            else
            NotificationsManager.instance.SetFailureNotificationMessage("Ya existe esa pregunta. Por favor intenta con otra.");
        }else
            NotificationsManager.instance.SetFailureNotificationMessage("Campos Incompletos. Por favor llene todos los campos.");
    }   
}
