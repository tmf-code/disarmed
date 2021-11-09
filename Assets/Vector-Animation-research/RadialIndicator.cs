using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadialIndicator : MonoBehaviour
{
    [Header("Radial Timers")]
    [SerializeField]
    private float indicatorTimer = 0.0f;

    [SerializeField]
    private float maxIndicatorTimer = 1.0f;

    [Header("UI Indicator")]
    [SerializeField]
    private Image radialIndicatorUI = null;

    [Header("Key Codes")]
    [SerializeField]
    private KeyCode selectKey = KeyCode.Mouse0;

    [Header("Unity Event")]
    [SerializeField]
    private UnityEvent myEvent = null;

    private bool shouldUpdate = false;

    private bool active = false;

    private void Update()
    {

        active = Input.GetKey(selectKey);  

        if (active)
        {
            shouldUpdate = false;
            indicatorTimer += Time.deltaTime * 0.5f;
            radialIndicatorUI.enabled = true;
            radialIndicatorUI.fillAmount = indicatorTimer;

            if (indicatorTimer >= maxIndicatorTimer)
            {
                indicatorTimer = maxIndicatorTimer;
                radialIndicatorUI.fillAmount = maxIndicatorTimer;
                radialIndicatorUI.enabled = false;
                myEvent.Invoke();
            }
        }
        else
        {
            if (shouldUpdate)
            {
                indicatorTimer -= Time.deltaTime;
                radialIndicatorUI.fillAmount = indicatorTimer;

                if (indicatorTimer <= 0)
                {
                    indicatorTimer = 0;
                    radialIndicatorUI.fillAmount = 0;
                    radialIndicatorUI.enabled = false;
                    shouldUpdate = false;
                }
            }
        }

        if (Input.GetKeyUp(selectKey))
        {
            shouldUpdate = true;
        }
    }
}
