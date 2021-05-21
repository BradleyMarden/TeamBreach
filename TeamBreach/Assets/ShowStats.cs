using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShowStats : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Close();
    }
    bool window = false;
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

    public GameObject tabContainer;
    public GameObject StatsCanvas;

    public TMP_Text tt;
    public TMP_Text ft;
    public TMP_Text d;
    public TMP_Text cww;
    public TMP_Text v;
    public TMP_Text de;
    public TMP_Text s;

    public void Showstats()
    {
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

    public void Close()
    {
        tabContainer.SetActive(false);
        StatsCanvas.SetActive(false);

    }
    public void LoadMenu() 
    {
        SceneManager.LoadScene("Menu");


    }
}
