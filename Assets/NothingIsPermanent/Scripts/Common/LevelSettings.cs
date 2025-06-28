using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Nothing Is Permanent/LevelSettings")]
public class LevelSettings : ScriptableObject {
    public int LevelTime;
    public int LevelGoal;
    public string StartLevelDescription;
    public string LevelSceneName;
}
