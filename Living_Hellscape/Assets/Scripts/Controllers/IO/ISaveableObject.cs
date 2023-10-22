using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveableObject
{
    public string GetFileName();
    public void SavePerm(GameDataWriter writer);
    public void LoadPerm(GameDataReader reader);
    public void SaveTemp(GameDataWriter writer);
    public void LoadTemp(GameDataReader reader);
}
