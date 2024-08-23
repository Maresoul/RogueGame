using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UITools : MonoBehaviour
{
    [MenuItem("Assets/创建图片", priority = 49)]
    public static void CreateImage()
    {
        UnityEngine.Object[] objs = Selection.objects;
        //等下物体创建完毕后，需要设置到这个根节点下
        var canvas=GameObject.FindObjectOfType<Canvas>();
        Transform tra = canvas.transform; /* GameObject.Find("UIRoot").transform;*/
        if (objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is Texture2D)
                {
                    string path = AssetDatabase.GetAssetPath(objs[i]);//获取路径
                    TextureImporter ti = TextureImporter.GetAtPath(path) as TextureImporter; //获取TextureImporter
                    if (ti.textureType == TextureImporterType.Sprite)
                    {
                        //,typeof(Button)
                        GameObject go = new GameObject(objs[i].name, new Type[] { typeof(Image) });
                        go.transform.SetParent(tra, false);
                        var image = go.GetComponent<Image>();
                        image.type = Image.Type.Simple;
                        UnityEngine.Object newImg = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
                        Undo.RecordObject(image, "Change Image");//有了这句才可以用ctrl+z撤消此赋值操作
                        image.sprite = newImg as Sprite;
                        image.SetNativeSize();
                    }
                }

            }
        }
        AssetDatabase.Refresh();
    }

    //[MenuItem("Assets/创建按钮", priority = 49)]
    public static void CreateImageHaveBtn()
    {
        UnityEngine.Object[] objs = Selection.objects;
        Transform tra = GameObject.Find("UIRoot").transform;
        if (objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is Texture2D)
                {
                    string path = AssetDatabase.GetAssetPath(objs[i]);//获取路径
                    TextureImporter ti = TextureImporter.GetAtPath(path) as TextureImporter; //获取TextureImporter
                    if (ti.textureType == TextureImporterType.Sprite)
                    {
                        GameObject go = new GameObject(objs[i].name, new Type[] { typeof(Image), typeof(Button) });
                        go.transform.SetParent(tra, false);
                        //var image = go.AddComponent<Image>();
                        var image = go.GetComponent<Image>();
                        image.type = Image.Type.Simple;
                        UnityEngine.Object newImg = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
                        Undo.RecordObject(image, "Change Image");
                        image.sprite = newImg as Sprite;
                        image.SetNativeSize();
                    }
                }
            }
        }
        AssetDatabase.Refresh();
    }
}
