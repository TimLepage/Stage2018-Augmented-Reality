using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour {
    public GameObject ruler;
    public GameObject flag;
    private Vector3 coord_old;
    private Vector3 coord_new;
    private float d;
    public Text pos;
    public Text dist;
    public TextMesh dist3D;
    private float angle;
    public GameObject pf;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }
    
    //This function closes the drawing
    public void OnClick()
    {
        //Gets the position of the first flag
        coord_new = pf.GetComponent<SavePositions>().getPosList(1);
        coord_old = flag.GetComponent<DistanceMeasurement>().getVector3(pos.text);
        GameObject instanciated_ruler = GameObject.Instantiate(ruler);
        //Calculates the distance between the two last points and displays it in the field "dist" and "dist3D"
        d = (float)Math.Sqrt(Math.Pow(coord_new.x - coord_old.x, 2) + Math.Pow(coord_new.z - coord_old.z, 2)) * 100;
        dist.text = d.ToString("G4") + " cm";
        instanciated_ruler.GetComponentInChildren<TextMesh>().text = d.ToString("G4") + " cm";
        pos.text = coord_new.ToString("G4");
        //places the object "Ruler" in between the two last points
        instanciated_ruler.transform.position = new Vector3((coord_old.x + coord_new.x) / 2, (coord_old.y + coord_new.y) / 2, (coord_old.z + coord_new.z) / 2);
        instanciated_ruler.transform.localScale = new Vector3(d / 100, 0.0008f, 0.0008f);
        //Gets the angle between the vector created with the two last points and the x axis
        if (coord_new.z > coord_old.z)
        {
            angle = 360 - flag.GetComponent<DistanceMeasurement>().GetAngle_x(new Vector3(coord_new.x - coord_old.x, coord_new.y - coord_old.y, coord_new.z - coord_old.z));
        }
        else
        {
            angle = flag.GetComponent<DistanceMeasurement>().GetAngle_x(new Vector3(coord_new.x - coord_old.x, coord_new.y - coord_old.y, coord_new.z - coord_old.z));
        }
        instanciated_ruler.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
    }
}
