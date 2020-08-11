using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FingerGame : MonoBehaviour {

    [Header("Game Properties")]
    [SerializeField] int gameRounds = 5;
    [SerializeField] int fingerCounts = 1;

    [Header("Game UI")]
    [SerializeField] Text RoundText;
    [SerializeField] Text TakeTimesText;
    [SerializeField] Text UnitTimeText;
    [SerializeField] RectTransform GameArea;
    [SerializeField] RectTransform Target;
    [SerializeField] ParticleSystem BreakEffact;

    double scores = 0;
    double unitScore = 0;

    int restRound;
    int restFingerCount;

    bool roundClear = true;

    public void InitGame(int rounds, int counts)
    {
        gameRounds = rounds;
        fingerCounts = counts;

        InitGame();
    }

    void InitGame()
    {
        restRound = gameRounds;
        restFingerCount = fingerCounts;
        scores = 0;
        unitScore = 0;
    }

    void Reset()
    {
        gameRounds = 5;
        fingerCounts = 1;
    }

    void InitRound()
    {
        roundClear = false;
        restRound--;
        restFingerCount = fingerCounts;
        RoundText.text = "Round " + (gameRounds - restRound).ToString();
        GenerateTarget();
    }

    Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(
            GameArea.rect.x + Target.rect.width / 2,
            GameArea.rect.x + GameArea.rect.width - Target.rect.width / 2);
        float y = Random.Range(
            GameArea.rect.y + Target.rect.height / 2,
            GameArea.rect.y + GameArea.rect.height - Target.rect.height / 2);

        return new Vector3(x, y, 0f);
    }

    bool IsTargetOverlapping(Collider2D col2d, List<Collider2D> col2dList)
    {
        if (col2dList.Count > 0)
        {
            Collider2D[] targets = col2dList.ToArray();
            int count = col2d.OverlapCollider(new ContactFilter2D(), targets);
            if (count > 0)
            {
                return true;
            }
        }

        return false;
    }

    void GenerateTarget()
    {
        var targetList = new List<Collider2D>();

        for (int i = 0; i < fingerCounts; i++)
        {
            GameObject target = Instantiate(Target.gameObject, GameArea);
            target.transform.localPosition = GetRandomPositionInArea();
            int limit = 0;

            while (IsTargetOverlapping(target.GetComponent<Collider2D>(), targetList) && limit < 1000)
            {
                limit++;
                target.transform.localPosition = GetRandomPositionInArea();
            }

            targetList.Add(target.GetComponent<Collider2D>());

            // Touch event
            EventTrigger eventTrigger = target.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((x) =>
            {
                TargetTouchEvent(target);
            });
            eventTrigger.triggers.Add(entry);
        }
    }

    void TargetTouchEvent(GameObject target)
    {
        Destroy(target);
        restFingerCount--;
        BreakEffact.gameObject.transform.position = target.transform.position;
        BreakEffact.Play();

        if (restFingerCount == 0)
        {
            UnitTimeText.text = unitScore.ToString("F3") + "s";
            roundClear = true;
            unitScore = 0;
        }
    }

    void Start()
    {
        InitGame();
    }

    void Update ()
    {
        unitScore += Time.deltaTime;
        if (restRound != 0 || !roundClear)
        {
            scores += Time.deltaTime;
            if (roundClear)
            {
                InitRound();
            }
        }
        TakeTimesText.text = scores.ToString("F3") + "s";
    }
}
