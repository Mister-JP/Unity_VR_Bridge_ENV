using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;
using UnityEngine.XR;

public class FlyControl : MonoBehaviour
{
    public InputActionProperty flyButton;
    public InputActionProperty downButton;
    public InputActionProperty joyStickInput;
    public CharacterController characterController;
    public float speed = 1f;
    public float gravity =0f;
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        Vector2 joystickPosition = joyStickInput.action.ReadValue<Vector2>();

        // Threshold to determine if joystick is moved enough vertically
        float verticalThreshold = 0.5f;

        // If joystick is moved upwards beyond the threshold, increase height
        if (joystickPosition.y > verticalThreshold)
        {
            characterController.Move(Vector3.up * speed * Time.deltaTime);
            velocity.y = 0;
        }
        // If joystick is moved downwards beyond the threshold, decrease height
        else if (joystickPosition.y < -verticalThreshold)
        {
            characterController.Move(Vector3.down * speed * Time.deltaTime);
            velocity.y = 0;
        }
        // If joystick is not being moved, apply gravity
        else
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if (flyButton.action.ReadValue<float>() > 0)
        {
            //Debug.Log("Here1");
            //Debug.Log(Time.deltaTime);
            characterController.height += speed * Time.deltaTime;
           // Debug.Log(characterController.height);
        }

        if (downButton.action.ReadValue<float>() > 0)
        {
            //Debug.Log(Time.deltaTime);
            //Debug.Log("2");
            characterController.height -= speed * Time.deltaTime;
            //Debug.Log(characterController.height);
        }
    }*/
}
