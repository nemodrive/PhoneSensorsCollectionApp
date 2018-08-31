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
	// -- Location
	// longitude, latitude, altitude
    public Vector3 location;
    // Timestamp (in seconds since 1970) when location was last time updated
    public double loc_tp;
    // horizontal, veritcal accuracy 
    public Vector2 loc_accuracy;

    // -- Gyroscope
    public Quaternion attitude;
    public Vector3 gravity;
    public Vector3 rotationRate;
    public Vector3 rotationRateUnbiased;
    public float updateInterval;
    public Vector3 userAcceleration;

    // -- Magnetometer
	public float headingAccuracy;
	public float magneticHeading;
	public Vector3 rawVector;
	public double mag_tp;
	public float trueHeading;

	// -- Acceleration
	public Vector3 acceleration;

	public double update_tp;
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
        Input.location.Start(0.1f, 0.1f);

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

    // Update is called once per frame
    void Update() {

        SensorData s_data;
    	s_data = new SensorData();

    	// -- Location 
	    s_data.location = new Vector3(
	    	Input.location.lastData.longitude,
	    	Input.location.lastData.latitude,
	    	Input.location.lastData.altitude
	    	);

	    s_data.loc_tp = Input.location.lastData.timestamp;
	    // horizontal, veritcal accuracy 
	    s_data.loc_accuracy = new Vector2 (
	    	Input.location.lastData.horizontalAccuracy,
	    	Input.location.lastData.horizontalAccuracy
	    	);

	    // -- Gyroscope
	    s_data.attitude = m_Gyro.attitude;
	    s_data.gravity = m_Gyro.gravity;
	    s_data.rotationRate = m_Gyro.rotationRate;
	    s_data.rotationRateUnbiased = m_Gyro.rotationRateUnbiased;
	    s_data.updateInterval = m_Gyro.updateInterval;
	    s_data.userAcceleration = m_Gyro.userAcceleration;

	    // -- Magnetometer
		s_data.headingAccuracy = m_Comp.headingAccuracy;
		s_data.magneticHeading = m_Comp.magneticHeading;
		s_data.rawVector = m_Comp.rawVector;
		s_data.mag_tp = m_Comp.timestamp;
		s_data.trueHeading = m_Comp.trueHeading;

		// Acceleration
		s_data.acceleration = Input.acceleration;

		s_data.update_tp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if (pressed)
        {
            ws.Send(JsonUtility.ToJson(s_data));
        }

        acc_info.text = "Acc: " + s_data.acceleration;
        gps_info.text = "Location: " + s_data.location;
        gyro_info.text = "Gyro:" + s_data.rotationRateUnbiased + " " + s_data.attitude;
        compass_info.text = "Compass: " + s_data.magneticHeading + " " + s_data.headingAccuracy;
    }

}