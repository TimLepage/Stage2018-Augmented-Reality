using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePositions : MonoBehaviour {
    private List<Vector3> posList = new List<Vector3>();
    private List<float> distList = new List<float>();
    public Text pos;
    public Text dist;
    public GameObject flag;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    //This function saves the position and distance of the current objects in two lists
    public void SavePos()
    {
        posList.Add(flag.GetComponent<DistanceMeasurement>().getVector3(pos.text));
        distList.Add(float.Parse(dist.text.TrimEnd('m', 'c', ' ')));

        /*for (var i = 0; i < posList.Count; i++)
        {
            Debug.Log(posList.Count);
            Debug.Log(posList[i].ToString("G4"));
        }*/
    }

    public Vector3 getPosList(int i)
    {
        return posList[i];
    }
}
