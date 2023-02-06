using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class AndroidDirectoryManager : MonoBehaviour
{
    public string Directory_path;                   // 読み込むアセットのfile以下のパス

    [SerializeField] TextMeshProUGUI tmpConsole;    // コンソール用テキストオブジェクト


    public string date;                             // テスト用
    
    public CharactorManager charactorManager;       // 文字表示用クラス
    
    void Update()
    {

    }

    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        // ディレクトリ確保
        if(Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }

    public void Score_Save()
    {
        //データの保存
        SafeCreateDirectory(Path.Combine(Application.persistentDataPath, Directory_path));
        string json = JsonUtility.ToJson(date);
        var Writer = new StreamWriter(Path.Combine(Application.persistentDataPath, Directory_path, "date.json"));
        Writer.Write(json);
        Writer.Flush();
        Writer.Close();
    }


    public AssetBundle Score_Load(int assetNum)
    {
        // アセットバンドル読み込み
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + Directory_path + "/" + "videoplane"+ assetNum.ToString());
        if (myLoadedAssetBundle == null)
        {
            tmpConsole.SetText("Failed to load AssetBundle!");
            Debug.Log("Failed to load AssetBundle!");
            return null;
        }

        return myLoadedAssetBundle;




        /*
        //データの取得
        var reader = new StreamReader(Path.Combine(Application.persistentDataPath,Directory_path ,"date.json"));
        string json = reader.ReadToEnd();
        reader.Close();
        tmpConsole.SetText(json);
        Debug.Log(json);
        */
    }

}
