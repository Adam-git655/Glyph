using UnityEngine;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private AnimationCurve fadeInCurve;
    
    private void Awake()
    {
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FadeOut(2f);
    }
    
    public void FadeIn(float duration = 1f)
    {
        if (canvasGroup.alpha >= 1) return;
        
        StartCoroutine(FadeInCoroutine(duration));
    }

    public void GoToOldCheckPoint(float duration = 1f)
    {
        StartCoroutine(GoToOldCheckPointCoroutine(duration));
    }
    
    private IEnumerator GoToOldCheckPointCoroutine(float duration = 1f)
    {
        yield return FadeInCoroutine(duration);
        SaveManager.Instance.Load();
        yield return FadeOutCoroutine(duration);
    }
    
    private IEnumerator FadeInCoroutine(float duration = 1f)
    {
        float t = 0f;
        Time.timeScale = 0f;
        
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            float value = fadeInCurve.Evaluate(t);
            canvasGroup.alpha = value;

            yield return null;
        }

        Time.timeScale = 1f;
    }

    public void FadeOut(float duration = 1f)
    {
        if (canvasGroup.alpha <= 0) return;
        
        StartCoroutine(FadeOutCoroutine(duration));
    }
    
    private IEnumerator FadeOutCoroutine(float duration = 1f)
    {
        yield return null;
        
        float t = 0f;
        Time.timeScale = 0f; 
        
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            float value = fadeOutCurve.Evaluate(t);
            canvasGroup.alpha = value;
            
            yield return null;
        }

        Time.timeScale = 1f;
    }
}
