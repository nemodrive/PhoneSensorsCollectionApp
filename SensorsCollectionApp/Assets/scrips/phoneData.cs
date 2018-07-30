using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using SimpleJSON;
using WebSocketSharp;


[Serializable]
public class SensorData
{
    public Vector3 accelerometer;
    public Vector2 gps;
    public Vector3 gyro_rot;
    public Quaternion gyro_att;
    public Vector2 magnetometer;
    public Vector3 magentometer_raw;
    public String time_stamp;
}


public class phoneData : MonoBehaviour {

    public int i;

    [SerializeField]
    public Text acc_info;

    [SerializeField]
    public Text gyro_info;

    [SerializeField]
    public Text gps_info;

    [SerializeField]
    public Text aux_info;

    [SerializeField]
    public Text compass_info;


    public Button startButton;
    public Button stopButton;
    public bool pressed = false;

    public Gyroscope m_Gyro;
    public Compass m_Comp;
    public WebSocket ws = null;

    // Use this for initialization
    IEnumerator Start () {

        //Create new WebSocket Connection
        GameObject obj1 = GameObject.Find("Player1");

        if (obj1.GetComponent<connectScript>().IP_ADDRESS == null || obj1.GetComponent<connectScript>().PORT == null)
        {
            aux_info.text = "Connection error";
            yield return -1;

        }
        string address = "ws://" + 
            obj1.GetComponent<connectScript>().IP_ADDRESS + 
            ":" + obj1.GetComponent<connectScript>().PORT;
        ws = new WebSocket(address);

        // BUTTON SETUP
        Button btn1 = startButton.GetComponent<Button>();
        btn1.onClick.AddListener(StartOnClick);

        Button btn2 = stopButton.GetComponent<Button>();
        btn2.onClick.AddListener(StopOnClick);
       
        //GYRO SETUP
        //Set up and enable the gyroscope (check your device has one)
        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;

        //GPS_SETUP
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
            gps_info.text = "Timed out";
        else
        { 
            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                gps_info.text = "Unable to determine device location";
            }
            else
            {
                // Access granted and location value could be retrieved
                gps_info.text = "Location: " + Input.location.lastData.latitude +
                    " " + Input.location.lastData.longitude + " " +
                    Input.location.lastData.altitude + " " +
                    Input.location.lastData.horizontalAccuracy +  " " +
                    Input.location.lastData.timestamp;
            }

            // Stop service if there is no need to query location updates continuously

        }

        //Enable Magenetomert
        m_Comp = Input.compass;
        m_Comp.enabled = true;

        yield return 0;
    }


    void StartOnClick()
    {
        pressed = true;
        //Connect
        ws.Connect();
        aux_info.text = "Connectedto server";

    }

    void StopOnClick()
    {
        pressed = false;

        //Disconnect
        ws.Close();
        aux_info.text = "Disconnected from server";
    }

    SensorData GetData(Vector3 acc, Vector3 gyroRot, Quaternion gyroAtt, Vector3 comp, float mH, float tH, float lat, float lon)
    {
        SensorData value;

        value = new SensorData();

        value.accelerometer = acc;
        value.gps = new Vector2(lat, lon);
        value.gyro_rot = gyroRot;
        value.gyro_att = gyroAtt;
        value.magnetometer = new Vector2(mH, tH);
        value.magentometer_raw = comp;
        value.time_stamp = (System.DateTime.Now).Ticks.ToString();

        return value;
    }

    // Update is called once per frame
    void Update() {

        Vector3 acc, gyroRot, mag_raw;
        float latitude, longitude, mH, tH;
        Quaternion gyroAtt;

        acc = Input.acceleration;
        gyroRot = m_Gyro.rotationRate;
        gyroAtt = m_Gyro.attitude;
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        mag_raw = m_Comp.rawVector;
        mH = m_Comp.magneticHeading;
        tH = m_Comp.trueHeading;

        if (pressed)
        {
            SensorData values = GetData(acc, gyroRot, gyroAtt, mag_raw, mH, tH, latitude, longitude);
            ws.Send(JsonUtility.ToJson(values));
        }
        acc_info.text = "Acc: " + acc;
        gps_info.text = "Location: " + latitude + " " + longitude + " " + Input.location.lastData.altitude;
        gyro_info.text = "Gyro:" + gyroRot + " " + gyroAtt;
        compass_info.text = "Compass: " + mH + " " + tH ;
    }

}