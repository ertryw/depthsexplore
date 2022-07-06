using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class CryptedFileSaver
{

    public static bool IsFileExists(string path)
    {
        return File.Exists(path);
    }

    public static void SaveFileAes(string path, object objectToSave)
    {

    }

    public static T LoadFileAes<T>(string path)
    {
        return default(T);
    }

    public static void SaveFileWithBinaryFormater(string path, object objectToSave)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(path);
        binaryFormatter.Serialize(file, objectToSave);
        file.Close();
    }

    public static T LoadFileWithBinaryFormater<T>(string path) where T : class
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            T data = binaryFormatter.Deserialize(file) as T;
            file.Close();
            return data;
        }
        catch
        {
            return default(T);
        }
    }



}


