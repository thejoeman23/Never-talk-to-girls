using System;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject cinemachineCamera;
    private GameObject _player;
    private GameObject _camera;
    
    private void Start()
    {
        _camera = Camera.main.gameObject;
        _player = GameObject.FindWithTag("Player");
    }

    public void MoveCameraAnePlayer(Transform parent)
    {
        Transform playerTarget = null;
        Transform target = null;
        foreach (Transform child in parent)
        {
            if (child.CompareTag("playerpos"))
                playerTarget = child.gameObject.transform;
            if (child.CompareTag("camerapos"))
                target = child.gameObject.transform;
        }

        if (playerTarget == null || target == null)
        {
            Debug.LogWarning("You are missing a camera or player target");
            return;
        }
        
        cinemachineCamera.SetActive(false);
        
        _camera.transform.DOMove(target.position, 0.5f).Play();
        _camera.transform.DORotateQuaternion(target.rotation, .5f).Play();
        _player.transform.DOMove(playerTarget.position, .5f).Play();
        _player.GetComponent<PlayerMovement>().canMove = false;
    }

    public void ResetCamera()
    {
        cinemachineCamera.SetActive(true);
        _player.GetComponent<PlayerMovement>().canMove = true;
    }
}
