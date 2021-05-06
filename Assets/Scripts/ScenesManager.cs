using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;
    public static ScenesManager Instance { get => instance; set => instance = value; }

    private void Awake ()
    {
        instance = this;        
    }

    public void DeleteCurrentCanvas(Canvas canvas)
    {
        canvas.enabled = false;
    }

    public void LoadNewCanvas(Canvas canvas)
    {
        canvas.enabled = true;
    }

    public void DeleteCurrentCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 0;
        canvas.interactable = false;
    }

    public void LoadNewCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 1;
        canvas.interactable = true;
    }
}
