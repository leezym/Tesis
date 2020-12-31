using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ListTrivias : MonoBehaviour
{
    public Canvas canvasConfigTrivias;
    public Dropdown buildingsDropdown;
    List<Dictionary<string,object>> buildingsList = new List<Dictionary<string,object>>();

    async void Start()
    {
        await SearchBuilding();
    }

    void Update()
    {

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

    public async void SearchTrivias()
    {
        //await 
    }
}
