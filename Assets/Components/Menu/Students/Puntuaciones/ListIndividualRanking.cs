using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ListIndividualRanking : MonoBehaviour
{
    public Canvas canvasPuntuaciones, canvasIndividualRanking;
    public GameObject studentRankingPrefab;
    public Transform content;
    public Text textMyRanking, textMyScore;
    List<GameObject> currentStudents = new List<GameObject>();
    List<Dictionary<string, object>> studentsList = new List<Dictionary<string, object>>();
    int count = 1;

    // Update is called once per frame
    void Update()
    {
        if (canvasPuntuaciones.enabled && canvasIndividualRanking.enabled && studentsList.Count == 0)
            SearchPosition();
        else
        {
            count = 1;
            studentsList = new List<Dictionary<string, object>>();
        }
    }

    void ClearCurrentStudents()
    {
        // Vaciar lista y borrar grupos actuales
        foreach(GameObject group in currentStudents)
        {
            Destroy(group);
        }
        currentStudents.Clear();
    }

    public async void SearchPosition()
    {
        studentsList = await UsersManager.instance.GetStudentsByOrderOfScore();
        object myScore = await DataBaseManager.instance.SearchAttribute("Students", AuthManager.instance.GetUserId(), "score");
        object nameStudent = await DataBaseManager.instance.SearchAttribute("Students", AuthManager.instance.GetUserId(), "name");

        textMyScore.text = myScore.ToString();

        ClearCurrentStudents();
        count = 1;

        foreach(Dictionary<string, object> group in studentsList)
        {
            // Instanciar prefab
            GameObject groupElement = Instantiate (studentRankingPrefab, new Vector3(content.position.x,content.position.y, content.position.z) , Quaternion.identity);
            groupElement.transform.parent = content.transform;

            // Asignar puesto
            groupElement.transform.Find("PositionLabel").GetComponent<Text>().text = count.ToString();
            
            foreach(KeyValuePair<string, object> pair in group)
            {
                if(pair.Key == "name")
                {
                    groupElement.transform.Find("NameLabel").GetComponent<Text>().text = pair.Value.ToString();
                    if (pair.Value.ToString() == nameStudent.ToString())
                    {
                        textMyRanking.text = count.ToString();
                    }
                }
                if(pair.Key == "score")
                    groupElement.transform.Find("ScoreLabel").GetComponent<Text>().text = pair.Value.ToString();
            }
            // Añadir a Lista
            currentStudents.Add(groupElement);
            count ++;
        }
    }
}
