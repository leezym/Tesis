using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditGroup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Canvas>().enabled)
        {
            BringRoomData();
        }
    }

    void BringRoomData()
    {

    }
}
