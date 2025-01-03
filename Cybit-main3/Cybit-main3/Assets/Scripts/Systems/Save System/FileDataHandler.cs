using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    // game data config
    private string _gameDataDirPath = string.Empty;
    private string _gameSaveFileName = "GeneralSaveData";
    private string _gameSaveDirectoryName = "SaveFiles";

    // profiles data config
    private string _profilesDataDirPath = string.Empty;
    private string _profilesSaveDirectoryName = "Profiles";

    public FileDataHandler(string gameDataDirPath, string gameDataFileName)
    {
        _gameSaveFileName = gameDataFileName;
        _gameDataDirPath = Path.Combine(gameDataDirPath, _gameSaveDirectoryName);
        _profilesDataDirPath = Path.Combine(_gameDataDirPath, _profilesSaveDirectoryName);
    }

    // game data read & write
    public GameData Load()
    {
        string fullPath = Path.Combine(_gameDataDirPath, _gameSaveFileName + ".json");

        GameData loadedData = null;
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

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }
    public void Save(GameData data)
    {
        string fullPath = Path.Combine(_gameDataDirPath, _gameSaveFileName + ".json");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to create save Data" + fullPath + "\n" + e);
        }
    }

    // profiles data read & write
    public Profile LoadProfile(string fileName)
    {
        string fullPath = Path.Combine(_profilesDataDirPath, fileName);
        Profile loadedProfile = null;
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

                loadedProfile = JsonUtility.FromJson<Profile>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedProfile;
    }
    public void SaveProfile(Profile profile, string fileName)
    {
        string fullPath = Path.Combine(_profilesDataDirPath, fileName + ".json");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(profile, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to create save Data" + fullPath + "\n" + e);
        }
    }
}
