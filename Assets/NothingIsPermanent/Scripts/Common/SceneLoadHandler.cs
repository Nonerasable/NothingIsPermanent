using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour {
    public event Action<string> OnSceneLoaded;
    
    void Awake() {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) {
        OnSceneLoaded?.Invoke(scene.name);
    }
}