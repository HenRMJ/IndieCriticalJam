using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ImagePrompt", menuName = "ScriptableObjects/ImagePromptSO", order = 1)]
public class ImagePromptSO : ScriptableObject
{
    [Header("Image Prompt")]
    public Sprite PromptImage;

    [Header("Transform Values")]
    public TransformValues[] SubLevel;

    [Header("Answers")]
    public string[] CorrectAnswers;
    public string[] CloseAnswers;
}

[Serializable]
public class TransformValues
{
    public Vector2 Position;
    public float Rotation;
    public float Scale;
    public int Score;

    public TransformValues(Vector2 Position, float Rotation, float Scale)
    {
        this.Position = Position;
        this.Rotation = Rotation;
        this.Scale = Scale;
        Score = 0;
    }
}