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
            if (transform.gameObject.tag != "Enemy")
            {

                collision.transform.GetComponent<PlayerController>().m_IsVolatile = true;
                collision.transform.GetComponent<PlayerController>().SetVolatile();
            }
            
            //Destroy(transform.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {

            collision.transform.GetComponent<PlayerController>().HasCollidedWithWall(transform.gameObject);
            if (transform.gameObject.tag != "Enemy")
            {

                collision.transform.GetComponent<PlayerController>().m_IsVolatile = true;
                collision.transform.GetComponent<PlayerController>().SetVolatile();
            }

            //Destroy(transform.gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {

            collision.transform.GetComponent<PlayerController>().HasCollidedWithWall(transform.gameObject);
            if (transform.gameObject.tag != "Enemy")
            {

                collision.transform.GetComponent<PlayerController>().m_IsVolatile = true;
                collision.transform.GetComponent<PlayerController>().SetVolatile();
            }

            //Destroy(transform.gameObject);
        }
    }
}
