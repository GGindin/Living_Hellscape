using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditController : MonoBehaviour
{
    [SerializeField]
    float speed = 20f;

    [SerializeField]
    TextMeshProUGUI textBox;

    [SerializeField]
    TextAsset text;

    [SerializeField]
    RectTransform topTarget, botTarget;

    RectTransform thisTrans;

    float halfHeight;

    bool wantsToSkip;

    private void Awake()
    {
        thisTrans = GetComponent<RectTransform>();

        SetText();

    }

    // Start is called before the first frame update
    void Start()
    {
        StartCreditsRoll();
    }

    void SetText()
    {
        textBox.text = text.text;
    }

    void GetHalfHeight()
    {
        halfHeight = thisTrans.rect.height / 2f;
    }

    void PositionAtBottom()
    {
        thisTrans.position = new Vector3(0, -halfHeight);
    }

    void StartCreditsRoll()
    {
        StartCoroutine(ProcessCredits());
    }

    IEnumerator ProcessCredits()
    {
        yield return null;

        GetHalfHeight();
        PositionAtBottom();

        Vector3 endPos = new Vector3(0f, halfHeight);


        while (true)
        {
            thisTrans.position = Vector3.MoveTowards(thisTrans.position, endPos, Time.deltaTime * speed);

            if(Vector3.Distance(endPos, thisTrans.position) < .01f)
            {
                break;
            }

            yield return null;
        }

        if (SceneController.Instance)
        {
            SceneController.Instance.LoadMainMenuScene();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!VignetteController.Instance.isActiveAndEnabled)
        {
            var input = InputController.GetUserInput();

            if(wantsToSkip && input.mainAction == ButtonState.Down)
            {
                if (SceneController.Instance)
                {
                    SceneController.Instance.LoadMainMenuScene();
                }
            }

            if(input.mainAction == ButtonState.Down)
            {
                wantsToSkip = true;
            }
        }
    }
}
