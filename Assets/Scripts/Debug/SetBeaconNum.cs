using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetBeaconNum : MonoBehaviour
{
    private int debugNum;
    public TextMeshProUGUI tmpBNum;
    public CharactorManager charactorManager;
    public Move move;

    void Start()
    {
        debugNum = 0;
    }

    public void numPlus()
    {
        if(debugNum >= 0 && debugNum < 13)
        {
            debugNum += 1;
        }
        
        tmpBNum.SetText(debugNum.ToString());
    }

    public void numMinus()
    {
        if(debugNum > 0 && debugNum <=13)
        {
            debugNum -= 1;
        }

        tmpBNum.SetText(debugNum.ToString());
    }

    public void SpawnWithNum()
    {
        move.OnMove(debugNum);
    }

    public void SpawnFromBeacon(int beaconNum)
    {
        move.OnMove(beaconNum);
    }
}
