using System;
using UnityEngine;

[Serializable]
public class MicrobeGlobalParams {
    public float BaseDestructionSpeed = 0.5f;
    public float FinalDissolveTime = 0.8f;
    [Range(0.0f, 1.0f)] public float FinalDissolveThreshold = 0.7f;
}
