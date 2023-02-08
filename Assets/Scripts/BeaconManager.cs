using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public enum BroadcastMode {
	send	= 0,
	receive	= 1,
	unknown = 2
}
public enum BroadcastState {
	inactive = 0,
	active	 = 1
}

internal class BeaconManager: MonoBehaviour {
	[SerializeField]
	private Text _statusText;

	[SerializeField]
	private GameObject _statusScreen;

	[SerializeField]
	private GameObject _menuScreen;

	[SerializeField]
	private Button _bluetoothButton;

	private Text _bluetoothText;

	/*** Beacon Properties ***/
	
	// Region
	private string s_Region;
	// UUID, Namespace or Url
	private string s_UUID;
	// Major/Minor or Instance
	private string s_Major;

	private string s_Minor;

	/** Input **/
	// beacontype
	[SerializeField]
	private Dropdown InputDropdown;
	private BeaconType bt_PendingType;
	private BeaconType bt_Type;
	// Region
	[SerializeField]
	private InputField input_Region;
	// UUID, Namespace or Url
	[SerializeField]
	private Text txt_inputUUIDChar;
	[SerializeField]
	private InputField input_UUID;
	// Major/Minor or Instance
	[SerializeField]
	private Text txt_inputMajorChar;
	[SerializeField]
	private InputField input_Major;
	[SerializeField]
	private Text txt_inputMinorChar;
	[SerializeField]
	private InputField input_Minor;

	// Beacon BroadcastMode (Send, Receive)
	[SerializeField]
	private Text txt_BroadcastMode_ButtonText;
	[SerializeField]
	private Text txt_BroadcastMode_LabelText;
	private BroadcastMode bm_Mode;

	// Beacon BroadcastState (Start, Stop)
	[SerializeField]
	private Image img_ButtonBroadcastState;
	[SerializeField]
	private Text txt_BroadcastState_ButtonText;
	[SerializeField]
	private Text txt_BroadcastState_LabelText;
	private BroadcastState bs_State;

	// GameObject for found Beacons
	[SerializeField]
	private GameObject go_ScrollViewContent;

	[SerializeField]
	private GameObject go_FoundBeacon;
	List<GameObject> go_FoundBeaconCloneList = new List<GameObject>();
	GameObject go_FoundBeaconClone;
	private float f_ScrollViewContentRectWidth;
	private float f_ScrollViewContentRectHeight;
	private int i_BeaconCounter = 0;

	// 動画生成用
	public SetBeaconNum setBeaconNum;

	// Receive
	private List<Beacon> mybeacons = new List<Beacon>();

    private BeaconProperty[] beaconProperties = new BeaconProperty[2];

    private void Start() {
		setBeaconPropertiesAtStart(); // please keep here!

		_bluetoothButton.onClick.AddListener(delegate() {
			BluetoothState.EnableBluetooth();
		});
		_bluetoothText = _bluetoothButton.GetComponentInChildren<Text>();
		BluetoothState.BluetoothStateChangedEvent += delegate(BluetoothLowEnergyState state) {
			switch (state) {
			case BluetoothLowEnergyState.TURNING_OFF:
			case BluetoothLowEnergyState.TURNING_ON:
				break;
			case BluetoothLowEnergyState.UNKNOWN:
			case BluetoothLowEnergyState.RESETTING:
				SwitchToStatus();
				_statusText.text = "Checking Device…";
				break;
			case BluetoothLowEnergyState.UNAUTHORIZED:
				SwitchToStatus();
				_statusText.text = "You don't have the permission to use beacons.";
				break;
			case BluetoothLowEnergyState.UNSUPPORTED:
				SwitchToStatus();
				_statusText.text = "Your device doesn't support beacons.";
				break;
			case BluetoothLowEnergyState.POWERED_OFF:
				SwitchToMenu();
				_bluetoothButton.interactable = true;
				_bluetoothText.text = "Enable Bluetooth";
				break;
			case BluetoothLowEnergyState.POWERED_ON:
				SwitchToMenu();
				_bluetoothButton.interactable = false;
				_bluetoothText.text = "Bluetooth already enabled";
				break;
			case BluetoothLowEnergyState.IBEACON_ONLY:
				SwitchToMenu();
				_bluetoothButton.interactable = false;
				_bluetoothText.text = "iBeacon only";
				break;
			default:
				SwitchToStatus();
				_statusText.text = "Unknown Error";
				break;
			}
		};
		f_ScrollViewContentRectWidth = ((RectTransform)go_FoundBeacon.transform).rect.width;
		f_ScrollViewContentRectHeight = ((RectTransform)go_FoundBeacon.transform).rect.height;
		BluetoothState.Init();
	}

