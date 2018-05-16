using UnityEngine;
using System.Collections;

public class rotate: MonoBehaviour
{
    public float speed = 100f;
    
    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
        transform.Rotate(Vector3.left, speed * Time.deltaTime);
    }
}