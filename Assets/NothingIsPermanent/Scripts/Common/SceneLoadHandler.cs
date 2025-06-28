using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour {

    [SerializeField] private List<string> levelSceneNames;
    
    void Awake() {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        for (int i = 0; i < levelSceneNames.Count; i++) {
            if (scene.name == levelSceneNames[i]) {
                DIContainer.Inst.ShowLevelInfo(i);
                return;
            }
        }
        DIContainer.Inst.LoadTestPreset();
    }
}
