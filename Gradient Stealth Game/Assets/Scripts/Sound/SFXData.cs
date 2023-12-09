using UnityEngine;

[System.Serializable]
public class SFXData
{
    public Sound sound;
    public SFXData(Sound sound)
    {
        this.sound = sound;
    }
}