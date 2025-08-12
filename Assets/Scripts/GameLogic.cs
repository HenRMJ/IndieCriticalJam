using System;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private ImagePromptSO[] levels;

    private AnswerValidator _validator;
    private GameUI _ui;

    private int _currentLevel = 0;
    private int _currentSubLevel = 0;
    private int _potentialScore;
    private int _currentScore;

    private void Start()
    {
        _validator = GetComponent<AnswerValidator>();
        _ui = GetComponent<GameUI>();
        
        _validator.OnIncorrectAnswer += Validator_OnIncorrectAnswer;
        _validator.OnCorrectAnswer += Validator_OnCorrectAnswer;
        _validator.OnCloseAnswer += Validator_OnCloseAnswer;

        _validator.SetCurrentPrompt(levels[_currentLevel]);

        _potentialScore = levels[_currentLevel].SubLevel[_currentSubLevel].Score;

        _ui.UpdateImageTransform(levels[_currentLevel].SubLevel[_currentSubLevel], false);
    }

    private void OnDestroy()
    {
        if (_validator != null)
        {
            _validator.OnIncorrectAnswer -= Validator_OnIncorrectAnswer;
            _validator.OnCorrectAnswer -= Validator_OnCorrectAnswer;
            _validator.OnCloseAnswer -= Validator_OnCloseAnswer;
        }
    }

    private void NextSubLevel()
    {
        _currentSubLevel++;

        if (_currentLevel >= levels.Length || 
            _currentSubLevel >= levels[_currentLevel].SubLevel.Length)
        {
            NextLevel();
            return;
        }

        _potentialScore = levels[_currentLevel].SubLevel[_currentSubLevel].Score;

        _ui.UpdateImageTransform(levels[_currentLevel].SubLevel[_currentSubLevel]);
    }

    private void NextLevel()
    {
        _currentLevel++;

        if (_currentLevel >= levels.Length)
        {
            Debug.Log("All levels completed!");
            return;
        }

        _currentSubLevel = 0;
        _potentialScore = levels[_currentLevel].SubLevel[_currentSubLevel].Score;

        _validator.SetCurrentPrompt(levels[_currentLevel]);

        _ui.UpdateImageTransform(levels[_currentLevel].SubLevel[_currentSubLevel], false);
        _ui.UpdateImageSprite(levels[_currentLevel].PromptImage);
    }

    private void Validator_OnCloseAnswer(object sender, string userAnswer)
    {
        NextSubLevel();
    }

    private void Validator_OnCorrectAnswer(object sender, string userAnswer)
    {
        if (_currentLevel < levels.Length)
        {
            _currentScore += _potentialScore;
            _ui.UpdateScore(_currentScore);
        }

        NextLevel();
    }

    private void Validator_OnIncorrectAnswer(object sender, string userAnswer)
    {
        NextSubLevel();
    }
}
