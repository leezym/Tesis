using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Threading.Tasks;

public class ListHints : MonoBehaviour
{
    public GameObject hintPrefab;
    public Transform hintContent;
    public Canvas canvasConfigHints, canvasAddHints;
    List<DocumentSnapshot> hintsList = new List<DocumentSnapshot>();
    List<GameObject> currentHints = new List<GameObject>();
    int currentSizeHints = 0, newSizeHints = 0;

    public InputField inputHintName, inputHintDescription, inputHintAnswer;
    
    // Nombre de la DB - Buildings
    void Start()
    {
        InitializeAtributes();
    }

    public void InitializeAtributes() 
    {
        inputHintName.text = "";
        inputHintDescription.text = "";
        inputHintAnswer.text = "";
    }

    async void Update()
    {
        if (canvasConfigHints.enabled)
            await SearchHint();
        else
            currentSizeHints = 0;
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

    async Task SearchHint()
    {

        hintsList = await DataBaseManager.instance.SearchByCollection("Hints");
        newSizeHints = hintsList.Count;

        if (currentSizeHints != newSizeHints)
        {
            ClearCurrentHints();
            foreach(DocumentSnapshot hint in hintsList)
            {
                foreach(KeyValuePair<string,object> pair in hint.ToDictionary())
                {
                    if(pair.Key == "name"){

                        // Instanciar prefab
                        GameObject hintElement = Instantiate (hintPrefab, new Vector3(hintContent.position.x,hintContent.position.y, hintContent.position.z) , Quaternion.identity);
                        hintElement.transform.parent = hintContent.transform;
                        
                        // Editar texto
                        Text hintText = hintElement.GetComponentInChildren<Text>();
                        hintText.text = pair.Value.ToString();

                        // Añadir a Lista
                        currentHints.Add(hintElement);
                    }
                }
            }
            currentSizeHints = newSizeHints;
        }       
    }

    public bool CheckNewData()
    {
        if( inputHintName.text != "" && inputHintDescription.text != "" && inputHintAnswer.text != "")
        {            
            return true;
        }
        return false;
    }

    public async void SaveHint()
    {    
        if (CheckNewData())
        {
            List<Dictionary<string, object>> newHint = await HintsManager.instance.GetHintByName(inputHintName.text);
            if (newHint.Count == 0)
            {
                await HintsManager.instance.PostNewHint(inputHintName.text, inputHintDescription.text, inputHintAnswer.text);
                ScenesManager.instance.LoadNewCanvas(canvasConfigHints);
                ScenesManager.instance.DeleteCurrentCanvas(canvasAddHints);
                ClearCurrentHints();
                NotificationsManager.instance.SetSuccessNotificationMessage("Pista creada.");
            }
            else
                NotificationsManager.instance.SetFailureNotificationMessage("Ya existe una pista con ese nombre. Por favor intenta con otro.");
        }else
            NotificationsManager.instance.SetFailureNotificationMessage("Campos Incompletos. Por favor llene todos los campos.");
    }
}

