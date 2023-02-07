using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Move : MonoBehaviour
{
    static public Move instance;
    GameObject loader;
    GameObject now;

    //バージョン
    int version = 2;

    //AssetBundle URL
    string bundleURL;

    //対象のバンドル名
    string bundleName;

    //キャッシュ
    AssetBundle chach;

    //キャッシュからバンドル名で呼び出すためのdictionary
    Dictionary<string, GameObject> bundleList = new Dictionary<string, GameObject> ();

    // スポーンする場所
    public GameObject spawnPoint;
    private Vector3 spawnPosition;
    private Vector3 spawnAngles;

    // スポーンしたGameObjectのVideoPlayer
    private VideoPlayer nowVP;

    // 文字収集管理クラス
    public CharactorManager charactorManager;
    private int bNum;



    void Awake ()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad (this.gameObject);
        } else {
            Destroy (this.gameObject);
        }

    }

    void Update ()
    {
        spawnPosition = spawnPoint.GetComponent<Transform>().position;
        spawnAngles = spawnPoint.GetComponent<Transform>().eulerAngles;
        spawnAngles.x = 90;
        spawnAngles.y +=180;
        
    }

    public void OnMove (int bundleNum)
    {
        bundleName = "videoplane" + bundleNum.ToString();
        bNum = bundleNum;

        if (!bundleList.ContainsKey (bundleName)) {
            //URL設定
            bundleURL = "file://" + Application.persistentDataPath + "/AssetBundles/" + bundleName;


            
            if (PlayerPrefs.GetInt ("bundleVersion", -1) != version) {
                Caching.ClearCache ();
                PlayerPrefs.SetInt ("bundleVersion", version);
            }
            

            StartCoroutine ("Import");
        } else {
            OnComplete ();
        }
    }

    IEnumerator Import ()
    {
        Invoke ("SetLoader", 1f);

        //キャッシュの読み込み準備待ち
        while (!Caching.ready) {
            yield return null;
        }
        //ダウンロード
        WWW www = WWW.LoadFromCacheOrDownload (bundleURL, version);
        while (!www.isDone) {
            yield return null;
        }
        //アセットバンドルをキャッシュ
        chach = www.assetBundle;

        //dictionaryに保管
        string[] names = chach.GetAllAssetNames ();
        bundleList.Add (bundleName, chach.LoadAsset<GameObject> (names [0]));

        //リクエストの解放
        www.Dispose ();

        OnComplete ();

    }

    void OnComplete ()
    {
        
        GameObject prefab = bundleList [bundleName];

        if (prefab != null) {
            if (now != null) {
                if(nowVP != null && nowVP.isPlaying == true)
                {
                    Debug.Log("Video is playing!");
                    DeleteLoader ();
                    return;
                }
                Destroy(now);
            }
            now = Instantiate (prefab);
            
            if(now)
            {
                Debug.Log("Video " + bNum.ToString() + " Spawned!");
            }
            

            now.GetComponent<Transform>().position = spawnPosition;
            now.GetComponent<Transform>().eulerAngles = spawnAngles;

            
            nowVP = now.GetComponent<VideoPlayer>();

            
            nowVP.loopPointReached += LoopPointReached;
            
            charactorManager.SetCharactor(bNum);

            DeleteLoader ();

            //保存
            PlayerPrefs.SetString ("presentScene", bundleName);

        }
    }

    void SetLoader ()
    {
        GameObject prefab = Resources.Load<GameObject> ("loader");
        loader = Instantiate (prefab);
        /*
        GameObject stage = GameObject.Find ("stage");
        loader.transform.SetParent (stage.transform);
        loader.transform.localPosition = Vector3.zero;
        loader.transform.localScale = Vector3.one;
        */
    }

    void DeleteLoader ()
    {
        CancelInvoke ();
        Destroy (loader);
    }

    
    public void LoopPointReached(VideoPlayer vp)
    {
        // 動画再生完了時の処理
        Destroy(now);

        nowVP = null;

    }
    
}

