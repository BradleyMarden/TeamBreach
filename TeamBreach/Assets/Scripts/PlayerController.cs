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
    private bool m_IsFacingLeft = false;
    private SpriteRenderer m_SpriteRenderer;


    //Player states NTS: CHNAGE TO PRIVATE
    public enum PlayerState {NONE, DEFAULTIDLE,DEFAULT, FASTIDLE, FAST, SLOWIDLE, SLOW, STUNNED};
    public enum PlayerLife {NONE, ALIVE, DYING, DEAD };
    public PlayerState m_PlayerState = PlayerState.NONE;
    public PlayerLife m_PlayerLife = PlayerLife.NONE;
    public bool m_IsVolatile = false;
    private bool m_IsMoving = false;
   
    [SerializeField] private float m_StunTime, m_RespawmTime;
    [SerializeField] private bool m_EngineSpeedFast;

    //Respawn
    public Transform m_StartPos;
    private bool m_FreezeController = false;

    //FMOD
    private FMOD.Studio.EventInstance i;
    [Tooltip ("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_FootstepEvent;

    //FUEL SLIDER
    [SerializeField] Image m_FuelBar;
    [SerializeField] Slider m_Slider;
    [SerializeField] private float m_MaxFuel, m_CurrentFuel;
    [SerializeField] private float m_FuelUserRate;
    public float m_LerpSpeed;


    //Control Engine Speed
    [SerializeField] float m_SlowDownAmount, m_SpeedUpAmount;

    //Animtaion
    private Animator m_Animator;

    [SerializeField] GameObject m_SmokeEffect;
    [SerializeField] GameObject m_StarEffect;

    private bool m_HasCollectable = false;
    void Start()
    {
        m_RB = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

      if (m_PlayerLife!= PlayerLife.DEAD) { SetFuelAmount();}


    }
    private void FixedUpdate()
    {
        CheckStates();
        if (!m_FreezeController)
        {
            CheckMouseInput();
            float y = Input.GetAxisRaw("Vertical");
            float x = Input.GetAxisRaw("Horizontal");
            Debug.LogError("x" + x);
            Debug.LogError("y" + y);
            float l_ForceX = x * m_Speed * Time.fixedDeltaTime;
            float l_ForceY = y * m_Speed * Time.fixedDeltaTime;
            


            if (x != 0 || y != 0) { m_IsMoving = true; }
            else { m_IsMoving = false; }


            if (x == -1) { m_IsFacingLeft = true; }
            else if(x == 1) { m_IsFacingLeft = false; }

            if (m_IsFacingLeft) 
            {
                m_SpriteRenderer.flipX = false;

            }
            else 
            {
                m_SpriteRenderer.flipX = true;

            }
            m_RB.AddForce(new Vector2(l_ForceX, l_ForceY));
        }
    }

    void Spawn() 
    {
        StopAllCoroutines();
        m_PlayerState = PlayerState.DEFAULTIDLE;
        m_PlayerLife = PlayerLife.ALIVE;
        m_CurrentFuel = m_MaxFuel;
        transform.position = m_StartPos.position;
        m_IsVolatile = false;
        m_FreezeController = false;
        transform.GetComponent<SpriteRenderer>().enabled = true;
    }

    void CheckMouseInput() 
    {




        if (m_IsMoving)
        {
            if (Input.GetKey(KeyCode.Mouse0)) { m_PlayerState = PlayerState.FASTIDLE; }
            else if (Input.GetKey(KeyCode.Mouse1)) { m_PlayerState = PlayerState.SLOWIDLE; }
            else { m_PlayerState = PlayerState.DEFAULTIDLE; }
            if (m_PlayerState == PlayerState.DEFAULTIDLE)
            {
                m_PlayerState = PlayerState.DEFAULT;
            }
            else if (m_PlayerState == PlayerState.FASTIDLE)
            {
                m_PlayerState = PlayerState.FAST;

            }
            else if (m_PlayerState == PlayerState.SLOWIDLE)
            {
                m_PlayerState = PlayerState.SLOW;

            }
        }
        else 
        {
        if (Input.GetKey(KeyCode.Mouse0)) { m_PlayerState = PlayerState.FASTIDLE; }
            else if (Input.GetKey(KeyCode.Mouse1)) { m_PlayerState = PlayerState.SLOWIDLE; }
            else { m_PlayerState = PlayerState.DEFAULTIDLE; }
        }
        
        

    }

    void PlayFootstep() 
    {
        i = FMODUnity.RuntimeManager.CreateInstance(m_FootstepEvent);
        i.start();
        i.release();
        
    }
  

    void SetFuelAmount() 
    {
        //Currently when the player dies the fuel amount is reset to max. would like lerp to zero when dead.
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
                StandardSpeed();
                break;

            case PlayerState.DEFAULTIDLE:
                m_Animator.SetInteger("State", 1);
                StandardSpeed();
                break;

            case PlayerState.DEFAULT:
                m_Animator.SetInteger("State", 2);
                m_Speed = m_NormalSpeed;
                StandardSpeed();
                break;
            
            case PlayerState.FASTIDLE:
                m_Animator.SetInteger("State", 3);

                SpeedUp();
                break;
          
            case PlayerState.FAST:
                m_Animator.SetInteger("State", 4);
                m_Speed = m_FastSpeed;
                SpeedUp();
                break;
            case PlayerState.SLOWIDLE:
                m_Animator.SetInteger("State", 5);

                SlowMo();
                break;

            case PlayerState.SLOW:
                m_Animator.SetInteger("State", 6);
                m_Speed = m_SlowSpeed;
                SlowMo();
                break;
            case PlayerState.STUNNED:
                m_Animator.SetInteger("State", 5);
                StartCoroutine(PlayStun());
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
                break;
        }
    }

    IEnumerator PlayStun() 
    {
        m_FreezeController = true;
        m_StarEffect.SetActive(true);
        yield return new WaitForSeconds(m_StunTime);
            m_FreezeController = false;
            Debug.LogError("dawd");
        m_StarEffect.SetActive(false);

        m_PlayerState = PlayerState.DEFAULTIDLE;
            StopAllCoroutines();


    }


    public void HasCollidedWithWall(GameObject p_Object) 
    {
        if (m_IsVolatile && p_Object.transform.tag != "Collectable") { StartCoroutine(Respawn()); Debug.LogError("You Died!"); }
        if (m_PlayerState == PlayerState.FAST) { m_PlayerState = PlayerState.STUNNED; }
        if (m_PlayerState == PlayerState.DEFAULT) { Debug.LogError("Too Fast??"); }
        if (m_PlayerState == PlayerState.SLOW) { Debug.LogError("Wow, you must be bad, huh..."); }


    }

    IEnumerator Respawn() 
    {
        m_PlayerLife = PlayerLife.DEAD;
        m_PlayerState = PlayerState.DEFAULTIDLE;
        m_SmokeEffect.SetActive(true);
        AddDeath();

        transform.GetComponent<SpriteRenderer>().enabled = false;
        m_FreezeController = true;
        yield return new WaitForSeconds(m_RespawmTime);
        m_SmokeEffect.SetActive(false);
        Spawn();

    }

    void AddDeath() 
    {
        if (PlayerPrefs.HasKey("Deaths")) { PlayerPrefs.SetFloat("Deaths", PlayerPrefs.GetFloat("Deaths") + 1); }
        else { PlayerPrefs.SetFloat("Deaths", 1); }
    }
    void StandardSpeed() 
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        m_EngineSpeedFast = false;

    }
    void SlowMo() 
    {
        Time.timeScale = m_SlowDownAmount;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        m_EngineSpeedFast = false;
    }
    void SpeedUp() 
    {
        Time.timeScale = m_SpeedUpAmount;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        m_EngineSpeedFast = true;

    }
    void AddCollectable()
    {
        m_HasCollectable = true;
    }
    bool HasCollectable()
    {
        return m_HasCollectable;
    }
}
