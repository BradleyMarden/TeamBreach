using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Movement Controlls
    [SerializeField]
    private float m_Speed = 5000, m_NormalSpeed = 5000, m_FastSpeed = 10000, m_SlowSpeed = 2000;
    private Rigidbody2D m_RB;

    //Player states NTS: CHNAGE TO PRIVATE
    public enum PlayerState {NONE, DEFAULT, FAST, SLOW };
    public bool m_IsVolatile = false;
    public PlayerState m_PlayerState = PlayerState.NONE;

    //Respawn
    public Transform m_StartPos;

    private bool m_FreezeController = false;


    private FMOD.Studio.EventInstance i;


    [SerializeField] float m_SlowDownAmount, m_SpeedUpAmount;
    void Start()
    {
        m_RB = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        CheckMouseInput();
        CheckStates();
        if (!m_FreezeController)
        {
            float y = Input.GetAxisRaw("Vertical");
            float x = Input.GetAxisRaw("Horizontal");
            float l_ForceX = x * m_Speed * Time.fixedDeltaTime;
            float l_ForceY = y * m_Speed * Time.fixedDeltaTime;
            m_RB.AddForce(new Vector2(l_ForceX, l_ForceY));
        }
    }


    void CheckMouseInput() 
    {
        if (Input.GetKey(KeyCode.Mouse0)) { m_PlayerState = PlayerState.FAST; }
        else if (Input.GetKey(KeyCode.Mouse1)) { m_PlayerState = PlayerState.SLOW; }
        else { m_PlayerState = PlayerState.DEFAULT; }

    }

    void PlayFootstep() 
    {
        i = FMODUnity.RuntimeManager.CreateInstance("event:/New Event 2");
        i.start();
        i.release();
        
    }

    void CheckStates() 
    {
        switch (m_PlayerState) 
        {
            case PlayerState.NONE:
                m_Speed = 0;
                Debug.LogError("Player has no state set!");
                break;
            case PlayerState.DEFAULT:
                m_Speed = m_NormalSpeed;
                Time.timeScale = 1;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                break;
            case PlayerState.FAST:
                m_Speed = m_FastSpeed;
                Time.timeScale = m_SpeedUpAmount;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                break;
            case PlayerState.SLOW:
                m_Speed = m_SlowSpeed;
                Time.timeScale = m_SlowDownAmount;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                break;
        }
    }

    public void HasCollidedWithWall(GameObject p_Object) 
    {
        if (m_IsVolatile && p_Object.transform.tag != "Collectable") { StartCoroutine(Respawn()); Debug.LogError("You Died!"); }
        if (m_PlayerState == PlayerState.FAST) { Debug.LogError("Slow Down Cowboy"); }
        if (m_PlayerState == PlayerState.DEFAULT) { Debug.LogError("Too Fast??"); }
        if (m_PlayerState == PlayerState.SLOW) { Debug.LogError("Wow, you must be bad, huh..."); }


    }

    IEnumerator Respawn() 
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        m_FreezeController = true;
        yield return new WaitForSeconds(1);
        transform.position = m_StartPos.position;
        m_IsVolatile = false;
        m_FreezeController = false;
        transform.GetComponent<SpriteRenderer>().enabled = true;

    }



}
