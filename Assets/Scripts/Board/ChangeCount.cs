using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void GetCountText(string count)
    {
        text.text = count; 
    }
}
