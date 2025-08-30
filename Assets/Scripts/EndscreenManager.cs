using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndscreenManager : MonoBehaviour
{
    [SerializeField] private GameObject endscreenCanvas;
    [SerializeField] private GameObject endscreenPrefab;
    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = 0.8f;
    [SerializeField] private float fadeDuration = 0.45f;

    public void DisplayNewEndscreen(Sprite endscreenSprite)
    {
        endscreenCanvas.SetActive(true);

        // Instantiate a preview endscreen (we'll animate this in)
        GameObject preview = Instantiate(endscreenPrefab, endscreenCanvas.transform);
        RectTransform rect = preview.GetComponent<RectTransform>();
        if (rect == null) { Debug.LogError("Prefab missing RectTransform"); return; }

        // Ensure there's a CanvasGroup to fade the whole thing
        CanvasGroup cg = preview.GetComponent<CanvasGroup>();
        if (cg == null) cg = preview.AddComponent<CanvasGroup>();
        cg.alpha = 0f; // start invisible

        // if you want to set the sprite immediately (or at reveal), try:
        Image img = preview.GetComponentInChildren<Image>();
        if (img != null && endscreenSprite != null) img.sprite = endscreenSprite;

        // Start tiny
        rect.localScale = Vector3.zero;

        // Sequence: pop -> shake -> settle, fade with CanvasGroup
        Sequence s = DOTween.Sequence();
        s.Append(rect.DOScale(1.1f, tweenDuration * 0.8f).SetEase(Ease.OutBack));
        s.Join(cg.DOFade(1f, fadeDuration).SetEase(Ease.OutSine));
        s.Append(rect.DOShakeAnchorPos(0.25f, 30f, vibrato: 30, randomness: 90f));
        s.Append(rect.DOScale(1f, tweenDuration * 0.2f).SetEase(Ease.OutSine));
        s.Play();
    }
}