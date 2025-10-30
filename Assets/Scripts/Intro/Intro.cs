using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [Header("Intro")]
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI thumbnailtext;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button continueButton;

    [Header("Controls")]
    [SerializeField] private Image wasdToMove;
    [SerializeField] private Image eToInteract;
    [SerializeField] private Image panMouseToPanCamera;
    [SerializeField] private Button startGameButton;

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float waitBetween = 0.5f;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        // Set everything transparent initially
        SetAlphaRecursive(thumbnail, 0);
        SetAlphaRecursive(thumbnailtext, 0);
        SetAlphaRecursive(instructionText, 0);
        SetAlphaRecursive(continueButton.gameObject, 0);
        SetAlphaRecursive(wasdToMove, 0);
        SetAlphaRecursive(eToInteract, 0);
        SetAlphaRecursive(panMouseToPanCamera, 0);
        SetAlphaRecursive(startGameButton.gameObject, 0);

        // Button listeners
        continueButton.onClick.AddListener(() => StartCoroutine(ContinueSequence()));
        startGameButton.onClick.AddListener(() => SceneManager.LoadScene(1));

        // Start intro sequence
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Fade in thumbnail
        yield return FadeRecursive(thumbnail.gameObject, 1, fadeDuration);

        yield return new WaitForSeconds(2);
        
        // Fade in thumbnail text
        yield return FadeRecursive(thumbnailtext.gameObject, 1, fadeDuration);

        yield return new WaitForSeconds(2);
        
        // Fade them out
        yield return FadeRecursive(thumbnail.gameObject, 0, fadeDuration);
        yield return FadeRecursive(thumbnailtext.gameObject, 0, fadeDuration);

        // Fade in instruction text
        yield return FadeRecursive(instructionText.gameObject, 1, fadeDuration);

        // Play audio
        if (_audioSource != null) _audioSource.Play();

        // Wait
        yield return new WaitForSeconds(waitBetween);

        // Fade in continue button
        yield return FadeRecursive(continueButton.gameObject, 1, fadeDuration);
        continueButton.onClick.AddListener(() => StartCoroutine(ContinueSequence()));
    }

    IEnumerator ContinueSequence()
    {
        continueButton.gameObject.SetActive(false);
        
        // Play audio
        if (_audioSource != null) _audioSource.Pause();
        
        yield return FadeRecursive(instructionText.gameObject, 0, fadeDuration);
        
        yield return FadeRecursive(continueButton.gameObject, 0, fadeDuration);
        
        // Fade in WASD
        yield return FadeRecursive(wasdToMove.gameObject, 1, fadeDuration);

        // Fade in E
        yield return FadeRecursive(eToInteract.gameObject, 1, fadeDuration);

        // Fade in Mouse
        yield return FadeRecursive(panMouseToPanCamera.gameObject, 1, fadeDuration);

        // Wait
        yield return new WaitForSeconds(waitBetween);

        // Fade in Start Game Button
        startGameButton.interactable = true;
        yield return FadeRecursive(startGameButton.gameObject, 1, fadeDuration);
        startGameButton.onClick.AddListener(() => SceneManager.LoadScene(1));
    }

    // --- Helpers ---

    void SetAlphaRecursive(GameObject obj, float alpha)
    {
        foreach (var g in obj.GetComponentsInChildren<Graphic>(true))
        {
            Color c = g.color;
            c.a = alpha;
            g.color = c;
        }
        foreach (var t in obj.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }
    }

    void SetAlphaRecursive(Graphic g, float alpha)
    {
        if (g != null) SetAlphaRecursive(g.gameObject, alpha);
    }

    IEnumerator FadeRecursive(GameObject obj, float targetAlpha, float duration)
    {
        Sequence s = DOTween.Sequence();

        foreach (var g in obj.GetComponentsInChildren<Graphic>(true))
        {
            s.Join(g.DOFade(targetAlpha, duration));
        }
        foreach (var t in obj.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            s.Join(t.DOFade(targetAlpha, duration));
        }

        yield return s.WaitForCompletion();
    }
}