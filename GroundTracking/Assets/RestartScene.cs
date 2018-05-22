using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //GameObject[] flags = GameObject.FindGameObjectsWithTag("flagtag");
        //foreach (GameObject go in flags)
        //{
        //    Destroy(go);
        //}
        //GameObject[] rulers = GameObject.FindGameObjectsWithTag("rulertag");
        //foreach (GameObject go in rulers)
        //{
        //    Destroy(go);
        //}
    }
}
