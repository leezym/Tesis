using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.UIElements;


public class ListNeos : MonoBehaviour
{
    //public ScrollView ScrollWindow;
    public Text nameContent;
    private string idInductor;
    public List<string> NeoJaverianos = new List<string>();

    void Awake()
    {
        
    }

    // Update is called once per frame
    async void Update()
    {
        if (GetComponent<Canvas>().enabled){            
            if (idInductor == null)
                idInductor = AuthManager.instance.GetUserId();

            NeoJaverianos = await DataBaseManager.instance.SearchNeoJaveriansAsync("Rooms", idInductor);
            foreach (string nombre in NeoJaverianos)
            {
                nameContent.text = nombre+"\n";
            }
        }        
    }
}
