﻿using System.Collections;
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
            //SearchRanking();
            SearchScore();
        }

    }

    void SearchRanking()
    {
        // Pendiente
    }

    async void SearchScore()
    {
        string idInductor = await UsersManager.instance.GetInductorIdByAuth(AuthManager.instance.GetUserId());
        string idRoom = await RoomsManager.instance.GetRoomByInductor(idInductor);
        List<Dictionary<string, object>> hintsChallengesList = await HintsChallengesManager.instance.GetHintChallengeByRoom(idRoom);
        int score = 0;
        foreach(Dictionary<string, object> hintChallenge in hintsChallengesList)
        {
            foreach(KeyValuePair<string, object> pair in hintChallenge)
            {
                if(pair.Key == "score")
                    score += Convert.ToInt32(pair.Value);
            }
        }

        pointsGroupText.text = score.ToString();
    }
}