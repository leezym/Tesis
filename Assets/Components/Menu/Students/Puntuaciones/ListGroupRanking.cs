using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ListGroupRanking : MonoBehaviour
{
    public GameObject canvasGroupRanking;
    public GameObject groupRankingPrefab;
    public Transform content;
    int count = 1;

    // Update is called once per frame
    void Update()
    {
        if (canvasGroupRanking.activeSelf)
            SearchPosition();
        else
            count = 1;
    }

    async void SearchPosition()
    {
        List<Dictionary<string, object>> groupsList = await RoomsManager.instance.GetRoomsByOrderOfScore();
        foreach(Dictionary<string, object> group in groupsList)
        {
            // Instanciar prefab
            GameObject groupElement = Instantiate (groupRankingPrefab, new Vector3(content.position.x,content.position.y, content.position.z) , Quaternion.identity);
            groupElement.transform.parent = content.transform;

            // Asignar puesto
            groupElement.transform.Find("").GetComponent<Text>().text = count.ToString();
            count ++;
            
            foreach(KeyValuePair<string, object> pair in group)
            {
                if(pair.Key == "name")
                    groupElement.transform.Find("").GetComponent<Text>().text = pair.Value.ToString();
                if(pair.Key == "score")
                    groupElement.transform.Find("").GetComponent<Text>().text = pair.Value.ToString();
            }
        }
    }
}
