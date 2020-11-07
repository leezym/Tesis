using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.UIElements;


public class ListNeos : MonoBehaviour
{
    public Text nameLabel;
    //public ScrollView ScrollWindow;
    public GameObject scrollMilagroso;
    private string idInductor;
    public List<string> NeoJaverianos = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        idInductor = AuthManager.instance.GetUserId();
    }

    // Update is called once per frame
    async void Update()
    {
        NeoJaverianos = await DataBaseManager.instance.SearchNeoJaveriansAsync("Rooms", idInductor);
        foreach (string nombre in NeoJaverianos)
        {
            scrollMilagroso.GetComponent<Text>().text = nombre+"\n";
        }
        
    }
}
