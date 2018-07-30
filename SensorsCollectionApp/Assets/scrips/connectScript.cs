using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class connectScript : MonoBehaviour {

    public Button connectBtn;
    public InputField ip_field;
    public InputField port_field;

    public string IP_ADDRESS = null;
    public string PORT = null;

	// Use this for initialization
	void Start () {

        Button btn = connectBtn.GetComponent<Button>();
        btn.onClick.AddListener(ConnectToServer);
	}
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ConnectToServer()
    {
        IP_ADDRESS = ip_field.text;
        PORT = port_field.text;

        SceneManager.LoadScene("SecondScene");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
