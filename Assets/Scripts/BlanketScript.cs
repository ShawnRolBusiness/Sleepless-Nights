using UnityEngine;
using UnityEngine.InputSystem;

public class BlanketScript : MonoBehaviour
{
    [SerializeField] private InputSystem_Actions controls;
    [SerializeField] private bool isDragging = false;
    private GameObject[] bones;
    private GameObject currentBone;
    public float dragForce = 2f;

    private Camera mainCamera;
    private Vector3 offset;

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Drag.performed += ctx => StartDrag();
        controls.Player.Drag.canceled += ctx => EndDrag();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        mainCamera = Camera.main;
        bones = GameObject.FindGameObjectsWithTag("Bones");
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDragging)
        {
            // When the mouse cursor overlaaps the circle collider, update the offset.
            if (currentBone != null && currentBone.GetComponent<CircleCollider2D>().OverlapPoint(GetMouseWorldPosition()))
            {
                offset = (Vector3)currentBone.GetComponent<Rigidbody2D>().position - (Vector3)GetMouseWorldPosition();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDragging && currentBone.GetComponent<Rigidbody2D>() != null)
        {
            Vector2 targetPos = GetMouseWorldPosition() + (Vector2)offset;
            Vector2 force = (targetPos - currentBone.GetComponent<Rigidbody2D>().position) * dragForce;
            currentBone.GetComponent<Rigidbody2D>().linearVelocity = force;
        } else if (!isDragging && currentBone != null &&currentBone.GetComponent<Rigidbody2D>().simulated)
        {
            currentBone.GetComponent<Rigidbody2D>().simulated = true;
        }
    }

    private void StartDrag()
    {
        if (bones != null)
        {
            foreach (GameObject bone in bones)
            {
                if (bone.GetComponent<CircleCollider2D>() != null && 
                    bone.GetComponent<CircleCollider2D>().OverlapPoint(GetMouseWorldPosition()))
                {
                    currentBone = bone;
                    isDragging = true;
                    Debug.Log(bone + " is being dragged");
                    break; // Only log once per frame
                }
            }
        }
    }

    private void EndDrag()
    {
        isDragging = false;
        currentBone.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        return new Vector2(mouseWorldPos.x, mouseWorldPos.y);
    }
}
