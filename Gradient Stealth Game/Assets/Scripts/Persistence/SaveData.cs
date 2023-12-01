using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public uint LevelUnlocked { get; set; }

    public SaveData()
    {
        LevelUnlocked = 1;
    }
}
