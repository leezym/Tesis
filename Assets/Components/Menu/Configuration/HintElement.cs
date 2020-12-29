using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintElement : MonoBehaviour
{
    public void SelectHint()
    {
        Text hintText = this.gameObject.GetComponentInChildren<Text>();
        ScenesManager.instance.LoadNewCanvas(GameObject.Find("PanelAddHints").GetComponent<Canvas>());
        ScenesManager.instance.DeleteCurrentCanvas(GameObject.Find("PanelConfigHints").GetComponent<Canvas>());
    }
}
