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
    int count = 1;

    public void Refresh()
    {
        SearchPosition();
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

    async void SearchPosition()
    {
        List<Dictionary<string, object>> studentsList = await UsersManager.Instance.GetStudentsByOrderOfScore();
        object myScore = await DataBaseManager.Instance.SearchAttribute("Students", AuthManager.Instance.GetUserId(), "score");
        object nameStudent = await DataBaseManager.Instance.SearchAttribute("Students", AuthManager.Instance.GetUserId(), "name");

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
