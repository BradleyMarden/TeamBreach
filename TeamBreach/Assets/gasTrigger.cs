using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gasTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {

            if (collision.transform.GetComponent<PlayerController>().m_PlayerState == PlayerController.PlayerState.FAST ||
                collision.transform.GetComponent<PlayerController>().m_PlayerState == PlayerController.PlayerState.VOLATILE ||
                collision.transform.GetComponent<PlayerController>().m_PlayerState == PlayerController.PlayerState.DEFAULT)
            {
                Debug.LogError("State" + collision.transform.GetComponent<PlayerController>().m_PlayerState);
                collision.transform.GetComponent<PlayerController>().Extinguish();
            }
        }
    }
}
