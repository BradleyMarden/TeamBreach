using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToPos : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform Point1;
    public Transform Point2;
    public float speed;
    public bool randomTime = false;
    float rSpeed;
    void Start()
    {
        transform.position = Point1.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Point2.position , speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, Point2.position) <=1)
        {
           transform.position = Point1.transform.position;
        }
    }
}
