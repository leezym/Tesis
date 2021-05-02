using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Threading.Tasks;
using System;

public class ListStudentHints : MonoBehaviour
{
    public GameObject hintElementPrefab;
    public Transform contentList;
    public List<InfoHint> currentHints = new List<InfoHint>();    
    List<Dictionary<string,object>> hintsList = new List<Dictionary<string, object>>();

    int index = 0;

    void ClearCurrentHints()
    {
        // Vaciar lista y borrar pistas actuales
        foreach(InfoHint hint in currentHints)
        {
            Destroy(hint.hint);
        }
        currentHints.Clear();
    }
    
    public async void SearchHint()
    {
        hintsList = await HintsChallengesManager.Instance.GetHintChallengeByRoom(GlobalDataManager.Instance.idRoomByStudent);
            
        ClearCurrentHints();
        foreach(Dictionary<string,object> hint in hintsList)
        {
            string hintDescription = "", hintName  = "";

            // Instanciar prefab
            GameObject hintElement = Instantiate (hintElementPrefab, new Vector3(contentList.position.x,contentList.position.y, contentList.position.z) , Quaternion.identity);
            hintElement.transform.parent = contentList.transform;

            foreach(KeyValuePair<string,object> pair in hint)
            {
                //Editar text
                if(pair.Key == "name")
                {
                    Text hintNameLabel = hintElement.transform.Find("HintNameLabel").GetComponent<Text>();
                    hintNameLabel.text = pair.Value.ToString();
                    hintName = pair.Value.ToString();
                }
                if (pair.Key == "description")
                {
                    hintDescription = pair.Value.ToString();
                }
                if(pair.Key == "score")
                {
                    if(Convert.ToInt32(pair.Value) > 0)
                    {
                        hintElement.transform.Find("Toggle").GetComponent<Toggle>().isOn = true;
                    }       
                    else
                    {
                        hintElement.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
                    }
                }
            }

            // Añadir a Lista
            currentHints.Add(new InfoHint(hintElement, hintName, hintDescription));
        }
    }
}

[System.Serializable]
public class InfoHint
{
    public GameObject hint;
    public string name;
    public string description;

    public InfoHint(GameObject hint, string name, string description)
    {
        this.hint = hint;
        this.name = name;
        this.description = description;
    }
}
