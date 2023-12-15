using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
    }

    public SaveData Load()
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        SaveData saveData = null;

        if (File.Exists(fullPath))
        {
            try 
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                saveData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error, cannot load from file: " + fullPath + "\n" + e);
            }
        }

        return saveData;
    }

    public void Save(SaveData saveData)
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        try 
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(saveData, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error, cannot save to file: " + fullPath + "\n" + e);
        }
    }
}
