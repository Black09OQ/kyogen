using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using TMPro;
 
//ARFoundationを使用する際に追加するネームスペース
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
 
public class SpawnManager : MonoBehaviour
{   
    /// テスト用
    [SerializeField] GameObject redCube;//生成するオブジェクト
    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    ///

    private AssetBundle myLoadedAssetBundle;
    private GameObject prefab;

    public CharactorManager charactorManager;

    public GameObject spawnPoint;
    private Transform spawnTransform;

    private GameObject _videoPlane;

    private VideoPlayer videoPlayer = null;

    public TextMeshProUGUI tmpConsole;

    
    

     
 
    void Start()
    {
        // テスト用         
        arRaycastManager = GetComponent<ARRaycastManager>();


    }
    
    void Update()
    {

        // 動画オブジェクト生成座標と向きの取得
        spawnTransform = spawnPoint.GetComponent<Transform>();
        Vector3 spawnAngles = spawnTransform.eulerAngles;
        spawnAngles.x = 90;
        spawnAngles.y +=180;
        spawnTransform.eulerAngles = spawnAngles;


        /*
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)　　//画面に指が触れた時に処理する
            {

                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;　　//RayとARPlaneが衝突しところのPose
                    Instantiate(redCube, hitPose.position, hitPose.rotation);　　//オブジェクトの生成
                }
            }
        }
        */
        
    }

    public void SpawnVideo(int beaconNum)
    {
        if(videoPlayer != null && videoPlayer.isPlaying == true)
        {
            tmpConsole.SetText("Video is playing!");
            return;
        }

        if(myLoadedAssetBundle != null)
        {
            myLoadedAssetBundle.Unload(true);
        }

        myLoadedAssetBundle = GetComponent<AndroidDirectoryManager>().Score_Load(beaconNum);

        // アセットバンドル内の動画オブジェクト取得
        GameObject prefab = myLoadedAssetBundle.LoadAsset<GameObject>("VideoPlane"+beaconNum.ToString());

        // 動画オブジェクト生成
        _videoPlane = Instantiate(prefab,spawnTransform.position,spawnTransform.rotation) as GameObject;

        if(_videoPlane)
        {
            tmpConsole.SetText("Prefab is Avarable!");
        }
        else
        {
            tmpConsole.SetText("Prefab is Disable!");
        }
        
        videoPlayer = _videoPlane.GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += LoopPointReached;
        
        // 動画再生
        videoPlayer.Play();

        _videoPlane.SetActive(true);

        // 文字セット
        charactorManager.SetCharactor(beaconNum);

        // 動画オブジェクト生成
        // GameObject _videoPlane = Instantiate(prefab,spawnTransform.position,spawnTransform.rotation) as GameObject;
        // VideoPlayer vp = _videoPlane.GetComponent<VideoPlayer>();
        
        // vp.Play();

        // charactorManager.SetCharactor(beaconNum);
        
    }

    public void LoopPointReached(VideoPlayer vp)
    {
        // 動画再生完了時の処理
        Destroy(_videoPlane);

        // メモリ開放
        myLoadedAssetBundle.Unload(true);

    }
 
    
}
