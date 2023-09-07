using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool _enable = true;
    [SerializeField, Range(0, 0.01f)] private float _Amplitude; [SerializeField, Range(0, 30)] private float _frequency;
    [SerializeField] private Transform _camera = null; [SerializeField] private Transform _cameraHolder = null;

    private float _toggleSpeed = 0.1f;
    private Vector3 _startPos;

    private Rigidbody _controller;
    private GameObject GroundCheckGO;
    private GroundCheck GroundCheckScript;

    private float varSpeed = 0f;

    private void Awake()
    {//d
        _controller = GetComponent<Rigidbody>();
        _startPos = _camera.localPosition;

        GroundCheckGO = GameObject.FindObjectOfType<GroundCheck>().gameObject;
        GroundCheckScript = GroundCheckGO.GetComponent<GroundCheck>();
    }

    void FixedUpdate()
    {
        if (!_enable) return;
        CheckMotion();
        ResetPosition();
    }
    private Vector3 FootStepMotion(float speed)
    {
        float auxFrequency = _frequency;
        if(speed > 6){
            auxFrequency *= 1.3f;
        }
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * auxFrequency) * _Amplitude * varSpeed;
        pos.x += Mathf.Cos(Time.time * auxFrequency / 2) * _Amplitude * 2 * varSpeed;
        return pos;
    }
    private void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        varSpeed = speed/5;
        if (speed < _toggleSpeed) return;
        if (!GroundCheckScript.isGrounded) return;

        PlayMotion(FootStepMotion(speed));
    }
    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * 15.0f;
        return pos;
    }
    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }
}