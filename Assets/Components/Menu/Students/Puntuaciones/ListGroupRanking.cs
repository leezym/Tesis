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
    string nameRoom;

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
        Dictionary<string, object> data = await RoomsManager.Instance.GetRoomAsync(GlobalDataManager.Instance.idRoomByStudent);

        foreach(KeyValuePair<string, object> pair in data){
            if(pair.Key == "score")
                textMyGroupScore.text = pair.Value.ToString();
            if(pair.Key == "room")
                nameRoom = pair.Value.ToString();
        }

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
                    
                    if (pair.Value.ToString() == nameRoom)
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
