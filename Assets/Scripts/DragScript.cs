using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DragScript : MonoBehaviour
{
    public InputSystem_Actions controls;

    [Header("Drag Settings")]
    public float dragForce = 10f;

    [SerializeField] private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Drag.performed += ctx => StartDrag();
        controls.Player.Drag.canceled += ctx => EndDrag();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (isDragging)
        {
            // When the mouse cursor overlaaps the circle collider, update the offset.
            if (circleCollider != null && circleCollider.OverlapPoint(GetMouseWorldPosition()))
            {
                offset = (Vector3)rb.position - (Vector3)GetMouseWorldPosition();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDragging && rb != null)
        {
            Vector2 targetPos = GetMouseWorldPosition() + (Vector2)offset;
            Vector2 force = (targetPos - rb.position) * dragForce;
            rb.linearVelocity = force;
        }
    }

    private void StartDrag()
    {
        if (circleCollider != null && circleCollider.OverlapPoint(GetMouseWorldPosition()))
        {
            //rb.simulated = true; // Enable physics simulation when dragging
            isDragging = true;
            offset = (Vector3)rb.position - (Vector3)GetMouseWorldPosition();
        }
    }

    private void EndDrag()
    {
        //rb.simulated = false; // Disable physics simulation when not dragging
        isDragging = false;
        rb.linearVelocity = Vector3.zero; // Stop movement when released
        
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        return new Vector2(mouseWorldPos.x, mouseWorldPos.y);
    }
}


