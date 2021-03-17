using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class CardsTrivias : MonoBehaviour
{
    [Header ("INDUCTOR")]
    public Canvas canvasActTrivia;
    public GameObject triviaCardPrefab;
    public Sprite backgroundCardLago, backgroundCardPalmas,backgroundCardGuayacanes, backgroundCardRaulPosada;
    List<Dictionary<string,object>> triviasList = new List<Dictionary<string,object>>();
    List<GameObject> currentTrivias = new List<GameObject>();    

    public GameObject triviasScrollbar;
    public Transform contentCards;
    float scrollPos = 0;
    float[] pos;

    void Update()
    {
        if (canvasActTrivia.enabled)
            CardAnimation();
    }

    void ClearCurrentTrivias()
    {
        // Vaciar lista y borrar pistas actuales
        foreach(GameObject trivia in currentTrivias)
        {
            Destroy(trivia);
        }
        currentTrivias.Clear();
    }

    public async void SearchTrivia()
    {
        triviasList = await TriviasChallengesManager.Instance.GetTriviaChallengeByBuilding(GlobalDataManager.Instance.idUserInductor);
        ClearCurrentTrivias();
        foreach(Dictionary<string,object> trivia in triviasList)
        {
            // Instanciar prefab
            GameObject triviaElement = Instantiate (triviaCardPrefab, new Vector3(contentCards.position.x,contentCards.position.y, contentCards.position.z) , Quaternion.identity);
            triviaElement.transform.parent = contentCards.transform; 

            foreach(KeyValuePair<string,object> pair in trivia)
            {
                //Editar text
                if(pair.Key == "name"){                       
                    
                    Text triviaNameLabel = triviaElement.transform.Find("BuildingNameLabel").GetComponent<Text>();
                    triviaNameLabel.text = pair.Value.ToString();   

                    if (pair.Value.ToString() == "El Lago")
                        triviaElement.GetComponent<Image>().sprite = backgroundCardLago;
                    else if (pair.Value.ToString() == "Los Guayacanes")
                        triviaElement.GetComponent<Image>().sprite = backgroundCardGuayacanes;
                    else if (pair.Value.ToString() == "Las Palmas")
                        triviaElement.GetComponent<Image>().sprite = backgroundCardPalmas;
                    else if (pair.Value.ToString() == "Raúl Posada S.J.")
                        triviaElement.GetComponent<Image>().sprite = backgroundCardRaulPosada;
                }
                if(pair.Key == "amongQuestions")
                {
                    Text triviaAmongQuestionsLabel = triviaElement.transform.Find("AmountQuestionsLabel").GetComponent<Text>();
                    triviaAmongQuestionsLabel.text = pair.Value.ToString();
                }
                if(pair.Key == "available")
                {
                    if (Convert.ToBoolean(pair.Value))
                        triviaElement.transform.Find("InitializeTriviaButton").GetComponent<Button>().interactable = true;
                    else
                        triviaElement.transform.Find("InitializeTriviaButton").GetComponent<Button>().interactable = false;
                }
            }               

            // Añadir a Lista
            currentTrivias.Add(triviaElement);
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
            scrollPos = triviasScrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for(int i = 0; i < pos.Length; i++)
            {
                if(scrollPos < pos[i] + (distance/2) && scrollPos > pos[i] - (distance/2))
                {
                    triviasScrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(triviasScrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
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
