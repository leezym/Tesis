using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriviaElement : MonoBehaviour
{
    public void SelectBuilding()
    {
        int valueBuilding = this.GetComponent<Dropdown>().value;
        string buildingName = this.GetComponent<Dropdown>().options[valueBuilding].text;

    }
}
