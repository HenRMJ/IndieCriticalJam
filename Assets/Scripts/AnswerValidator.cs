using TMPro;
using System;
using UnityEngine;
using System.Text.RegularExpressions;

public class AnswerValidator : MonoBehaviour
{
    public event EventHandler<string> OnCorrectAnswer;
    public event EventHandler<string> OnCloseAnswer;
    public event EventHandler<string> OnIncorrectAnswer;

    [SerializeField] private TMP_InputField answerField;

    private string[] _correctAnswers;
    private string[] _closeAnswers;

    private void Start()
    {
        answerField.onSubmit.AddListener(ProcessAnswer);
    }

    private void OnDestroy()
    {
        answerField.onSubmit.RemoveListener(ProcessAnswer);
    }

    private void ProcessAnswer(string answer)
    {
        if (_closeAnswers.Length == 0 || _correctAnswers.Length == 0)
        {
            Debug.LogWarning("No current prompt set.");
            return;
        }

        answerField.SetTextWithoutNotify(string.Empty);
        answerField.Select();
        answerField.ActivateInputField();

        if (ValidateAnswer(answer))
        {
            OnCorrectAnswer?.Invoke(this, answer);
            return;
        }

        if (ValidateAnswer(answer, true))
        {
            OnCloseAnswer?.Invoke(this, answer);
            return;
        }

        OnIncorrectAnswer?.Invoke(this, answer);
    }

    private bool ValidateAnswer(string answer, bool checkClose = false)
    {
        string[] answersToCheck = checkClose ? _closeAnswers : _correctAnswers;
        string normalizedAnswer = Normalize(answer);

        for (int i = 0; i < answersToCheck.Length; i++)
        {
            string normalizedCheckAnswer = Normalize(answersToCheck[i]);

            if (normalizedAnswer == normalizedCheckAnswer) return true;
            if (IsCloseMatch(normalizedAnswer, normalizedCheckAnswer)) return true;
        }

        return false;
    }

    private string Normalize(string input)
    {
        string lower = input.ToLowerInvariant();

        lower = Regex.Replace(lower, @"[^\w\s]", ""); // Remove punctuation
        lower = Regex.Replace(lower, @"\s+", " "); // Replace multiple spaces with a single space

        return lower;
    }

    private bool IsCloseMatch(string answer, string checkAnswer)
    {
        int distance = LevenshteinDistance(answer, checkAnswer);
        return distance <= Mathf.Max(2, checkAnswer.Length / 5);
    }

    private int LevenshteinDistance(string mainString, string compareString)
    {
        int[,] distance = new int[mainString.Length + 1, compareString.Length + 1];

        for (int i = 0; i <= mainString.Length; i++) distance[i, 0] = i;
        for (int j = 0; j <= compareString.Length; j++) distance[0, j] = j;

        for (int i = 1; i <= mainString.Length; i++)
        {
            for (int j = 1; j <= compareString.Length; j++)
            {
                int cost = mainString[i - 1] == compareString[j - 1] ? 0 : 1;

                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        return distance[mainString.Length, compareString.Length];
    }

    public void SubmitAnswer()
    {
        ProcessAnswer(answerField.text);
    }

    public void SetCurrentPrompt(ImagePromptSO prompt)
    {
        _correctAnswers = prompt.CorrectAnswers;
        _closeAnswers = prompt.CloseAnswers;
    }
}
