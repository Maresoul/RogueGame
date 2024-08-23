using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System;

public class PathHelper 
{
    [MenuItem("GameObject/CopyPath")]
    public static void GetSelectGameObjectPath() {
        if (Selection.activeGameObject!=null)
        {
            List<string> str = new List<string>();
            GetParentName(Selection.activeGameObject.transform,ref str);
            str.Reverse();
            string path = "";

            for (int i = 1; i < str.Count; i++)
            {
                path += str[i] + "/";
            }
            path += Selection.activeGameObject.name;

            TextEditor t = new TextEditor();
            t.text = $"\"{path}\"";// path;
            t.OnFocus();
            t.Copy();
            Debug.Log(path);
        }
    }

    public static void GetParentName(Transform p,ref List<string> str) {
        if (p.transform.parent != null)
        {
            str.Add(p.transform.parent.name);
            GetParentName(p.transform.parent,ref str);
        }
    }

    [MenuItem("Tools/打开沙盒目录")]
    public static void OpenPersistentDataPath() {
        if (Directory.Exists(Application.persistentDataPath))
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }



    [MenuItem("Tools/获取SDK MD5")]
    public static void GetFileHash()
    {
        try
        {
            var filePath = @"F:/zyzpro 2/client/res_ZY/Assets/com.rlabrecque.steamworks.net/Plugins/steam_api64.dll";
            //Debug.LogError(GetFileHash());
            FileStream filestream = File.OpenRead(filePath);//  new FileStream(filePath, FileMode.Open);
            int length = (int)filestream.Length;
            byte[] data = new byte[length];
            filestream.Read(data, 0, length);
            filestream.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMd5 = "";
            foreach (byte item in result)
            {
                fileMd5 += Convert.ToString(item, 16);
            }
            Debug.LogError(fileMd5);
            //return fileMd5;
        }
        catch (FileNotFoundException)
        {

            //return "";
        }
    }


}
