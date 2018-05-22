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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }
    
    public void OnClick()
    {
        GameObject f = GameObject.Instantiate(flag);
        f.transform.position = new Vector3(0, 0.00784f, 0);
        f.transform.localScale = new Vector3(0.001f, 0.008f, 0.001f);


        coord_new = new Vector3(0, 0, 0);
        coord_old = getVector3(pos.text);
        GameObject instanciated_ruler = GameObject.Instantiate(ruler);
        //Calculates the distance between the two last points and displays it in the field "dist" and "dist3D"
        d = (float)Math.Sqrt(Math.Pow(coord_new.x - coord_old.x, 2) + Math.Pow(coord_new.z - coord_old.z, 2)) * 100;
        dist.text = d.ToString("G4") + " cm";
        instanciated_ruler.GetComponentInChildren<TextMesh>().text = d.ToString("G4") + " cm";
        pos.text = "0.0, 0.0, 0.0";
        //places the object "Ruler" in between the two last points
        instanciated_ruler.transform.position = new Vector3((coord_old.x + coord_new.x) / 2, (coord_old.y + coord_new.y) / 2, (coord_old.z + coord_new.z) / 2);
        instanciated_ruler.transform.localScale = new Vector3(d / 100, 0.0008f, 0.0008f);
        //Gets the angle between the vector created with the two last points and the x axis
        if (coord_new.z > coord_old.z)
        {
            angle = 360 - GetAngle_x(new Vector3(coord_new.x - coord_old.x, coord_new.y - coord_old.y, coord_new.z - coord_old.z));
        }
        else
        {
            angle = GetAngle_x(new Vector3(coord_new.x - coord_old.x, coord_new.y - coord_old.y, coord_new.z - coord_old.z));
        }
        instanciated_ruler.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
    }

    public float GetAngle_x(Vector3 ab)
    {
        return (float)Math.Acos(ab.x / Math.Sqrt(ab.x * ab.x + ab.y * ab.y + ab.z * ab.z)) * 180 / (float)Math.PI;
    }

    //This function creates a Vector3 from a string
    public Vector3 getVector3(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }
}
