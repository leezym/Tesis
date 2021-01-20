using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance;
    
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
    }

    public void LoadNewCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 1;
    }
}
