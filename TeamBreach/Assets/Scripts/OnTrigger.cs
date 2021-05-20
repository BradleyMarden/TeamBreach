using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            
            collision.transform.GetComponent<PlayerController>().HasCollidedWithWall(transform.gameObject);
            collision.transform.GetComponent<PlayerController>().m_IsVolatile = true;
            //Destroy(transform.gameObject);
        }
    }

}
