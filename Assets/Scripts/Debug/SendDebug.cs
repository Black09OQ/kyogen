using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SendDebug : MonoBehaviour
{

    private TextMeshProUGUI DebugText;

    void Awake()
    {
        Application.logMessageReceived += LoggedCb;  // ログ出力時のコールバックを登録
    }

    void Start()
    {
        DebugText = this.GetComponent<TextMeshProUGUI>();
    }

    public void LoggedCb(string logstr, string stacktrace, LogType type)
    {
        DebugText.GetComponent<TextMeshProUGUI>().SetText(logstr);
    }
}
