using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    
    [SerializeField] List<LevelSettings> levelSettings;

    public Action OnGameOver;
    public Action OnLevelWin;
    public Action<int> OnLevelProgressUpdated;
    public Action<float> OnTimeUpdated;
    
    private bool _isLevelStarted = false;
    private int _currentLevelIndex;
    private LevelSettings _currentLevelSettings;
    private float _currentLevelTime = 0f;
    private int _currentLevelProgress = 0;
    private List<DestructibleObject> _destructibleObjects = new();
    
    public void SetupLevel(int levelIndex) {
        foreach (var destructibleObject in _destructibleObjects) {
            destructibleObject.OnBeforeDestroy -= HandleObjectDestroyed;
        }
        _destructibleObjects.Clear();
        _currentLevelIndex = levelIndex;
        _currentLevelSettings = levelSettings[levelIndex];
        _currentLevelProgress = 0;
        OnLevelProgressUpdated?.Invoke(_currentLevelProgress);
    }

    public LevelSettings GetCurrentLevelSettings() {
        return _currentLevelSettings;
    }

    public LevelSettings GetNextLevelSettings() {
        if (_currentLevelIndex == levelSettings.Count - 1) {
            return null;
        }
        return levelSettings[_currentLevelIndex + 1];
    }
    
    public void StartCurrentLevel() {
        _currentLevelSettings = levelSettings[_currentLevelIndex];
        _currentLevelTime = _currentLevelSettings.LevelTime;
        _isLevelStarted = true;
    }

    public void IncreaseLevelProgress(int objectValue) {
        _currentLevelProgress += objectValue;
    }

    public void AddDestructibleObject(DestructibleObject destructibleObject) {
        _destructibleObjects.Add(destructibleObject);
        destructibleObject.OnBeforeDestroy += HandleObjectDestroyed;
    }

    private void Update() {
        if (!_isLevelStarted) {
            return;
        }

        if (_currentLevelProgress >= _currentLevelSettings.LevelGoal) {
            _isLevelStarted = false;
            WinLevel();
        }
        
        _currentLevelTime -= Time.deltaTime;
        if (_currentLevelTime <= 0.0f) {
            _currentLevelTime = 0.0f;
            _isLevelStarted = false;
            GameOver();
        }
        OnTimeUpdated?.Invoke(_currentLevelTime);
    }

    private void WinLevel() {
        OnLevelWin?.Invoke();
    }

    private void GameOver() {
        OnGameOver?.Invoke();
    }

    private void HandleObjectDestroyed() {
        _currentLevelProgress += 1;
        OnLevelProgressUpdated?.Invoke(_currentLevelProgress);
    }
}
