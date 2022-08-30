using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        transform.Find("ExitButton").GetComponent<TextMeshProUGUI>().GetLocalizationText("Exit");
    }
}
