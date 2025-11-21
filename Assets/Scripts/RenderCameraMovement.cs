using System;
using UnityEngine;

public class RenderCameraMovement : MonoBehaviour
{
    private Transform _camera;

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        transform.position = _camera.position;
        transform.rotation = _camera.rotation;
    }
}
