using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CapturePhotoScript : MonoBehaviour
{
    public InputActionProperty showButton;
    public Transform MainCam;
    public Transform right;
    public float spawnDistance = 5;
    public GameObject photoFrame;
    public GameObject DroneCam;
    public GameObject RustAICam;
    public GameObject DeckAICam;
    public GameObject PierAICam;
    public GameObject RealCam;
    public CharacterController characterController;
    public float speed = 1f;
    public GameObject photoObject;
    public Vector3 offset = new Vector3(0.2f, 0.2f, 0.2f);
    //public float spawnDistance = 2;
    // Start is called before the first frame update
    void Start()
    {
        photoFrame.SetActive(true);
        photoObject.SetActive(false);
    }

    // Update is called once per frame
    private bool isFollowing = false;

    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            isFollowing = true;
            DroneCam.SetActive(false);
            RustAICam.SetActive(false);
            DeckAICam.SetActive(false);
            PierAICam.SetActive(false);
            photoObject.transform.position = right.transform.position + offset;
            photoObject.SetActive(true);
        }

        if (showButton.action.WasReleasedThisFrame())
        {
            isFollowing = false;
            DroneCam.SetActive(true);
            RustAICam.SetActive(true);
            DeckAICam.SetActive(true);
            PierAICam.SetActive(true);
            DroneCam.transform.position = MainCam.position;
            RustAICam.transform.position = MainCam.position;
            DeckAICam.transform.position = MainCam.position;
            PierAICam.transform.position = MainCam.position;
            DroneCam.transform.rotation = MainCam.rotation;
            RustAICam.transform.rotation = MainCam.rotation;
            DeckAICam.transform.rotation = MainCam.rotation;
            PierAICam.transform.rotation = MainCam.rotation;
            photoObject.SetActive(false);
        }

        if (isFollowing)
        {
            //photoObject.transform.position = right.transform.position;
            photoObject.transform.position = right.transform.position + new Vector3(right.transform.forward.x, right.transform.forward.y, right.transform.forward.z).normalized * spawnDistance;
            photoObject.transform.LookAt(new Vector3(MainCam.transform.position.x, MainCam.transform.position.y, MainCam.transform.position.z));
            photoObject.transform.forward *= -1;
            RealCam.transform.position = MainCam.position;
            RealCam.transform.rotation = MainCam.rotation;
        }
    }

}
