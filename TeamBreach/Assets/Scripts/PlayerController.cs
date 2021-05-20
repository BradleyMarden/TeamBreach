using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{

    //Movement Controlls
    [SerializeField]
    private float m_Speed = 5000, m_NormalSpeed = 5000, m_FastSpeed = 10000, m_SlowSpeed = 2000;
    private Rigidbody2D m_RB;

    //Player states NTS: CHNAGE TO PRIVATE
    public enum PlayerState {NONE, DEFAULT, FAST, SLOW };
    public enum PlayerLife {NONE, ALIVE, DYING, DEAD };
    public bool m_IsVolatile = false;
    public PlayerState m_PlayerState = PlayerState.NONE;
    public PlayerLife m_PlayerLife = PlayerLife.NONE;

    //Respawn
    public Transform m_StartPos;

    private bool m_FreezeController = false;


    private FMOD.Studio.EventInstance i;

    [SerializeField] Image m_FuelBar;
    [SerializeField] Slider m_Slider;
    [SerializeField] private float m_MaxFuel, m_CurrentFuel;
    [SerializeField] private float m_FuelUserRate;
    public float m_LerpSpeed;
    [SerializeField] float m_SlowDownAmount, m_SpeedUpAmount;

    private Animator m_Animator;
    void Start()
    {
        m_RB = transform.GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

        if (m_PlayerLife!= PlayerLife.DEAD) { SetFuelAmount();}


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

    void Spawn() 
    {
        m_PlayerLife = PlayerLife.ALIVE;
        m_CurrentFuel = m_MaxFuel;
        transform.position = m_StartPos.position;
        m_IsVolatile = false;
        m_FreezeController = false;
        transform.GetComponent<SpriteRenderer>().enabled = true;
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
    void CheckFuel() 
    {

    }

    void SetFuelAmount() 
    {
        if (m_CurrentFuel <= 0) { StartCoroutine(Respawn()); }

        m_CurrentFuel -= m_FuelUserRate * Time.deltaTime; 
        //m_FuelBar.GetComponent<Renderer>().material.SetFloat("_Fuel", m_CurrentFuel);
        m_FuelBar.material.SetFloat("_Fuel",  m_CurrentFuel/m_MaxFuel);
        m_Slider.value = m_CurrentFuel / m_MaxFuel;
        
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
                m_Animator.SetInteger("State", 1);
                m_Speed = m_NormalSpeed;
                Time.timeScale = 1;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                break;
            case PlayerState.FAST:
                m_Animator.SetInteger("State", 2);
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

        switch (m_PlayerLife) 
        {
            case PlayerLife.NONE:
                break;
            case PlayerLife.ALIVE:
                break;
            case PlayerLife.DYING:
                break;
            case PlayerLife.DEAD:
                Debug.LogError("DEAD");
                float lerped = Mathf.Lerp(m_CurrentFuel / m_MaxFuel, 0, m_LerpSpeed* Time.deltaTime);
                //m_FuelBar.material.SetFloat("_Fuel", lerped);

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
        m_PlayerLife = PlayerLife.DEAD;
        transform.GetComponent<SpriteRenderer>().enabled = false;
        m_FreezeController = true;
        yield return new WaitForSeconds(1);
        // transform.position = m_StartPos.position;
        Spawn();

    }



}
