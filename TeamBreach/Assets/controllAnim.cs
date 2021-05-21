using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class controllAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Animator am;

    public GameObject tabContainer;
    public GameObject StatsCanvas;
    public GameObject InstrucCanvas;

    public TMP_Text tt;
    public TMP_Text ft;
    public TMP_Text d;
    public TMP_Text cww;
    public TMP_Text v;
    public TMP_Text de;
    public TMP_Text s;
    public Sprite fuze;

    public bool window = false;






    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            window = !window;
            if (window)
            {
                Showstats();
            }
            else
            {
                Close();
            }
        }
    }


    public void LoadGameScene() 
    {
        am.GetComponent<Animator>().enabled = true;
        Debug.Log("OVer" + gameObject.name);
        SceneManager.LoadScene(1);
    }
    public void CloseGame() 
    {
        am.GetComponent<Animator>().enabled = true;

        Application.Quit();
    }

    public void Showstats() 
    {
        am.GetComponent<Animator>().enabled = true;
        tabContainer.SetActive(true);
        StatsCanvas.SetActive(true);

        if (PlayerPrefs.HasKey("Fastest Time"))
        {
            ft.text = "Fastest Time: " + PlayerPrefs.GetFloat("Fastest Time");
        }
        if (PlayerPrefs.HasKey("Total Time"))
        {
            tt.text = "Total Time: " + PlayerPrefs.GetFloat("Total Time");
        }
        if (PlayerPrefs.HasKey("Deaths"))
        {
            d.text = "Deaths: " + PlayerPrefs.GetFloat("Deaths");
        }
        if (PlayerPrefs.HasKey("WallContact"))
        {
            cww.text = "Wall crashed into: " + PlayerPrefs.GetFloat("WallContact");
        }
        if (PlayerPrefs.HasKey("Volatile"))
        {
            v.text = "Times you've been volatile: " + PlayerPrefs.GetFloat("Volatile");
        }
        if (PlayerPrefs.HasKey("Doors"))
        {
            de.text = "Walls Smashed: " + PlayerPrefs.GetFloat("Doors");
        }
        if (PlayerPrefs.HasKey("Stun"))
        {
            s.text = "Times self stunned: " + PlayerPrefs.GetFloat("Stun");
        }

    }
    public void HowToPlay()
    {
        am.GetComponent<Animator>().enabled = true;
        tabContainer.SetActive(true);
        InstrucCanvas.SetActive(true);

    }

    public void Close() 
    {
        am.GetComponent<Animator>().enabled = false;
        am.GetComponent<SpriteRenderer>().sprite = fuze;
        tabContainer.SetActive(false);
        InstrucCanvas.SetActive(false);
        StatsCanvas.SetActive(false);

    }
    private void OnMouseOver()
    {
        am.GetComponent<Animator>().enabled = true;
        Debug.Log("OVer" + gameObject.name);
    }
    private void OnMouseExit()
    {
        am.GetComponent<Animator>().enabled = false;

    }
    public void StopAnim() 
    {

    }
}
