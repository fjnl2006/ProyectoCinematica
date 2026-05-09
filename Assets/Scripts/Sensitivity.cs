using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Sensitivity : MonoBehaviour
{
    [Header("Sensibility")]
    [SerializeField] private Slider sliderSens;
    [SerializeField] private TextMeshProUGUI sliderSensText;
    [SerializeField] private Cannon cannon;

    private void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("currentSensitivity", cannon.rotationSpeed);
        cannon.rotationSpeed = savedSensitivity;
        sliderSens.value = savedSensitivity;
        sliderSensText.text = (savedSensitivity * 1).ToString("0.00");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        
        sliderSens.onValueChanged.AddListener(OnSliderChanged);
    }

    // Update is called once per frame
    private void OnSliderChanged(float value)
    {
        if (cannon != null)
        {
            cannon.rotationSpeed = value;
            PlayerPrefs.SetFloat("currentSensitivity", value);
        }
        
        if (sliderSensText != null)
        {
            sliderSensText.text = (value * 1).ToString("0.00");
        }
    }
}
