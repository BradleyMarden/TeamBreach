using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightflicker : MonoBehaviour
{

    [SerializeField] private float m_FlickerSpeed, m_SpotSpeed, m_IntensityMin, m_IntensityMax, m_SpotAngleMin, m_SpotAngleMax;

    private float m_NewIntensity, m_SpotAngle;
    private Light m_Light;

    [SerializeField] private float m_Time = 0;
    [SerializeField] private float m_SpotTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_Light = GetComponent<Light>();
        StartCoroutine(Flicker());
        StartCoroutine(AngleSpot());

    }

    // Update is called once per frame
    void Update()
    {
        m_Light.intensity = Mathf.Lerp(m_Light.intensity, m_NewIntensity, m_Time * Time.deltaTime);
        m_Light.spotAngle = Mathf.Lerp(m_Light.spotAngle, m_SpotAngle, m_SpotTime * Time.deltaTime);
        // transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + 2, transform.rotation.w);

    }

    IEnumerator Flicker()
    {
        yield return new WaitForSeconds(Random.Range(0, m_FlickerSpeed));

        // m_Light.intensity = Mathf.Lerp(m_Light.intensity, Random.Range(m_IntensityMin, m_IntensityMax),2);
        m_NewIntensity = Random.Range(m_IntensityMin, m_IntensityMax);
        StartCoroutine(Flicker());

    }
    IEnumerator AngleSpot()
    {
        yield return new WaitForSeconds(Random.Range(0, m_SpotSpeed));

        m_SpotAngle = Random.Range(m_SpotAngleMin, m_SpotAngleMax);
        StartCoroutine(AngleSpot());

    }
}