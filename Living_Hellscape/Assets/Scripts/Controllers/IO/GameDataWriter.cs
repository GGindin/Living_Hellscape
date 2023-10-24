using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataWriter
{
    public BinaryWriter Writer { get; set; }

    public void WriteFloat(float value)
    {
        Writer.Write(value);
    }

    public void WriteInt(int value)
    {
        Writer.Write(value);
    }

    //how to read or write at specific pos with binary writer/reader
    //https://stackoverflow.com/questions/6674761/read-binary-file-in-c-sharp-from-specific-position
    public void SeekForward(long byteOffset)
    {
        Writer.BaseStream.Seek(byteOffset, SeekOrigin.Begin);
    }
}
