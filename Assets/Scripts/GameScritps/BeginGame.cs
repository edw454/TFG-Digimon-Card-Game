using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginGame : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float fadeSpeed = 1f;
    public float blinkSpeed = 0.5f;

    [SerializeField] private Color originalColor;

    void Start()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();

        originalColor = text.color;
    }

    System.Collections.IEnumerator Blink()
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * fadeSpeed, 1f);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        if (Input.anyKey)
        {
            Debug.Log("A key or mouse click has been detected");
            SceneManager.LoadScene("MenuScene");
        }
    }
}
