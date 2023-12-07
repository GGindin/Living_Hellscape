using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditController : MonoBehaviour
{
    [SerializeField]
    float speed = 20f;

    [SerializeField]
    float lingerTime = 5f;

    [SerializeField]
    RectTransform topTarget, botTarget;

    RectTransform thisTrans;

    float halfHeight;

    bool wantsToSkip;
    bool isSkip;

    private void Awake()
    {
        thisTrans = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetHalfHeight();
        PositionAtBottom();
        StartCreditsRoll();
    }

    void GetHalfHeight()
    {
        var corns = new Vector3[4];
        thisTrans.GetWorldCorners(corns);

        halfHeight = Mathf.Abs(corns[0].y - corns[1].y) / 2f;
    }

    void PositionAtBottom()
    {
        thisTrans.position = botTarget.position + new Vector3(0, -halfHeight);
    }

    void StartCreditsRoll()
    {
        StartCoroutine(ProcessCredits());
    }

    IEnumerator ProcessCredits()
    {
        Vector3 endPos = topTarget.position + new Vector3(0f, halfHeight);

        float lingerTimer = lingerTime;

        while (true)
        {
            thisTrans.position = Vector3.MoveTowards(thisTrans.position, endPos, Time.deltaTime * speed);

            if(Vector3.Distance(endPos, thisTrans.position) < .01f)
            {
                break;
            }

            yield return null;
        }

        while (true)
        {
            lingerTimer -= Time.deltaTime;

            if (lingerTimer < 0)
            {
                break;
            }
            yield return null;
        }

        if (SceneController.Instance && !isSkip)
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
                    isSkip = true;
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
