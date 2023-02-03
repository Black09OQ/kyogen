using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
 
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


    public CharactorManager charactorManager;

    private Transform spawnTransform;
    public GameObject spawnPoint;

    
    

     
 
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
        GameObject prefab = GetComponent<AndroidDirectoryManager>().Score_Load(beaconNum.ToString());

        // 動画オブジェクト生成
        GameObject _videoPlane = Instantiate(prefab,spawnTransform.position,spawnTransform.rotation) as GameObject;
        VideoPlayer vp = _videoPlane.GetComponent<VideoPlayer>();
        
        vp.Play();

        charactorManager.SetCharactor(beaconNum);
        
    }
 
    
}
