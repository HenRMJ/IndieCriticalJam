using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text potentialScoreText;

    [Header("Animation Values")]
    [SerializeField] private AnimationCurve imageTransitionCurve;
    [SerializeField] private float imageTransitionTime;
    [Space, SerializeField] private AnimationCurve scoreAnimationCurve;
    [Space, SerializeField] private AnimationCurve scaleAnimationCurve;
    [SerializeField] private float scoreScale;

    private Coroutine _potentialScoreCoroutine;
    private Coroutine _imageCoroutine;
    private Coroutine _scoreCoroutine;
    private Image _image;

    private void Start()
    {
        _image = imageTransform.GetComponent<Image>();
    }    

    private IEnumerator AnimateTransform(TransformValues transformValues)
    {
        float timer = 0;
        Vector2 startPosition = imageTransform.anchoredPosition;
        float startScale = imageTransform.localScale.x;
        float startRotation = imageTransform.eulerAngles.z;

        while (timer <= imageTransitionTime)
        {
            timer += Time.deltaTime;

            float normalizedTime = timer / imageTransitionTime;
            float curveEvaluation = imageTransitionCurve.Evaluate(normalizedTime);
            
            float scaleLerp = Mathf.LerpUnclamped(
                startScale, 
                transformValues.Scale, 
                curveEvaluation);
            float rotationLerp = MathExtension.LerpAngleUnclamped(
                startRotation, 
                transformValues.Rotation, 
                curveEvaluation);

            imageTransform.anchoredPosition = Vector2.LerpUnclamped(
                startPosition, 
                transformValues.Position, 
                curveEvaluation);
            imageTransform.localScale = Vector3.one * scaleLerp;
            imageTransform.eulerAngles = new(0, 0, rotationLerp);

            yield return null;
        }

        imageTransform.anchoredPosition = transformValues.Position;
        imageTransform.localScale = Vector3.one * transformValues.Scale;
        imageTransform.eulerAngles = new(0, 0,transformValues.Rotation);

        _imageCoroutine = null;
    }

    private IEnumerator AnimateScore(TMP_Text textToChange, float newScore)
    {
        float timer = 0;
        int startingScore = int.Parse(textToChange.text);

        while (timer < imageTransitionTime)
        {
            timer += Time.deltaTime;

            float normalizedTime = timer / imageTransitionTime;
            float curveEvaluation = scoreAnimationCurve.Evaluate(normalizedTime);
            float scaleCurveEvaluation = scaleAnimationCurve.Evaluate(normalizedTime);

            float curvedScore = Mathf.FloorToInt(Mathf.LerpUnclamped(
                startingScore, 
                newScore, 
                curveEvaluation));

            textToChange.transform.localScale = Vector3.LerpUnclamped(
                Vector3.one, 
                Vector3.one * scoreScale, 
                scaleCurveEvaluation);
            textToChange.text = curvedScore.ToString();

            yield return null;
        }

        textToChange.text = newScore.ToString();

        if (textToChange == scoreText)
        {
            _scoreCoroutine = null;
        }
        else
        {
            _potentialScoreCoroutine = null;
        }
    }

    private void UpdatePotentialScore(float newScore, bool withTransition = true)
    {
        if (withTransition)
        {
            if (_potentialScoreCoroutine != null)
            {
                StopCoroutine(_potentialScoreCoroutine);
            }
            _potentialScoreCoroutine = StartCoroutine(AnimateScore(potentialScoreText, newScore));
        }
        else
        {
            potentialScoreText.text = newScore.ToString();
        }
    }

    public void UpdateImageTransform(TransformValues imageTransformValues, bool withTransition = true)
    {
        if (withTransition)
        {
            if (_imageCoroutine != null)
            {
                StopCoroutine(_imageCoroutine);
            }
            _imageCoroutine = StartCoroutine(AnimateTransform(imageTransformValues));
        }
        else
        {
            if (_imageCoroutine != null)
            {
                StopCoroutine(_imageCoroutine);
            }

            imageTransform.anchoredPosition = imageTransformValues.Position;
            imageTransform.localScale = Vector3.one * imageTransformValues.Scale;
            imageTransform.eulerAngles = new(0, 0, imageTransformValues.Rotation);
        }

        UpdatePotentialScore(imageTransformValues.Score, withTransition);
    }

    public void UpdateScore(float newScore)
    {
        if (_scoreCoroutine != null)
        {
            StopCoroutine(_scoreCoroutine);
        }
        _scoreCoroutine = StartCoroutine(AnimateScore(scoreText, newScore));
    }

    public void UpdateImageSprite(Sprite newSprite)
    {
        _image.sprite = newSprite;
    }
}

public static class MathExtension
{
    public static float LerpAngleUnclamped(float a, float b, float t)
    {
        float num = Mathf.Repeat(b - a, 360f);

        if (num > 180f)
        {
            num -= 360f;
        }

        return a + num * t;
    }
}