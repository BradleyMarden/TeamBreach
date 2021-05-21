using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreeakDown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player") 
        {

            if (collision.transform.GetComponent<PlayerController>().m_HasCollectable) 
            {
                transform.gameObject.SetActive(false);
                collision.transform.GetComponent<PlayerController>().BreakThroughWall();
            }
        }
    }
}
