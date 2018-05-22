using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceMeasurement : MonoBehaviour {
    private Vector3 coord_old;
    private Vector3 coord_new;
    private float d;
    public Text pos;
    public Text dist;
    public TextMesh dist3D; 
    public GameObject ruler;
    private float angle;

    // Use this for initialization
    void Start() {
        //Gets the coordinates of the last point from the text field "pos"
        coord_old = getVector3(pos.text);
        dist.text = "0";
        //Gets the coordinates of the new point and displays it in the text field "pos" with 4 significant numbers
        coord_new = transform.position;
        pos.text = coord_new.ToString("G4");
        angle = 0;
        GameObject instanciated_ruler = GameObject.Instantiate(ruler);
        //Calculates the distance between the two last points and displays it in the field "dist" and "dist3D"
        d = (float)Math.Sqrt(Math.Pow(coord_new.x - coord_old.x, 2) + Math.Pow(coord_new.z - coord_old.z, 2)) * 100;
        dist.text = d.ToString("G4") + " cm";
        dist3D.text = d.ToString("G4") + " cm";
        //places the object "Ruler" in between the two last points
        ruler.transform.position = new Vector3((coord_old.x + coord_new.x) / 2, (coord_old.y + coord_new.y) / 2, (coord_old.z + coord_new.z) / 2);
        ruler.transform.localScale = new Vector3(d / 100, 0.0008f, 0.0008f);
        //Gets the angle between the vector created with the two last points and the x axis
        if (coord_new.z > coord_old.z)
        {
            angle = 360 - GetAngle_x(new Vector3(coord_new.x - coord_old.x, coord_new.y - coord_old.y, coord_new.z - coord_old.z));
        }
        else
        {
            angle = GetAngle_x(new Vector3(coord_new.x - coord_old.x, coord_new.y - coord_old.y, coord_new.z - coord_old.z));
        }
        ruler.transform.rotation = Quaternion.Euler(0f, angle, 0f);

    }

    // Update is called once per frame
    void Update() {
        
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

    //This function calculates the angle between a vector and the x axis
    public float GetAngle_x(Vector3 ab)
    {
        return (float)Math.Acos(ab.x/Math.Sqrt(ab.x*ab.x +ab.y*ab.y + ab.z*ab.z))*180/(float)Math.PI;
    }

}


