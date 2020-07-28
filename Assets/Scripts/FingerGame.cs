using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerGame : MonoBehaviour {

    [Header("Game Properties")]
    [SerializeField] int gameRounds = 5;
    [SerializeField] int fingerCounts = 1;

    [Header("Game UI")]
    [SerializeField] Text RoundText;
    [SerializeField] Text TakeTimesText;
    [SerializeField] Text UnitTimeText;
    [SerializeField] Transform GameArea;

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
    }

    void Start()
    {
        InitGame();
    }

    void Update ()
    {
        TakeTimesText.text = scores.ToString("F3") + "s";
        unitScore += Time.deltaTime;

        if (restRound != 0)
        {
            if (roundClear)
            {
                // Round start
                InitRound();
                // Generate target
                StartCoroutine("TestForClearTarget");
            }

            scores += Time.deltaTime;
        }
    }

    IEnumerator TestForClearTarget()
    {
        while (restFingerCount != 0)
        {
            restFingerCount--;
            float waitTime = Random.Range(0.01f, 0.5f);
            Debug.Log(waitTime);
            yield return new WaitForSeconds(waitTime);
        }

        UnitTimeText.text = unitScore.ToString("F3") + "s";
        Debug.Log(UnitTimeText.text);
        roundClear = true;
        unitScore = 0;
    }
}
