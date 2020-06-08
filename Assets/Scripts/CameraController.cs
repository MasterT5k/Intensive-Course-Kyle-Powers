using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private Vector3 _startPos = Vector3.zero;
    [SerializeField]
    private float _xLimitMax = 0f;
    [SerializeField]
    private float _xLimitMin = 0f;
    private float _yPos;
    [SerializeField]
    private float _zLimitMax = 0f;
    [SerializeField]
    private float _zLimitMin = 0f;
    private Camera _myCamera;
    [SerializeField]
    private float _zoomOutLimit = 0f;
    [SerializeField]
    private float _zoomInLimit = 0f;
    private float _screenWidth;
    private float _screenHeight;
    // Start is called before the first frame update
    void Start()
    {
        if (_startPos == Vector3.zero)
        {
            _startPos = transform.position;
        }
        transform.position = _startPos;
        _yPos = transform.position.y;
        _myCamera = GetComponent<Camera>();
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollInput = Input.mouseScrollDelta.y;

        if (scrollInput != 0)
        {
            _myCamera.fieldOfView += -scrollInput;
            _myCamera.fieldOfView = Mathf.Clamp(_myCamera.fieldOfView, _zoomInLimit, _zoomOutLimit);
        }

        CameraMovement();
    }

    void CameraMovement()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        if (hInput != 0 || vInput != 0)
        {
            Vector3 direction = new Vector3(hInput, 0, vInput * 2);
            Vector3 velocity = direction * _speed * Time.deltaTime;
            velocity = transform.TransformDirection(velocity);

            Vector3 newPos = transform.position + velocity;
            newPos.y = _yPos;
            newPos.x = Mathf.Clamp(newPos.x, _xLimitMin, _xLimitMax);
            newPos.z = Mathf.Clamp(newPos.z, _zLimitMin, _zLimitMax);

            transform.position = newPos;
        }
    }
}
