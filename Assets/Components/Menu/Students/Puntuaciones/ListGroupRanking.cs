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
    int count = 1;

    public void Refresh()
    {
        SearchPosition();
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

    async void SearchPosition()
    {
        List<Dictionary<string, object>> groupsList = await RoomsManager.Instance.GetRoomsByOrderOfScore();
        object idRoom = await DataBaseManager.Instance.SearchAttribute("Students", AuthManager.Instance.GetUserId(), "idRoom");
        object myGroupScore = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom.ToString(), "score");
        object nameRoom = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom.ToString(), "room");

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
                    
                    if (pair.Value.ToString() == nameRoom.ToString())
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
