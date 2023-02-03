using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharactorManager : MonoBehaviour
{
    public GameObject[] charas;

    void Start()
    {
        for(int i = 0; i < 14; i++)
        {
            charas[i].SetActive(false);
        }
    }

    public void SetCharactor(int charaNum)
    {
        charas[charaNum].SetActive(true);
    }

}
