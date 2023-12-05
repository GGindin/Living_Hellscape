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
    public float inputDelay = 0.2f; // seconds
    public float reentryDelay = 0.5f; // seconds
    public int maxLines = 4;
    public TMP_Text currentText;
    public Image blinkArrow;

    private string buffer = string.Empty;
    private int loc = 0;
    private int currentVisibleChars = 0;
    private int totalChars = 0;

    private bool textRevealed = false;
    private bool end = false;
    public bool kill = false;
    private bool setImmediate = false;
    private bool lastBreak = false;

    private float timeSinceLastChar = 0;
    private float timeSinceLastBlink = 0;
    private float timeSinceLastInput = 0;
    private float timeAtLastExit = 0;


    private UserInput userInput;

    private System.Action callBack;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void OpenTextBox(string newText)
    {
        if (!gameObject.activeSelf && (timeAtLastExit + reentryDelay) < Time.realtimeSinceStartup)
        {
            gameObject.SetActive(true);
            buffer = newText;
            currentVisibleChars = 0;
            totalChars = 0;
            loc = 0;
            kill = false;
            end = false;
            textRevealed = false;
            processNewBox();
        }
    }

    public void OpenTextBoxImmediate(string newText)
    {
        gameObject.SetActive(true);
        currentText.text = newText;
        currentText.maxVisibleCharacters = 999;
        setImmediate = true;
    }

    private void processNewBox()
    {
        //currentText.textDisplay.enable = false;
        currentText.maxVisibleCharacters = 0;
        timeSinceLastInput = 0;
        currentText.text = "";
        currentText.ForceMeshUpdate();
        currentVisibleChars = 0;
        totalChars = 1;
        if (lastBreak == true)
        {
            currentText.text += '-';
            lastBreak = false;
        }
        while ((loc) < buffer.Length && currentText.textInfo.lineCount <= maxLines)
        {
            currentText.text += buffer[loc];
            if (((buffer[loc] == '.') || (buffer[loc] == '?')) || (buffer[loc] == '!'))
            {
                while ((loc + 1) < buffer.Length && buffer[loc + 1] == '!')
                {
                    loc++;
                    totalChars++;
                    currentText.text += buffer[loc];
                }
                loc += 2;
                textRevealed = false;
                //Debug.Log(loc);
                //Debug.Log(buffer.Length);
                if (loc > buffer.Length)
                {
                    end = true;
                }
                return;
            }
            totalChars++;
            loc++;
            currentText.ForceMeshUpdate();
        }
        if ((loc + 1) < buffer.Length)
        {
            //Debug.Log(loc);
            //Debug.Log(buffer.Length);

            loc -= 3;
            currentText.text = currentText.text.Remove(totalChars - 4);
            currentText.text += '-';
            lastBreak = true;
            
        }
        else
        {
            //Debug.Log("set end");
            end = true;
        }
        totalChars++;
        textRevealed = false;
        //currentText.textDisplay.enable = true;
    }

    private void revealCharacters()
    {
        timeSinceLastChar += Time.deltaTime;
        if (timeSinceLastChar >= charDelay)
        {
            while (timeSinceLastChar >= 0)
            {
                currentVisibleChars++;
                timeSinceLastChar -= charDelay;
            }
            if (currentVisibleChars > totalChars)
                textRevealed = true;

            currentText.maxVisibleCharacters = currentVisibleChars;
        }
    }

    public void OpenTextBoxWithCallBack(string newText, System.Action callBack)
    {
        if (!gameObject.activeSelf)
        {
            this.callBack = callBack;
            OpenTextBox(newText);
        }
    }

    public void CloseTextBox()
    {
        GameController.Instance.SetStopUpdates(false);
        gameObject.SetActive(false);
        currentText.text = "";
        buffer = "";
        setImmediate = false;
        timeAtLastExit = Time.realtimeSinceStartup;
    }



    private void Update()
    {
        userInput = InputController.GetUserInput();

        if (gameObject.activeSelf)
        {
            GameController.Instance.SetStopUpdates(true);
            if (setImmediate)
            {
                return;
            }

            if (kill)
            {
                CloseTextBox();
                if (callBack != null)
                {
                    callBack();
                    callBack = null;
                }
            }

            if (!textRevealed)
            {
                revealCharacters();
                if (timeSinceLastInput >= inputDelay && userInput.secondaryAction == ButtonState.Down)
                {
                    currentText.maxVisibleCharacters = totalChars + 1;
                    textRevealed = true;
                    timeSinceLastInput = 0;
                }
            }
            else if (!end && userInput.secondaryAction == ButtonState.Down)
            {
                if (timeSinceLastInput >= inputDelay)
                {
                    processNewBox();
                    timeSinceLastInput = 0;
                }
            }
            else if (end && userInput.secondaryAction == ButtonState.Down)
            {
                if (timeSinceLastInput >= inputDelay)
                {
                    kill = true;
                    timeSinceLastInput = 0;
                }
            }
            timeSinceLastInput += Time.deltaTime;
            if (textRevealed)
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
            }
        }
    }
} 

   /* void SetTextImmediate()
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
} */
