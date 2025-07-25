using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

[System.Serializable]
public class DictionarySerializer<TKey, TValue>
{
    private Dictionary<TKey, TValue> dictionary;

    public DictionarySerializer(Dictionary<TKey, TValue> dictionary)
    {
        this.dictionary = dictionary;
    }

    public void SaveToFile(string filePath, bool debug = false)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fileStream = File.Create(filePath))
        {
            formatter.Serialize(fileStream, dictionary);
            if(debug)
            {
                Debug.Log("Saved Successfully!");
            }
        }
    }

    public static Dictionary<TKey, TValue> LoadFromFile(string filePath, bool debug = false)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (File.Exists(filePath))
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
            {
                if(debug)
                {
                    Debug.Log($"opened {typeof(Dictionary<TKey, TValue>)} file of size {fileStream.Length}");
                }
                return (Dictionary<TKey, TValue>)formatter.Deserialize(fileStream);
            }
        }
        else
        {
            if (debug)
            {
                Debug.LogWarning("File not found. Creating a new dictionary.");
            }
            return new Dictionary<TKey, TValue>();
        }
    }
}
