using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int LevelUnlocked;

    public List<float> LevelTimers;

    public SaveData()
    {
        LevelUnlocked = 1;
        LevelTimers = new List<float>();
    }
}
