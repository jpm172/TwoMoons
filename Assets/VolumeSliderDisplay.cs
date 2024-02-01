using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderDisplay : MonoBehaviour
{
    public Slider slider;

    public void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        //yourFloat = Mathf.Round(yourFloat * 100f) / 100f;
        GetComponent<TextMeshProUGUI>().text = slider.value.ToString("P0");
    }

}
