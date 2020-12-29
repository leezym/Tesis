using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class HintsManager : MonoBehaviour
{
    public static HintsManager instance;

    public GameObject hintPrefab;
    public Transform hintContent;
    public Canvas canvasConfigHints;
    //public Dropdown BuildingsDropdown;
    List<Dictionary<string,object>> BuildingsList = new List<Dictionary<string,object>>();
    List<Dictionary<string,object>> hintsList = new List<Dictionary<string,object>>();
    public List<GameObject> currentHints = new List<GameObject>();


    int currentSizeHints = 0, newSizeHints = 0;

    void Awake() {
        instance = this;
    }
    
    // Nombre de la DB - Buildings
    async void Start()
    {
        //await SearchBuildingByCollection();
    }

    async void Update()
    {
        if (canvasConfigHints.enabled)
            await SearchHint();
    }

    async Task SearchBuildingByCollection(){
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
    }

    async Task SearchHint()
    {

        hintsList = await DataBaseManager.instance.SearchByCollection("Hints");
        newSizeHints = hintsList.Count;

        if (currentSizeHints != newSizeHints)
        {
            // Vaciar lista y borrar pistas actuales
            foreach(GameObject hint in currentHints)
            {
                Destroy(hint);
            }
            currentHints.Clear();

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
}
