using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using UnityEngine.Events;

public class FirebaseInit : MonoBehaviour
{
    public UnityEvent OnFirebaseInitialized = new UnityEvent();

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            if (task.Exception != null) {
            Debug.LogError($"Failed to initialize Firebase with {task.Exception}");
            }
            OnFirebaseInitialized.Invoke();
        });
    }

    void Update()
    {
        
    }
}
