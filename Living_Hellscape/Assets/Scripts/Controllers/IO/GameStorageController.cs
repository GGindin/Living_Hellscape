using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameStorageController : MonoBehaviour
{
    [SerializeField]
    bool debugSaveLocation = true;

    public static GameStorageController Instance { get; private set; }

    const string SAVE_1 = "SAVE_1";
    const string SAVE_2 = "SAVE_2";
    const string SAVE_3 = "SAVE_3";

    const string PERM_DATA_DIR = "PERM";
    const string TEMP_DATA_DIR = "TEMP";

    string appDatatPath;
    string permDataPath;
    string tempDataPath;
    string savePath;

    public int saveFileInt {  get; private set; }

    GameDataWriter gameDataWriter = new GameDataWriter();
    GameDataReader gameDataReader = new GameDataReader();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        appDatatPath = Application.persistentDataPath;

        if (debugSaveLocation)
        {
            Debug.Log("Game Data Is Saved to: " + appDatatPath);
        }
    }

    public void Initialize(int saveFile)
    {
        savePath = Path.Combine(appDatatPath, GetSaveString(saveFile));

        permDataPath = Path.Combine(savePath, PERM_DATA_DIR);
        tempDataPath = Path.Combine(savePath, TEMP_DATA_DIR);


        Directory.CreateDirectory(savePath);

        if (Directory.Exists(tempDataPath))
        {
            Directory.Delete(tempDataPath, true);
        }

        Directory.CreateDirectory(permDataPath);
        Directory.CreateDirectory(tempDataPath);
    }

    public void SaveTemp(ISaveableObject saveable)
    {
        var file = saveable.GetFileName();
        var path = Path.Combine(tempDataPath, file);

        using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            gameDataWriter.Writer = writer;
            saveable.SaveTemp(gameDataWriter);
        }
    }

    public void SavePerm(ISaveableObject saveable)
    {
        var file = saveable.GetFileName();
        var path = Path.Combine(permDataPath, file);

        using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            gameDataWriter.Writer = writer;
            saveable.SavePerm(gameDataWriter);
        }
    }

    public void LoadTemp(ISaveableObject saveable)
    {
        var file = saveable.GetFileName();
        var path = Path.Combine(tempDataPath, file);

        if(File.Exists(path))
        {
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                gameDataReader.Reader = reader;
                saveable.LoadTemp(gameDataReader);
            }
        }
        else
        {
            saveable.LoadTemp(null);
        }
    }

    public void LoadPerm(ISaveableObject saveable)
    {
        var file = saveable.GetFileName();
        var path = Path.Combine(permDataPath, file);

        if (File.Exists(path))
        {
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                gameDataReader.Reader = reader;
                saveable.LoadPerm(gameDataReader);
            }
        }
        else
        {
            saveable.LoadPerm(null);
        }
    }

    public void DeleteAllData()
    {
        appDatatPath = Application.persistentDataPath;

        savePath = Path.Combine(appDatatPath, GetSaveString(0));
        if (Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true);
        }
        savePath = Path.Combine(appDatatPath, GetSaveString(1));
        if (Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true);
        }
        savePath = Path.Combine(appDatatPath, GetSaveString(2));
        if (Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true);
        }

        Debug.Log("ALL SAVE DATA DELETED AT: " + appDatatPath);
    }

    public void DeleteSaveFile(int saveFile)
    {
        appDatatPath = Application.persistentDataPath;

        savePath = Path.Combine(appDatatPath, GetSaveString(saveFile));
        if (Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true);
        }

        Debug.Log("SAVE FILE " + (saveFile + 1) + " DELETED AT: " + savePath);
    }

    public bool DoesSaveExist(int saveFile)
    {
        appDatatPath = Application.persistentDataPath;

        savePath = Path.Combine(appDatatPath, GetSaveString(saveFile));
        if (Directory.Exists(savePath))
        {
            return true;
        }

        return false;
    }

    private string GetSaveString(int saveFile)
    {
        switch (saveFile)
        {
            case 0:
                saveFileInt = 0;
                return SAVE_1;
            case 1:
                saveFileInt = 1;
                return SAVE_2;
            case 2:
                saveFileInt = 2;
                return SAVE_3;
            default:
                saveFileInt = 0;
                return SAVE_1;
        }
    }
}
