using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class WorldCanvas : MonoBehaviour
{
    private float _tweenTime;
    private Ease _tweenEase;
    [SerializeField] private float _scale = 0.002f;
    
    private void OnEnable()
    {
        _tweenTime = GameObject.Find("DialogueManager").GetComponent<DialogueManager>()._tweenTime;
        _tweenEase = GameObject.Find("DialogueManager").GetComponent<DialogueManager>()._tweenEase;

        transform.DOScale(_scale, _tweenTime).SetEase(_tweenEase).Play();
    }

    public void Hide()
    {
        transform.DOScale(0, _tweenTime)
            .SetEase(_tweenEase)
            .OnComplete(() => gameObject.SetActive(false))
            .Play();
    }
}