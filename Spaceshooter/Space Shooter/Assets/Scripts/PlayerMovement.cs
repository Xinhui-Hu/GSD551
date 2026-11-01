using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    //[Tooltip("Some texts explaining the function")]
    [SerializeField]
    private BoxCollider movementBounds;
    
    [SerializeField]
    private Transform playerVisual;
    
    [Header( "Movement")]
    [SerializeField]
    private float movementSpeed = 5f;
    
    
    [Space]
    
    [Header("Visual Rotation")]
    [SerializeField]
    private float maximumPitchAngle = 20f;
    
    [SerializeField]
    private float maximumYawAngle = 25f;
    
    [SerializeField]
    private float maximumBankAngle = 90f;

    [SerializeField]
    private float visualRotationSpeed = 8f;
    
    [Header("Barrel Roll")]

    [SerializeField]
    private float doubleTapWindow = 0.3f;

    [SerializeField]
    private float barrelRollDuration = 0.5f;
    
    [SerializeField]
    private float barrelRollMomentum = 10f;

    [SerializeField]
    private float barrelRollMomentRecovery = 20f;
    
    private float lastLeftBankPressTime = float.NegativeInfinity;
    private float lastRightBankPressTime = float.NegativeInfinity;
    
    private bool isBarrelRolling = false;
    private float barrelRollDirection;
    private float barrelRollElapsed;
    private float barrelRollAngle;
    private float currentbarrelRollMomentum;

    private Vector2 movementInput;
    private float horizontalInput;
    private float verticalInput;
    private float bankInput;
    
    private Quaternion currentFlightRotation;

    private void Start()
    {
        currentFlightRotation = playerVisual.localRotation;
    }


    // Update is called once per frame
    void Update()
    {
        ReadMovementInput();
        ReadBankInput();
        DetectBarrelRollInput();
        
        UpdateBarrelRoll();
        Move();
        RotatePlayerVisual();

        
    }

    private void UpdateBarrelRoll()
    {
        if (isBarrelRolling == false)
        {
            return;
        }
        
        barrelRollElapsed += Time.deltaTime;
        
        float rollProgress = Mathf.Clamp01(barrelRollElapsed / barrelRollDuration);
        
        //might be negative
        barrelRollAngle = -barrelRollDirection * 360f * rollProgress;

        if (rollProgress >= 1f)
        {
            isBarrelRolling = false;
            barrelRollElapsed = 0f;
            barrelRollAngle = 0f;
        }
    }

    private void DetectBarrelRollInput()
    {
        bool leftBankPressed = Keyboard.current.qKey.wasPressedThisFrame;
        bool rightBankPressed = Keyboard.current.eKey.wasPressedThisFrame;

        if (Gamepad.current != null)
        {
            leftBankPressed = leftBankPressed || Gamepad.current.leftShoulder.wasPressedThisFrame; 
            rightBankPressed = rightBankPressed || Gamepad.current.rightShoulder.wasPressedThisFrame; 
            
        }

        if (isBarrelRolling == true)
        {
            return;
        }

        if (leftBankPressed == true)
        {
            float time = Time.time - lastLeftBankPressTime;
            if (time <= doubleTapWindow)
            {
                //Debug.Log("Barrel Roll: Left");
                StartBarrelRoll(-1f);
                lastLeftBankPressTime = float.NegativeInfinity;
            }
            else
            {
                lastLeftBankPressTime = Time.time;
            }
        }
        else if (rightBankPressed == true)
        {
            float time = Time.time - lastRightBankPressTime;
            if (time <= doubleTapWindow)
            {
                //Debug.Log("Barrel Roll: Right");
                StartBarrelRoll(1f);
                lastRightBankPressTime = float.NegativeInfinity;
            }
            else
            {
                lastRightBankPressTime = Time.time;
            }
        }
    }

    private void StartBarrelRoll(float rollDirection)
    {
        isBarrelRolling = true;
        barrelRollDirection = rollDirection;

        barrelRollElapsed = 0f;
        barrelRollAngle = 0;
        
        currentbarrelRollMomentum = rollDirection * barrelRollMomentum;
    }

    public void ReadBankInput()
    {
        bankInput = Keyboard.current.eKey.ReadValue() -Keyboard.current.qKey.ReadValue();

        if (Gamepad.current != null)
        {
            bankInput += Gamepad.current.rightShoulder.ReadValue() - Gamepad.current.leftShoulder.ReadValue();
        }
        
        bankInput = Mathf.Clamp(bankInput, -1f, 1f);
    }

    private void RotatePlayerVisual()
    {
        float targetPitch = -movementInput.y * maximumPitchAngle;
        float targetYaw =  movementInput.x * maximumYawAngle;
        float targetBank = -bankInput * maximumBankAngle;
        
        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, targetBank);
        currentFlightRotation = Quaternion.Lerp(currentFlightRotation, targetRotation, visualRotationSpeed * Time.deltaTime);

        Quaternion barrelRollRotation = Quaternion.Euler(0, 0, barrelRollAngle);
        playerVisual.localRotation = currentFlightRotation* barrelRollRotation;

    }

    private void ReadMovementInput()
    {
        float horizontalInput = Keyboard.current.dKey.ReadValue() -Keyboard.current.aKey.ReadValue(); 
        float verticalInput = Keyboard.current.wKey.ReadValue() -Keyboard.current.sKey.ReadValue(); 
        if (Gamepad.current != null)
        {
            Vector2 leftStickInput = Gamepad.current.leftStick.ReadValue();
            horizontalInput += leftStickInput.x;
            verticalInput += leftStickInput.y;
        }
        movementInput = new Vector2(horizontalInput, verticalInput);
        movementInput = Vector2.ClampMagnitude(movementInput, 1f);
        
    }

    private void Move()
    {
        Vector3 movementDirection = new Vector3(movementInput.x, movementInput.y, 0f);
        Vector3 movementAmount = movementDirection * movementSpeed * Time.deltaTime;
        
        movementAmount.x += currentbarrelRollMomentum * Time.deltaTime;
        currentbarrelRollMomentum = Mathf.MoveTowards(currentbarrelRollMomentum, 0f, barrelRollMomentRecovery* Time.deltaTime);
        
        
        Vector3 targetPosition = transform.position + movementAmount;
        Bounds bounds = movementBounds.bounds;
        
        targetPosition.x = Mathf.Clamp(targetPosition.x, bounds.min.x, bounds.max.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, bounds.min.y, bounds.max.y);
        
        transform.position = targetPosition;
        
    }
}
