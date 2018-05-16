using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // gameObject.GetComponent<Renderer>().enabled = false;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