    	private void SwitchToStatus() {
		_statusScreen.SetActive(true);
		_menuScreen.SetActive(false);
	}

	private void SwitchToMenu() {
		_statusScreen.SetActive(false);
		_menuScreen.SetActive(true);
	}

	private void setBeaconPropertiesAtStart() {
		//RestorePlayerPrefs();

        bm_Mode = BroadcastMode.receive;
        bt_Type = BeaconType.iBeacon;
        if (iBeaconReceiver.regions.Length != 0) {
            Debug.Log("check iBeaconReceiver-inspector");
            for(int i = 0; i < 2; i++)
            {
                beaconProperties[i] = new BeaconProperty();
                beaconProperties[i].sRegion = iBeaconReceiver.regions[i].regionName;
                beaconProperties[i].sUUID = iBeaconReceiver.regions[i].beacon.UUID;
                beaconProperties[i].sMajor = iBeaconReceiver.regions[0].beacon.major.ToString();
                beaconProperties[i].sMinor = iBeaconReceiver.regions[0].beacon.minor.ToString();
            }
            /*
            s_Region	= iBeaconReceiver.regions[0].regionName;
            s_UUID = iBeaconReceiver.regions[0].beacon.UUID;
            s_Major = iBeaconReceiver.regions[0].beacon.major.ToString();
            s_Minor = iBeaconReceiver.regions[0].beacon.minor.ToString();
            */
        }
		
		InputDropdown.value = (int)bt_Type;
		bs_State = BroadcastState.inactive;
		SetBroadcastState();
		Debug.Log("Beacon properties and modes restored");
	}

    private void SetBroadcastState() {
		// setText
		if (bs_State == BroadcastState.inactive)
			txt_BroadcastState_ButtonText.text = "Start";
		else
			txt_BroadcastState_ButtonText.text = "Stop";
		txt_BroadcastState_LabelText.text = bs_State.ToString();
	}

    public void btn_StartStop() {
		//Debug.Log("Button Start / Stop pressed");
		/*** Beacon will start ***/
		if (bs_State == BroadcastState.inactive) {
			// ReceiveMode
            iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
            // check if all mandatory propertis are filled
            if (bt_Type == BeaconType.Any) {
                iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon()),new iBeaconRegion(s_Region, new Beacon())};
            } else {
                if (bt_Type == BeaconType.iBeacon) {
                    for(int i = 0; i < beaconProperties.Length; i++){
                        iBeaconReceiver.regions[i] = new iBeaconRegion(beaconProperties[i].sRegion, new Beacon(beaconProperties[i].sUUID, Convert.ToInt32(beaconProperties[i].sMajor), Convert.ToInt32(beaconProperties[i].sMinor)));    
                    }
                    // iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon(s_UUID, Convert.ToInt32(s_Major), Convert.ToInt32(s_Minor)))};
                }
            }
            // !!! Bluetooth has to be turned on !!! TODO
            iBeaconReceiver.Scan();
            Debug.Log ("Listening for beacons");
			
