using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundPercentageDisplay : MonoBehaviour
{
    public GameObject referenceSlider;
    
    void Update()
    {
        float currentVolume = referenceSlider.GetComponent<Slider>().value;
        float printedNumber = Mathf.RoundToInt(currentVolume*100);
        gameObject.GetComponent<TextMeshProUGUI>().text = printedNumber + "%";
    }
}
