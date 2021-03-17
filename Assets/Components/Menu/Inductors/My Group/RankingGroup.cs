using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

public class RankingGroup : MonoBehaviour
{
    public Canvas canvasMyGroup;
    public Text pointsGroupText;

    void Update()
    {
        if (canvasMyGroup.enabled)
        {
            SearchScore();
        }
    }

    async void SearchScore()
    {
        string idRoom = GlobalDataManager.Instance.idRoomByInductor;
        object score = await DataBaseManager.Instance.SearchAttribute("Rooms", idRoom, "score");
        pointsGroupText.text = score.ToString();
    }
}
