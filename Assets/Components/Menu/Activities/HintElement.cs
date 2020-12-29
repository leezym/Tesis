using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintElement : MonoBehaviour
{
    public void SelectHint()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
    }
}
