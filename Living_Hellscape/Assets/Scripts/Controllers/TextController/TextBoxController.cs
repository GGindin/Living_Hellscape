using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBoxController : MonoBehaviour
{
    public static TextBoxController instance { get; set; }
    public float charDelay = 0.01f; //seconds
    public float blinkDelay = 0.75f; // seconds
    public int maxChars = 96;
    public TMP_Text currentText;
    public Image blinkArrow;
    public bool isFinished;

    private string text = "";
    private string buffer = "";
    private float timeSinceLastChar;
    private float timeSinceLastBlink;
    private int itr = 0;
    private bool waitingForInput = false;
    private bool atEnd = false;
    private float buttonCooldown = 0.2f;
    private float buttonCooldownTimer = 0;
    private UserInput userInput;

    private System.Action callBack;
    private bool setTextImmediate;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    
    public void OpenTextBox(string newText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            text = newText;
            atEnd = false;
            waitingForInput = false;
            isFinished = false;
            itr = 0;
        }
    }

    public void OpenTextBoxImmediate(string newText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            text = newText;
            atEnd = false;
            waitingForInput = false;
            isFinished = false;
            setTextImmediate = true;
            SetTextImmediate();
            itr = 0;
        }
    }

    public void OpenTextBoxWithCallBack(string newText, System.Action callBack)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            text = newText;
            atEnd = false;
            waitingForInput = false;
            isFinished = false;
            itr = 0;
            this.callBack = callBack;
        }
    }

    public void CloseTextBox()
    {
        GameController.Instance.SetStopUpdates(false);
        gameObject.SetActive(false);
        currentText.text = "";
        buffer = "";
    }

    private void UpdateTextbox()
    {
        if (buffer.Length > maxChars && buffer[buffer.Length - 1] == ' ')
        {
            string newBuffer = "";
            buffer = newBuffer;
            waitingForInput = true;
        }
        else if (!waitingForInput)
        {
            currentText.text = buffer;
        }
    }

    private void Update()
    {
        userInput = InputController.GetUserInput();
        if (gameObject.activeSelf) {
            GameController.Instance.SetStopUpdates(true);
            if (isFinished == true)
            {
                if(callBack != null)
                {
                    callBack();
                    callBack = null;
                }
                CloseTextBox();
            }
            UpdateTextbox();
        }
        if (gameObject.activeSelf && !waitingForInput)
        {
            timeSinceLastChar += Time.deltaTime;
            if (timeSinceLastChar > charDelay)
            {
                for (int i = 0; i < (int)(timeSinceLastChar / charDelay); i++)
                {
                    buffer += text[itr];
                    itr++;

                    if (itr > text.Length - 1)
                        break;

                    if (text[itr] == ' ')
                        break;
                }
                timeSinceLastChar = 0;
            }
        }
        else if (gameObject.activeSelf && waitingForInput)
        {
            timeSinceLastBlink += Time.deltaTime;
            if (timeSinceLastBlink > blinkDelay)
            {
                if (blinkArrow.gameObject.activeSelf)
                    blinkArrow.gameObject.SetActive(false);
                else
                    blinkArrow.gameObject.SetActive(true);

                timeSinceLastBlink = 0;
            }
            if (userInput.mainAction == ButtonState.Down)
            {
                waitingForInput = false;
                buttonCooldownTimer = buttonCooldown;
                if (atEnd == true)
                    isFinished = true;
            }
        }

        if (!waitingForInput && userInput.mainAction == ButtonState.Down)
        {
            if (buttonCooldownTimer <= 0)
            {
                for (; buffer.Length < maxChars; itr++)
                {
                    if (itr > (text.Length - 1))
                        break;
                    buffer += text[itr];
                }
                while (itr <= (text.Length - 1) && text[itr] != ' ')
                {
                    buffer += text[itr];
                    itr++;
                }
                buttonCooldownTimer = buttonCooldown;
            }
        }

        if (itr + 1 > text.Length) 
        {
            atEnd = true;
            UpdateTextbox();
            waitingForInput = true;
        }
        buttonCooldownTimer -= Time.deltaTime;
    }
        

    void SetTextImmediate()
    {
        for (; buffer.Length < maxChars; itr++)
        {
            if (itr > (text.Length - 1))
                break;
            buffer += text[itr];
        }
        while (itr <= (text.Length - 1) && text[itr] != ' ')
        {
            buffer += text[itr];
            itr++;
        }
        buttonCooldownTimer = buttonCooldown;
    }
}
