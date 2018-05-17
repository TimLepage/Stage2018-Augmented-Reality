using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCubeColor : MonoBehaviour {
    private Color original;
    private GameObject text;
    // Use this for initialization
    void Start () {
		original = gameObject.GetComponent<Renderer>().material.color;
        text = GameObject.Find("New Text");
    }


    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000))
            {
                gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
                text.GetComponent<Renderer>().enabled = true;
            }
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = original;
            text.GetComponent<Renderer>().enabled = false;
        }

        float coeff = GameObject.Find("Slider").GetComponent<Slider>().value;
        gameObject.transform.localScale = new Vector3(0.2f + coeff, 0.2f + coeff, 0.2f + coeff);
    }
}
