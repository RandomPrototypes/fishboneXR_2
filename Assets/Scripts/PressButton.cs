using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressButton : MonoBehaviour
{
    [SerializeField]
    private Vector2 scaleCanvas = new Vector2(1,1);

    [SerializeField]
    private string text;

    [SerializeField]
    private UnityEvent onButtonPressed;

    bool buttonDown = false;

    GameObject main;
    GameObject canvas;
    GameObject textObject;

    float pressedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        main = gameObject.transform.Find("Main").gameObject;
        canvas = main.transform.Find("Canvas").gameObject;
        textObject = canvas.transform.Find("Text (TMP)").gameObject;
        canvas.transform.localScale = new Vector3(1.0f/scaleCanvas.x, 1,  1.0f/scaleCanvas.y);
        canvas.GetComponent<RectTransform>().sizeDelta = scaleCanvas;
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if(main.transform.localPosition.y < 0.35f)
            buttonDown = true;
        else if(buttonDown) {
            buttonDown = false;
            onButtonPressed?.Invoke();
        }
        float y = main.transform.localPosition.y;
        if(y < 0.3f) {
            y = 0.3f;
        } else if(y < 0.5f && pressedTime < 1.5f) {
            pressedTime += Time.deltaTime;
        } else {
            y = 0.5f;
            pressedTime = 0;
        }
        main.transform.localPosition = new Vector3(0,y,0);
    }

    public void setText(string text) {
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = text;
    }
}
