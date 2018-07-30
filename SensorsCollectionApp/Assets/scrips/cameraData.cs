using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;


public class cameraData : MonoBehaviour {

    // Use this for initialization

    public RawImage background;
    public bool cameraAvailable;
    public WebCamTexture backCam;
    public Texture defaultBackground;

    [SerializeField]
    public Text orient_val;

    public AspectRatioFitter fit;

    static readonly float MaxRecordingTime = 5.0f;
    public int counter = 0;

    public Button startButton;
    public Button stopButton;
    public bool pressed = false;


    void Start () {

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            cameraAvailable = false;
            return;

        }
        for( int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height, 30);
                //background.transform.localScale =  new Vector3(-1, 1, -1);
                break;
            }
        }

        if (backCam == null)
        {
            Debug.Log("Unable to find camera");
            return;
        }

 
        backCam.Play();
        background.texture = backCam;
        cameraAvailable = true;
        //StartCoroutine(CaptureTextureAsPNG());


    }
	
	// Update is called once per frame
	void Update () {

        if (!cameraAvailable)
            return;

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        background.rectTransform.localScale = new Vector3(-1f, -1f, 1f);

        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            background.rectTransform.localEulerAngles = new Vector3(0, 0, 180);
            //orient_val.text = "180";
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            background.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            //orient_val.text = "-180";

        }
        else
        {
            background.rectTransform.localEulerAngles = new Vector3(0, 0, 90);
            //orient_val.text = "90";
        }
    }


    IEnumerator CaptureTextureAsPNG()
    {
        int counter = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Texture2D _TextureFromCamera = new Texture2D(backCam.width, backCam.height);
            _TextureFromCamera.SetPixels(backCam.GetPixels());
            _TextureFromCamera.Apply();
            byte[] bytes = _TextureFromCamera.EncodeToPNG();
            string filePath = "/SavedScreen" + counter.ToString() + ".png";
            File.WriteAllBytes(Application.persistentDataPath + filePath, bytes);
            counter++;
        }
    }

}

