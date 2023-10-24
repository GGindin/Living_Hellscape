using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataReader
{
    public BinaryReader Reader { get; set; }

    public float ReadFloat()
    {
        return Reader.ReadSingle();
    }

    public int ReadInt()
    {
        return Reader.ReadInt32();
    }

    public void SeekForward(long byteOffset)
    {
        Reader.BaseStream.Seek(byteOffset, SeekOrigin.Begin);
    }
}