			bs_State = BroadcastState.active;
			img_ButtonBroadcastState.color = Color.red;
		} else {
            /*** stop ***/
            iBeaconReceiver.Stop();
            iBeaconReceiver.BeaconRangeChangedEvent -= OnBeaconRangeChanged;
            removeFoundBeacons();
			
			bs_State = BroadcastState.inactive;
			img_ButtonBroadcastState.color = Color.green;
		}
		SetBroadcastState();

	}


	private void OnBeaconRangeChanged(Beacon[] beacons) { // 
		foreach (Beacon b in beacons) {
			var index = mybeacons.IndexOf(b);
			if (index == -1) {
				mybeacons.Add(b);
			} else {
				mybeacons[index] = b;
			}
		}
		for (int i = mybeacons.Count - 1; i >= 0; --i) {
			if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now) {
				mybeacons.RemoveAt(i);
			}
		}
		DisplayOnBeaconFound();
	}

    private void DisplayOnBeaconFound() {
		removeFoundBeacons();
		RectTransform rt_Content = (RectTransform)go_ScrollViewContent.transform;
		foreach (Beacon b in mybeacons) {
			// create clone of foundBeacons
			go_FoundBeaconClone = Instantiate(go_FoundBeacon);
			// make it child of the ScrollView
			go_FoundBeaconClone.transform.SetParent(go_ScrollViewContent.transform);
			// get resolution based scalefactor
			float f_scaler = ((RectTransform)go_FoundBeaconClone.transform).localScale.y;
			Vector2 v2_scale = new Vector2(1,1);
			// reset scalefactor
			go_FoundBeaconClone.transform.localScale = v2_scale;
			// get anchorposition
			Vector3 pos = go_ScrollViewContent.transform.position; 
			// positioning
			pos.y -= f_ScrollViewContentRectHeight/f_scaler * i_BeaconCounter;
			go_FoundBeaconClone.transform.position = pos;
			i_BeaconCounter++;
			// resize scrollviewcontent
			rt_Content.sizeDelta = new Vector2(f_ScrollViewContentRectWidth,f_ScrollViewContentRectHeight*i_BeaconCounter);
			go_FoundBeaconClone.SetActive(true);
			// adding reference to instance
			go_FoundBeaconCloneList.Add(go_FoundBeaconClone);
			// get textcomponents
			Text[] Texts	= go_FoundBeaconClone.GetComponentsInChildren<Text>();
			// deleting placeholder
			foreach (Text t in Texts)
				t.text = "";
			Debug.Log ("fond Beacon: " + b.ToString());
			switch (b.type) {
			case BeaconType.iBeacon:
				Texts[0].text 	= "UUID:";
				Texts[1].text 	= b.UUID.ToString();
				Texts[2].text 	= "Major:";
				Texts[3].text	= b.major.ToString();
				Texts[4].text 	= "Minor:";
				Texts[5].text	= b.minor.ToString();
				Texts[6].text 	= "Range:";
				Texts[7].text	= b.range.ToString();
				Texts[8].text 	= "Strength:";
				Texts[9].text	= b.strength.ToString() + " db";
				Texts[10].text 	= "Accuracy:";
				Texts[11].text	= b.accuracy.ToString().Substring(0,10) + " m";
				Texts[12].text 	= "Rssi:";
				Texts[13].text	= b.rssi.ToString() + " db";

				if(b.range.ToString() == "IMMEDIATE"){
					setBeaconNum.SpawnFromBeacon(b.major);
				}

				break;
			default:
				break;
			}
		}
	}

	private void removeFoundBeacons() {
		Debug.Log("removing all found Beacons");
		// set scrollviewcontent to standardsize
		RectTransform rt_Content = (RectTransform)go_ScrollViewContent.transform;
		rt_Content.sizeDelta = new Vector2(f_ScrollViewContentRectWidth,f_ScrollViewContentRectHeight);
		// destroying each clone
		foreach (GameObject go in go_FoundBeaconCloneList)
			Destroy(go);
		go_FoundBeaconCloneList.Clear();
		i_BeaconCounter = 0;
	}




}
