using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithCamera : MonoBehaviour
{
    [Tooltip("The camera to face towards. Defaults to the main camera.")]
    [SerializeField] private Camera cam;

    private void Start()
    {
        if (!cam)
            cam = Camera.main;
    }

    private void Update()
    {
        transform.forward = cam.transform.forward;
    }
}