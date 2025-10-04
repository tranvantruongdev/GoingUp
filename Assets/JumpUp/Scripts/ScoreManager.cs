using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; set; }
    public Text _currentScoreText, _highScoreText, _currentScoreGameOverText, _highScoreGameOverText;

    public float CurrentScoreCounter, HighScoreCounter;
    // Start is called before the first frame update

    bool _isCounting;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //init and load highscore
    void Start()
    {
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetFloat("HighScore", 0);

        HighScoreCounter = PlayerPrefs.GetFloat("HighScore");

        UpdateTheHighScore();
        ResetTheCurrentScoreValue();
    }

    //save and update highscore
    void UpdateTheHighScore()
    {
        if (CurrentScoreCounter > HighScoreCounter)
            HighScoreCounter = CurrentScoreCounter;

        _highScoreText.text = HighScoreCounter.ToString("F1");
        PlayerPrefs.SetFloat("HighScore", HighScoreCounter);
    }

    //update currentscore
    public void UpdateScoreValue(int value)
    {
        CurrentScoreCounter += value;
        RoundInputValueWithDigits(CurrentScoreCounter, 1);
        _currentScoreText.text = CurrentScoreCounter.ToString("F1");
    }

    //reset current score
    public void ResetTheCurrentScoreValue()
    {
        CurrentScoreCounter = 0;
        UpdateScoreValue(0);
    }

    //update gameover scores
    public void UpdateScoreGameover()
    {
        UpdateTheHighScore();

        _currentScoreGameOverText.text = CurrentScoreCounter.ToString("F1");
        _highScoreGameOverText.text = HighScoreCounter.ToString("F1");
    }

    public void StartCountingCouroutine()
    {
        _isCounting = true;
        StartCoroutine(Countering());
    }

    public void StopCountingCouroutine()
    {
        _isCounting = false;
        StopCoroutine(Countering());
    }

    IEnumerator Countering()
    {
        while (_isCounting)
        {
            CurrentScoreCounter += .1f;
            RoundInputValueWithDigits(CurrentScoreCounter, 1);
            _currentScoreText.text = CurrentScoreCounter.ToString("F1");
            yield return new WaitForSeconds(.1f);
        }
    }

    //round on 1 decimal, because sometimes float get more than one decimal
    public float RoundInputValueWithDigits(float input, int digits)
    {
        float result = Mathf.Pow(10.0f, digits);
        return Mathf.Round(input * result) / result;
    }
}
