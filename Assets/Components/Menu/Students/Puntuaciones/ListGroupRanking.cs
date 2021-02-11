using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ListGroupRanking : MonoBehaviour
{
    public Canvas canvasPuntuaciones, canvasGroupRanking;
    public GameObject groupRankingPrefab;
    public Transform content;
    public Text textMyGroupRanking, textMyGroupScore;
    List<GameObject> currentGroups = new List<GameObject>();
    List<Dictionary<string, object>> groupsList = new List<Dictionary<string, object>>();
    int count = 1;

    // Update is called once per frame
    void Update()
    {
        if (canvasPuntuaciones.enabled && canvasGroupRanking.enabled && groupsList.Count == 0)
            SearchPosition();
        else
        {
            count = 1;
            groupsList = new List<Dictionary<string, object>>();
        }
    }

    void ClearCurrentGroups()
    {
        // Vaciar lista y borrar grupos actuales
        foreach(GameObject group in currentGroups)
        {
            Destroy(group);
        }
        currentGroups.Clear();
    }

    public async void SearchPosition()
    {
        groupsList = await RoomsManager.Instance.GetRoomsByOrderOfScore();
        object idRoom = await DataBaseManager.Instance.SearchAttribute("Students", AuthManager.Instance.GetUserId(), "idRoom");
        object myGroupScore = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom.ToString(), "score");
        object nameRoom = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom.ToString(),"name");

        textMyGroupScore.text = myGroupScore.ToString();

        ClearCurrentGroups();
        count = 1;

        foreach(Dictionary<string, object> group in groupsList)
        {
            // Instanciar prefab
            GameObject groupElement = Instantiate (groupRankingPrefab, new Vector3(content.position.x,content.position.y, content.position.z) , Quaternion.identity);
            groupElement.transform.parent = content.transform;

            // Asignar puesto
            groupElement.transform.Find("PositionLabel").GetComponent<Text>().text = count.ToString();
            
            foreach(KeyValuePair<string, object> pair in group)
            {
                if(pair.Key == "room")
                {
                    groupElement.transform.Find("NameLabel").GetComponent<Text>().text = pair.Value.ToString();
                    
                    if (pair.Value.ToString() == nameRoom.ToString() && nameRoom != null)
                    {
                        textMyGroupRanking.text = count.ToString();
                    }
                }
                if(pair.Key == "score")
                    groupElement.transform.Find("ScoreLabel").GetComponent<Text>().text = pair.Value.ToString();
            }
            // Añadir a Lista
            currentGroups.Add(groupElement);
            count ++;
        }
    }
}
