using System;
using UnityEngine;

public class Cameras : MonoBehaviour {
    public static Cameras Instance { get; private set; }

    [SerializeField]
    private Camera mainCamera;

    private Camera currentCamera;

    public Camera MainCamera => mainCamera;

    public Camera CurrentCamera => currentCamera;
    
    private void Awake() {
        Instance = this;
        
        SetCurrentCamera(mainCamera);
    }
    
    public void SetCurrentCamera(Camera camera) {
        currentCamera = camera;
    }
    
    public void ResetCurrentCamera() {
        currentCamera = mainCamera;
    }
}