using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class CardsHints : MonoBehaviour
{
    public Canvas canvasActHint;
    public GameObject hintCardPrefab;
    public Sprite[] backgroundCardHint, backgroundCircleCardHint;
    List<GameObject> currentHints = new List<GameObject>();

    public GameObject hintsScrollbar;
    public Transform contentCards;
    float scrollPos = 0;
    float[] pos;

    void Update()
    {
        if (canvasActHint.enabled)
            CardAnimation();
    }

    void ClearCurrentHints()
    {
        // Vaciar lista y borrar pistas actuales
        foreach(GameObject hint in currentHints)
        {
            Destroy(hint);
        }
        currentHints.Clear();
    }

    public async void  SearchHint()
    {
        List<Dictionary<string,object>> hintsList = await HintsChallengesManager.Instance.GetHintChallengeByRoom(GlobalDataManager.Instance.idRoomByInductor);
        int backgroundCount = 0;

        ClearCurrentHints();
        foreach(Dictionary<string,object> hint in hintsList)
        {
            // Reinicar contador de background
            if (backgroundCount == backgroundCardHint.Length)
                backgroundCount = 0;

            // Instanciar prefab
            GameObject hintElement = Instantiate (hintCardPrefab, new Vector3(contentCards.position.x,contentCards.position.y, contentCards.position.z) , Quaternion.identity);
            hintElement.transform.parent = contentCards.transform;
            
            // Editar background
            hintElement.GetComponent<Image>().sprite = backgroundCardHint[backgroundCount];
            hintElement.transform.Find("PositionCircle").GetComponent<Image>().sprite = backgroundCircleCardHint[backgroundCount];
            backgroundCount ++;

            foreach(KeyValuePair<string,object> pair in hint)
            {
                //Editar text
                if(pair.Key == "name"){                       
                    
                    Text hintNameLabel = hintElement.transform.Find("HintNameLabel").GetComponent<Text>();
                    hintNameLabel.text = pair.Value.ToString();                        
                }
                if (pair.Key == "answer"){
                    Text hintAnswerLabel = hintElement.transform.Find("HintAnswerLabel").GetComponent<Text>();
                    hintAnswerLabel.text = pair.Value.ToString();  
                }
                if(pair.Key == "hour")
                {
                    Text hintTimeLabel = hintElement.transform.Find("HintTimeLabel").GetComponent<Text>();
                    hintTimeLabel.text = pair.Value.ToString();
                    if(pair.Value.ToString() != "")
                    {
                        hintElement.transform.Find("FinishButton").GetComponent<Button>().interactable = false;
                        hintElement.transform.Find("HintScoreInput").GetComponent<InputField>().interactable = false;
                        hintElement.transform.Find("HintPositionNumberInput").GetComponent<InputField>().interactable = false;
                    }       
                    else
                    {
                        hintElement.transform.Find("FinishButton").GetComponent<Button>().interactable = true;
                        hintElement.transform.Find("HintScoreInput").GetComponent<InputField>().interactable = true;
                        hintElement.transform.Find("HintPositionNumberInput").GetComponent<InputField>().interactable = true;
                    }

                }
                if(pair.Key == "score" && Convert.ToInt32(pair.Value) != 0)
                {
                    InputField hintScoreInput = hintElement.transform.Find("HintScoreInput").GetComponent<InputField>();
                    hintScoreInput.text = pair.Value.ToString();
                }
                if(pair.Key == "position" && Convert.ToInt32(pair.Value) != 0)
                {
                    InputField hintPositionNumberInput = hintElement.transform.Find("HintPositionNumberInput").GetComponent<InputField>();
                    hintPositionNumberInput.text = pair.Value.ToString();
                }
            } 

            // Añadir a Lista
            currentHints.Add(hintElement);
        }     
    }

    void CardAnimation() 
    {
        // Animaciones Cards
        pos = new float[contentCards.childCount];
        float distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
        if(Input.GetMouseButton(0))
        {
            scrollPos = hintsScrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for(int i = 0; i < pos.Length; i++)
            {
                if(scrollPos < pos[i] + (distance/2) && scrollPos > pos[i] - (distance/2))
                {
                    hintsScrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(hintsScrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        for(int i = 0; i < pos.Length; i++)
        {
            if(scrollPos < pos[i] + (distance/2) && scrollPos > pos[i] - (distance/2))
            {
                contentCards.GetChild(i).localScale = Vector2.Lerp(contentCards.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for(int j = 0; j < pos.Length; j++)
                {
                    if(j != i)
                    {
                        contentCards.GetChild(j).localScale = Vector2.Lerp(contentCards.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }
    }
}
