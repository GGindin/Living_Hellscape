using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameStorageController : MonoBehaviour
{
    public static GameStorageController Instance { get; private set; }

    const string PERM_DATA_DIR = "PERM";
    const string TEMP_DATA_DIR = "TEMP";

    string appDatatPath;
    string permDataPath;
    string tempDataPath;

    GameDataWriter gameDataWriter = new GameDataWriter();
    GameDataReader gameDataReader = new GameDataReader();

    private void Awake()
    {
        Instance = this;

        appDatatPath = Application.persistentDataPath;

        permDataPath = Path.Combine(appDatatPath, PERM_DATA_DIR);
        tempDataPath = Path.Combine(appDatatPath, TEMP_DATA_DIR);

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
    }
}
