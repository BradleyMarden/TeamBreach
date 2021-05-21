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
    public enum PlayerState {NONE, DEFAULTIDLE,DEFAULT, FASTIDLE, FAST, SLOWIDLE, SLOW, STUNNED, VOLATILE};
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
    private FMOD.Studio.EventInstance m_FootstepInstance;
    [Tooltip ("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_FootstepEvent;

    private FMOD.Studio.EventInstance m_CharacterInstance;
    [Tooltip("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_CharacterEvent;


    private FMOD.Studio.EventInstance m_ExtinguishInstance;
    [Tooltip("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_ExtinguishEvent;

    private FMOD.Studio.EventInstance m_HeadbuttInstance;
    [Tooltip("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_HeadbuttEvent;


    private FMOD.Studio.EventInstance m_WallContactInstance;
    [Tooltip("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_WallContactEvent;


    private FMOD.Studio.EventInstance m_PlayerAmbienceInstance;
    [Tooltip("Enter the event path e.g: event:/New Event")]
    [SerializeField] string m_PlayerAmbienceEvent;


    //FUEL SLIDER
    [SerializeField] Image m_FuelBar;
    [SerializeField] Slider m_Slider;
    [SerializeField] private float m_MaxFuel, m_CurrentFuel;
    [SerializeField] private float m_FuelUserRate;
    public float m_LerpSpeed;

    bool isStateFast = false;
    bool isStateSlow = false;
    bool isStateDefault = false;

    //Control Engine Speed
    [SerializeField] float m_SlowDownAmount, m_SpeedUpAmount;

    //Animtaion
    private Animator m_Animator;

    [SerializeField] GameObject m_SmokeEffect;
    [SerializeField] GameObject m_StarEffect;
    [SerializeField] GameObject m_ClockEffect;
    PlayerState m_OldPLayerState;
    private bool m_HasCollectable = false;
    void Start()
    {
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("State", 3f);
        m_CharacterInstance = FMODUnity.RuntimeManager.CreateInstance(m_CharacterEvent);
        m_CharacterInstance.start();
        m_CharacterInstance.release();

        m_ExtinguishInstance = FMODUnity.RuntimeManager.CreateInstance(m_ExtinguishEvent);
        m_HeadbuttInstance = FMODUnity.RuntimeManager.CreateInstance(m_HeadbuttEvent);
        m_WallContactInstance = FMODUnity.RuntimeManager.CreateInstance(m_WallContactEvent);
        m_PlayerAmbienceInstance = FMODUnity.RuntimeManager.CreateInstance(m_PlayerAmbienceEvent);
        m_PlayerAmbienceInstance.start();
        m_PlayerAmbienceInstance.release();
               m_RB = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

      if (m_PlayerLife!= PlayerLife.DEAD) { SetFuelAmount();}

      if(m_PlayerState != m_OldPLayerState) 
        {
            m_CharacterInstance = FMODUnity.RuntimeManager.CreateInstance(m_CharacterEvent);
            m_ExtinguishInstance = FMODUnity.RuntimeManager.CreateInstance(m_ExtinguishEvent);
            m_HeadbuttInstance = FMODUnity.RuntimeManager.CreateInstance(m_HeadbuttEvent);
            m_WallContactInstance = FMODUnity.RuntimeManager.CreateInstance(m_WallContactEvent);

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
                    if (!isStateDefault)
                    {
                        m_PlayerAmbienceInstance.setParameterByName("State", 0);
                        m_PlayerAmbienceInstance.start();
                        m_PlayerAmbienceInstance.release();
                    }
                    isStateSlow = false;
                    isStateFast = false;
                    break;

                case PlayerState.DEFAULT:
                    m_Animator.SetInteger("State", 2);

                    if (!isStateDefault)
                    {
                        m_PlayerAmbienceInstance.setParameterByName("State", 0);
                        m_PlayerAmbienceInstance.start();
                        m_PlayerAmbienceInstance.release();
                    }
                    isStateSlow = false;
                    isStateFast = false;
                    isStateDefault = true;
                    m_Speed = m_NormalSpeed;
                    StandardSpeed();
                    break;

                case PlayerState.FASTIDLE:
                    m_Animator.SetInteger("State", 3);
                    if (!isStateFast)
                    {
                       
                        m_CharacterInstance.setParameterByName("State", 1f);
                        m_CharacterInstance.start();
                        m_CharacterInstance.release();
                        m_PlayerAmbienceInstance.setParameterByName("State", 1);
                        m_PlayerAmbienceInstance.start();
                        m_PlayerAmbienceInstance.release();
                    }

                    isStateSlow = false;
                    isStateDefault = false;

                    SpeedUp();
                    break;

                case PlayerState.FAST:
                    m_Animator.SetInteger("State", 4);
                    if (!isStateFast)
                    {

                        m_CharacterInstance.setParameterByName("State", 1f);
                        m_CharacterInstance.start();
                        m_CharacterInstance.release();
                        m_PlayerAmbienceInstance.setParameterByName("State", 1);
                        m_PlayerAmbienceInstance.start();
                        m_PlayerAmbienceInstance.release();
                    }
                    isStateFast = true;
                    isStateSlow = false;
                    isStateDefault = false;

                    m_Speed = m_FastSpeed;
                    SpeedUp();
                    break;
                case PlayerState.SLOWIDLE:
                    m_Animator.SetInteger("State", 5);
                    if (!isStateSlow)
                    {
                       
                        m_CharacterInstance.setParameterByName("State", 2f);
                        m_CharacterInstance.start();
                        m_CharacterInstance.release();
                        m_PlayerAmbienceInstance.setParameterByName("State", 2);
                        m_PlayerAmbienceInstance.start();
                        m_PlayerAmbienceInstance.release();
                    }
                    isStateFast = false;
                    isStateDefault = false;


                    SlowMo();
                    break;

                case PlayerState.SLOW:
                    m_Animator.SetInteger("State", 6);
                    if (!isStateSlow)
                    {

                        m_CharacterInstance.setParameterByName("State", 2f);
                        m_CharacterInstance.start();
                        m_CharacterInstance.release();
                        m_PlayerAmbienceInstance.setParameterByName("State", 2);
                        m_PlayerAmbienceInstance.start();
                        m_PlayerAmbienceInstance.release();
                    }
                    isStateSlow = true;
                    isStateFast = false;
                    isStateDefault = false;

                    m_Speed = m_SlowSpeed;
                    SlowMo();
                    break;
                case PlayerState.STUNNED:
                    isStateSlow = false;
                    isStateFast = false;
                    isStateDefault = false;
                    m_PlayerAmbienceInstance.setParameterByName("State", 2);
                    m_PlayerAmbienceInstance.start();
                    m_PlayerAmbienceInstance.release();
                    m_Animator.SetInteger("State", 5);
                    StartCoroutine(PlayStun());
                    m_CharacterInstance.start();
                    m_CharacterInstance.release();
                    SlowMo();

                    break;
                case PlayerState.VOLATILE:
                    m_Animator.SetInteger("State", 7);
                    isStateSlow = false;
                    isStateFast = false;
                    isStateDefault = false;

                    SpeedUp();
                    m_CharacterInstance.setParameterByName("State", 3f);
                    m_CharacterInstance.start();
                    m_CharacterInstance.release();


                    m_PlayerAmbienceInstance.setParameterByName("State", 3);
                    m_PlayerAmbienceInstance.start();
                    m_PlayerAmbienceInstance.release();
                    //StartCoroutine(PlayStun());
                    break;

            }

            m_OldPLayerState = m_PlayerState;
        }

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
        isStateSlow = false;
        isStateFast = false;
        isStateDefault = false;
    }

    void CheckMouseInput() 
    {




        if (m_IsMoving && m_PlayerState != PlayerState.VOLATILE)
        {
            if (Input.GetKey(KeyCode.Mouse0)) 
            { 
                m_PlayerState = PlayerState.FASTIDLE;
               // m_CharacterInstance = FMODUnity.RuntimeManager.CreateInstance(m_CharacterEvent);
               // m_CharacterInstance.start();
                //m_CharacterInstance.release();

            }
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
        else if (m_PlayerState != PlayerState.VOLATILE)
        {
            
        if (Input.GetKey(KeyCode.Mouse0)) 
            { 
                m_PlayerState = PlayerState.FASTIDLE;
               
            }
            else if (Input.GetKey(KeyCode.Mouse1)) { m_PlayerState = PlayerState.SLOWIDLE; }
            else { m_PlayerState = PlayerState.DEFAULTIDLE; }
        }
        
        

    }

    void PlayFootstep() 
    {
       m_FootstepInstance = FMODUnity.RuntimeManager.CreateInstance(m_FootstepEvent);
        m_FootstepInstance.start();
        m_FootstepInstance.release();
        
    }
  

    void SetFuelAmount() 
    {
        //Currently when the player dies the fuel amount is reset to max. would like lerp to zero when dead.
        if (m_CurrentFuel <= 0) {

            m_ExtinguishInstance.setParameterByName("State", 1f);
            m_ExtinguishInstance.start();
            m_ExtinguishInstance.release();
            StartCoroutine(Respawn()); 

        }

        m_CurrentFuel -= m_FuelUserRate * Time.deltaTime; 
        //m_FuelBar.GetComponent<Renderer>().material.SetFloat("_Fuel", m_CurrentFuel);
        m_FuelBar.material.SetFloat("_Fuel",  m_CurrentFuel/m_MaxFuel);
        m_Slider.value = m_CurrentFuel / m_MaxFuel;
        
    }

    void FastModeStart() 
    {
       // m_CharacterInstance = FMODUnity.RuntimeManager.CreateInstance(m_CharacterEvent);
        //m_CharacterInstance.setParameterByName("State", 1f);
        //m_CharacterInstance.start();
       // m_CharacterInstance.release();
    }
    

    void CheckStates() 
    {
       /* switch (m_PlayerState) 
        {
            case PlayerState.NONE:
                m_Speed = 0;
                Debug.LogError("Player has no state set!");
                StandardSpeed();
                break;

            case PlayerState.DEFAULTIDLE:
                m_Animator.SetInteger("State", 1);
                m_CharacterInstance.setParameterByName("State", 0f);
               
                StandardSpeed();
                break;

            case PlayerState.DEFAULT:
                m_Animator.SetInteger("State", 2);
                m_CharacterInstance.setParameterByName("State", 0f);

                m_Speed = m_NormalSpeed;
                StandardSpeed();
                break;
            
            case PlayerState.FASTIDLE:
                m_Animator.SetInteger("State", 3);

               

                 SpeedUp();
                break;
          
            case PlayerState.FAST:
                m_Animator.SetInteger("State", 4);
                m_CharacterInstance.setParameterByName("State", 1f);
                played = false;


                m_Speed = m_FastSpeed;
                SpeedUp();
                break;
            case PlayerState.SLOWIDLE:
                m_Animator.SetInteger("State", 5);
                m_CharacterInstance.setParameterByName("State", 2f);
               
                SlowMo();
                break;

            case PlayerState.SLOW:
                m_Animator.SetInteger("State", 6);
                m_CharacterInstance.setParameterByName("State", 2f);

                m_Speed = m_SlowSpeed;
                SlowMo();
                break;
            case PlayerState.STUNNED:
                m_Animator.SetInteger("State", 5);
                StartCoroutine(PlayStun());
                break;
            case PlayerState.VOLATILE:
                m_Animator.SetInteger("State", 7);
                m_CharacterInstance.setParameterByName("State", 3f);
                SpeedUp();
                //StartCoroutine(PlayStun());
                break;

        }*/

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
        //m_HeadbuttInstance.setParameterByName("State", 0f);
        m_HeadbuttInstance.start();
        m_HeadbuttInstance.release();
        yield return new WaitForSeconds(m_StunTime);
        m_FreezeController = false;
            Debug.LogError("dawd");
        m_StarEffect.SetActive(false);

        m_PlayerState = PlayerState.DEFAULTIDLE;
           // StopAllCoroutines();


    }

    public void SetVolatile() { m_PlayerState = PlayerState.VOLATILE; }
    public void HasCollidedWithWall(GameObject p_Object) 
    {
        if (m_IsVolatile && p_Object.transform.tag != "Collectable") 
        {
            m_ExtinguishInstance.setParameterByName("State", 3f);
            m_ExtinguishInstance.start();
            m_ExtinguishInstance.release();
            StartCoroutine(Respawn()); 
            Debug.LogError("You Died!"); 
        }
        if (m_PlayerState == PlayerState.FAST) 
        {m_PlayerState = PlayerState.STUNNED;}
        if (m_PlayerState == PlayerState.DEFAULT) { Debug.LogError("Too Fast??"); }
        if (m_PlayerState == PlayerState.SLOW) { Debug.LogError("Wow, you must be bad, huh..."); }
        m_WallContactInstance.start();
        m_WallContactInstance.release();


    }

    IEnumerator Respawn() 
    {

        m_PlayerLife = PlayerLife.DEAD;
        m_PlayerState = PlayerState.DEFAULTIDLE;
        m_SmokeEffect.SetActive(true);
        m_ClockEffect.SetActive(true);
        AddDeath();

        transform.GetComponent<SpriteRenderer>().enabled = false;
        m_FreezeController = true;
        yield return new WaitForSeconds(m_RespawmTime);
        m_SmokeEffect.SetActive(false);
        m_ClockEffect.SetActive(false);

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
