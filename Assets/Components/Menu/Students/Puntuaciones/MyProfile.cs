using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyProfile : MonoBehaviour
{
    public Canvas canvasPuntuaciones, canvasMyProfile;
    public Text textMyGroupRanking, textMyGroupScore, textMyRanking, textMyScore;

    void Update()
    {
        if (canvasPuntuaciones.enabled && canvasMyProfile.enabled)
        {
            SearchMyData();
            SearchGroupData();
        }
    }

    async void SearchGroupData()
    {
        object idRoom = await DataBaseManager.instance.SearchAttribute("Students", AuthManager.instance.GetUserId(), "idRoom");
        object myGroupScore = await DataBaseManager.instance.SearchAttribute("Rooms", idRoom.ToString(), "score");

        textMyGroupRanking.text = "";

        if (myGroupScore != null)
            textMyGroupScore.text = myGroupScore.ToString();
        else
            textMyGroupScore.text = "0";
    }

    async void SearchMyData()
    {
        object myScore = await DataBaseManager.instance.SearchAttribute("Students", AuthManager.instance.GetUserId(), "score");

        textMyRanking.text = "";
        
        if (myScore != null)
            textMyScore.text = myScore.ToString();
        else
            textMyScore.text = "0";
    }
}
