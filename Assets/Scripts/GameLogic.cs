using System;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;

    private AnswerValidator _validator;
    private AudioManager _audio;
    private GameUI _ui;

    private int _currentLevel = 0;
    private int _currentSubLevel = 0;
    private int _potentialScore;
    private int _currentScore;

    private void Start()
    {
        _validator = GetComponent<AnswerValidator>();
        _audio = GetComponent<AudioManager>();
        _ui = GetComponent<GameUI>();
        
        _validator.OnIncorrectAnswer += Validator_OnIncorrectAnswer;
        _validator.OnCorrectAnswer += Validator_OnCorrectAnswer;
        _validator.OnCloseAnswer += Validator_OnCloseAnswer;
        _validator.OnType += Validator_OnType;

        _validator.SetCurrentPrompt(levels[_currentLevel]);

        _potentialScore = levels[_currentLevel].SubLevel[_currentSubLevel].Score;

        _ui.UpdateImageTransform(levels[_currentLevel].SubLevel[_currentSubLevel], false);
        _ui.UpdateColor(levels[_currentLevel].PrimaryColor, levels[_currentLevel].SecondaryColor, false);
    }    

    private void OnDestroy()
    {
        if (_validator != null)
        {
            _validator.OnIncorrectAnswer -= Validator_OnIncorrectAnswer;
            _validator.OnCorrectAnswer -= Validator_OnCorrectAnswer;
            _validator.OnCloseAnswer -= Validator_OnCloseAnswer;
            _validator.OnType -= Validator_OnType;
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

        LevelSO currentLevel = levels[_currentLevel];

        _potentialScore = currentLevel.SubLevel[_currentSubLevel].Score;

        _validator.SetCurrentPrompt(currentLevel);

        _ui.UpdateImageTransform(currentLevel.SubLevel[_currentSubLevel], false);
        _ui.UpdateImageSprite(currentLevel.PromptImage);
        _ui.UpdateColor(currentLevel.PrimaryColor, currentLevel.SecondaryColor);
    }

    private void Validator_OnCorrectAnswer(object sender, string userAnswer)
    {
        if (_currentLevel < levels.Length && _potentialScore > 0)
        {
            _currentScore += _potentialScore;
            _ui.UpdateScore(_currentScore);
            _audio.PlaySound(SoundType.TextRise);
        }

        NextLevel();
    }

    private void Validator_OnCloseAnswer(object sender, string userAnswer)
    {
        _audio.PlaySound(SoundType.TextFall);
        NextSubLevel();
    }

    private void Validator_OnIncorrectAnswer(object sender, string userAnswer)
    {
        _audio.PlaySound(SoundType.TextFall);
        NextSubLevel();
    }

    private void Validator_OnType(object sender, EventArgs e)
    {
        _audio.PlaySound(SoundType.Type);
    }
}
