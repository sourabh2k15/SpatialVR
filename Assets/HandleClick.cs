using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HandleClick : MonoBehaviour {
    public Button b;
	// Use this for initialization
	void Start () {
        b = GameObject.Find("Button").GetComponent<Button>();
        b.onClick.AddListener(loadNext);
	}

    void loadNext() {
        Debug.Log("scene loaded!");
        SceneManager.LoadScene("main", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("init");
    }
	// Update is called once per frame
	void Update () {
		
	}
}
