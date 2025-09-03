using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EndscreenManager : MonoBehaviour
{
    public static EndscreenManager Instance;
    [SerializeField] private AudioClip clip;
    [SerializeField] private GameObject endscreenCanvas;
    [SerializeField] private GameObject endscreenPrefab;

    [Header("Tween Settings")]
    [SerializeField] private float overshootScale = 2.5f;

    [Tooltip("How long it takes to slam from oversized down to near 1x scale")]
    [SerializeField] private float slamDuration = 0.7f;

    [Tooltip("How long the shake impact lasts")]
    [SerializeField] private float shakeDuration = 0.3f;

    [Tooltip("Strength of the shake on impact")]
    [SerializeField] private float shakeStrength = 40f;

    [Tooltip("How much time the endscreen takes to settle into perfect scale")]
    [SerializeField] private float settleDuration = 0.15f;

    private void Start()
    {
        Instance = this;
    }

    public void DisplayNewEndscreen(Sprite endscreenSprite)
    {
        endscreenCanvas.SetActive(true);

        GameObject preview = Instantiate(endscreenPrefab, endscreenCanvas.transform);
        RectTransform rect = preview.GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogError("Prefab missing RectTransform");
            return;
        }

        // Apply sprite
        Image img = preview.GetComponentInChildren<Image>();
        if (img != null && endscreenSprite != null)
            img.sprite = endscreenSprite;

        // Start oversized (like it's dropping in)
        rect.localScale = Vector3.one * overshootScale;

        Sequence s = DOTween.Sequence();

        // Slam down
        s.Append(rect.DOScale(1f, slamDuration).SetEase(Ease.InQuad));

        // Shake impact
        s.Append(rect.DOShakeAnchorPos(
            duration: shakeDuration,
            strength: shakeStrength,
            vibrato: 50,
            randomness: 90f,
            snapping: false,
            fadeOut: true
        ));

        // Settle
        s.Append(rect.DOScale(1f, settleDuration).SetEase(Ease.OutSine));

        AudioManager.Instance.PlaySfxClip(clip);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().SetBool("IsDead", true);
        s.Play();
        
        // Add listener
        img.transform.GetComponentInChildren<Button>().onClick.AddListener(Reset);
    }

    private void Reset()
    {
        SceneManager.LoadScene(1);
    }
}