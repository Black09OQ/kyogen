using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetBeaconNum : MonoBehaviour
{
    public int beaconNum;
    private TextMeshProUGUI tmpBNum;
    [SerializeField] SpawnManager spawnManager;

    void Start()
    {
        tmpBNum = this.GetComponent<TextMeshProUGUI>();
        beaconNum = 0;
    }

    public void numPlus()
    {
        if(beaconNum >= 0 && beaconNum < 14)
        {
            beaconNum += 1;
        }
        
        tmpBNum.SetText(beaconNum.ToString());
    }

    public void numMinus()
    {
        if(beaconNum > 0 && beaconNum <=14)
        {
            beaconNum -= 1;
        }

        tmpBNum.SetText(beaconNum.ToString());
    }

    public void SpawnWithNum()
    {
        spawnManager.SpawnVideo(beaconNum);
    }
}
