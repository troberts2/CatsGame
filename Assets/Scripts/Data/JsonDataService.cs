using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

public class JsonDataService : IDataService
{

    private const string KEY = "kUPoAZKYoMooX373tN0049qXAKUC/z4qW+ZldX7xOkc=";
    private const string IV = "9gIq8hV4bT4cFA5gTZsCeg==";
    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted){
        string path = Application.persistentDataPath + RelativePath;

        try{
            if(File.Exists(path)){
                Debug.Log("Data exsists. Deleting old file and writing a new one!");
                File.Delete(path);
            }else{
                Debug.Log("Writing file for first time!");
            }
            using FileStream stream = File.Create(path);
            if(Encrypted){
                WriteEncryptedData(Data, stream);
            }
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }catch(Exception e){
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }
    private void WriteEncryptedData<T>(T Data, FileStream Stream){
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(
            Stream,
            cryptoTransform,
            CryptoStreamMode.Write
        );
        // Debug.Log($"Initialization Vector: {Convert.ToBase64String(aesProvider.IV)}");
        // Debug.Log($"Key: {Convert.ToBase64String(aesProvider.Key)}");
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(Data)));
    }
    public T LoadData<T>(string RelativePath, bool Encrypted){
        string path = Application.persistentDataPath + RelativePath;

        if(!File.Exists(path)){
            Debug.LogError($"Cannot load file at {path}. File does not exsist");
            throw new FileNotFoundException($"{path} does not exsist");
        }
        try{
            T data;
            if(Encrypted){
                data = ReadEncryptedData<T>(path);
            }else{
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            return data;
        }catch(Exception e){
            Debug.LogError($"Failed to load data due to:  {e.Message} {e.StackTrace}");
            throw e;
        }
    }
    private T ReadEncryptedData<T>(string Path){
        byte[] filesBytes = File.ReadAllBytes(Path);
        using Aes aesProvider = Aes.Create();

        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
        );
        using MemoryStream decryptionStream = new MemoryStream(filesBytes);
        using CryptoStream cryptoStream = new CryptoStream(
            decryptionStream,
            cryptoTransform,
            CryptoStreamMode.Read
        );
        using StreamReader reader = new StreamReader(cryptoStream);
        string result = reader.ReadToEnd();
        Debug.Log($"Decrypted result (if the following is now legible, probably wrong key or iv): {result}");
        return JsonConvert.DeserializeObject<T>(result);
    }
}
