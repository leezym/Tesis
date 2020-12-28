using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class HintsManager : MonoBehaviour
{
    //public static HintsManager instance;
    public Dropdown BuildingsDropdown;
    public Canvas canvasHints;
    List<Dictionary<string,object>> BuildingsList = new List<Dictionary<string,object>>();

    // Nombre de la DB - Buildings
    async void Start()
    {
        await SearchBuildingByCollection();
    }

    void Update()
    {
    }

    async Task SearchBuildingByCollection(){
        BuildingsList = await DataBaseManager.instance.SearchByCollection("Buildings");
        foreach(Dictionary<string,object> Buildings in BuildingsList){
            foreach(KeyValuePair<string,object> pair in Buildings){
                if(pair.Key == "name"){
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = pair.Value.ToString();
                    BuildingsDropdown.options.Add(option);
                }
            }
        }
    }

}
