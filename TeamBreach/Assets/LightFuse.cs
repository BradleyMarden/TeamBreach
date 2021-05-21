using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFuse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject door;
    public GameObject fuses;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.transform.GetComponent<PlayerController>().m_PlayerState == PlayerController.PlayerState.SLOW) 
            {
                // Debug.LogError("State" + collision.transform.GetComponent<PlayerController>().m_PlayerState);
                // collision.transform.GetComponent<PlayerController>().Extinguish();
                transform.GetComponent<Animator>().enabled = true;
                door.SetActive(false);
                fuses.SetActive(false);
                collision.transform.GetComponent<PlayerController>().ExtinguishSound();

            }
        }
    }
}
