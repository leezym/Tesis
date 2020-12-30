using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ListHints : MonoBehaviour
{
    public GameObject hintPrefab;
    public Transform hintContent;
    public Canvas canvasConfigHints, canvasAddHints;
    List<Dictionary<string,object>> hintsList = new List<Dictionary<string,object>>();
    List<GameObject> currentHints = new List<GameObject>();
    int currentSizeHints = 0, newSizeHints = 0;

    public InputField inputHintName, inputDescriptionHint, inputAnswerHint;

    //public Dropdown BuildingsDropdown;
    //List<Dictionary<string,object>> BuildingsList = new List<Dictionary<string,object>>();
    
    // Nombre de la DB - Buildings
    void Start()
    {
        InitializeAtributes();
    }

    public void InitializeAtributes() 
    {
        inputHintName.text = "";
        inputDescriptionHint.text = "";
        inputAnswerHint.text = "";
    }

    async void Update()
    {
        if (canvasConfigHints.enabled)
            await SearchHint();
        else
            currentSizeHints = 0;
    }

    /*async Task SearchBuildingByCollection(){
        BuildingsList = await DataBaseManager.instance.SearchByCollection("Buildings");
        foreach(Dictionary<string,object> Buildings in BuildingsList){
            foreach(KeyValuePair<string,object> pair in Buildings){
                if(pair.Key == "name"){
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = pair.Value.ToString();
                    //BuildingsDropdown.options.Add(option);
                }
            }
        }
    }*/

    void ClearCurrentHints()
    {
        // Vaciar lista y borrar pistas actuales
        foreach(GameObject hint in currentHints)
        {
            Destroy(hint);
        }
        currentHints.Clear();
    }

    async Task SearchHint()
    {

        hintsList = await DataBaseManager.instance.SearchByCollection("Hints");
        newSizeHints = hintsList.Count;

        if (currentSizeHints != newSizeHints)
        {
            ClearCurrentHints();
            foreach(Dictionary<string,object> hint in hintsList)
            {
                foreach(KeyValuePair<string,object> pair in hint)
                {
                    if(pair.Key == "name"){

                        // Instanciar prefab
                        GameObject hintElement = Instantiate (hintPrefab, new Vector3(hintContent.position.x,hintContent.position.y, hintContent.position.z) , Quaternion.identity);
                        hintElement.transform.parent = hintContent.transform;
                        
                        // Editar texto
                        Text hintText = hintPrefab.GetComponentInChildren<Text>();
                        hintText.text = pair.Value.ToString();

                        // Añadir a Lista
                        currentHints.Add(hintElement);
                    }
                }
            }
            currentSizeHints = newSizeHints;
        }       
    }

    public bool CheckNewData()
    {
        if( inputHintName.text != "" && inputDescriptionHint.text != "" && inputAnswerHint.text != "")
        {            
            return true;
        }
        return false;
    }

    public void SaveHint()
    {    
        if (CheckNewData())
        {
            HintsManager.instance.PostNewHint(inputHintName.text, inputDescriptionHint.text, inputAnswerHint.text);
            ScenesManager.instance.LoadNewCanvas(canvasConfigHints);
            ScenesManager.instance.DeleteCurrentCanvas(canvasAddHints);
        }else
            NotificationsManager.instance.SetFailureNotificationMessage("Campos Incompletos. Por favor llene todos los campos.");
    }    

}

