using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;


[System.Serializable]
public class BoxColliderTextMeshProPair
{
    public BoxCollider BoxCollider;
    public TMPro.TextMeshProUGUI TextMeshPro;
    public TMPro.TextMeshProUGUI EXP;
    public TMPro.TextMeshProUGUI noEXP;
    public Toggle Toggle;
    public Dropdown Dropdown;
    public GameObject AIEXP;
    public GameObject EXPAIEXP;
    public int Condition;
    public int ID;
    public float Distance { get; set; }

    public BoxColliderTextMeshProPair(BoxCollider boxCollider, TMPro.TextMeshProUGUI textMeshPro, Toggle toggle, Dropdown dropdown, int condition, GameObject aIEXP, GameObject eXPAIEXP, int id)
    {
        BoxCollider = boxCollider;
        TextMeshPro = textMeshPro;
        Toggle = toggle;
        Dropdown = dropdown;
        Condition = condition;
        AIEXP = aIEXP;
        EXPAIEXP = eXPAIEXP;
        ID = id;
    }
}

public class HandMenuManager : MonoBehaviour
{
    public Transform head;
    public Transform leftController;
    public float spawnDistance = 2;
    public GameObject menu;
    public GameObject confirmCanvas;
    public InputActionProperty showButton;
    public Toggle[] toggles;
    public Dropdown[] dropdowns;
    public TMPro.TextMeshProUGUI[] textLabels;
    public Button submitButton;
    public GameObject DroneCam;
    public GameObject ExpRustAI;
    public GameObject ExpRefPierAI;
    public GameObject ExpDeckAI;
    public GameObject ExpAI;
    public GameObject RustAI;
    public GameObject PierAI;
    public GameObject DeckAI;
    public GameObject AI;
    public string filenamePrefix;
    public BoxCollider boxCollider;
    public TMPro.TextMeshProUGUI textMeshPro;
    public List<BoxColliderTextMeshProPair> boxColliderTextMeshProPairs = new List<BoxColliderTextMeshProPair>();
    public GameObject[] ReferenceImages;
    public InputActionProperty captureButton;
    public Button yesButton;
    public Button noButton;
    public Button[] buttons;
    public Button[] controlButtons; // The three top buttons
    private BoxColliderTextMeshProPair closestPair;
    public Color defaultColor = Color.white;
    public Color clickedColor = Color.red;
    private Button lastClickedButton = null;
    private Button lastSelectedButton = null;

    public Camera mainCamera; // Assign this from the inspector
    public GameObject[] backgrounds; // Assign your 3 backgrounds here in the inspector

    private float[] gazeTimes = new float[3]; // Assuming 3 backgrounds
    private int currentBackgroundIndex = -1;
    private float gazeStartTime;
    public bool[] AiExplanation;
    public bool[] AiNoExplanation;
    public bool[] noAI;
    public TMPro.TextMeshProUGUI Disabled;





    private float startTime;
    private float menuPressedTime;


    // Start is called before the first frame update
    void Start()
    {
        // Modify the submit button to open the confirmation canvas instead of directly submitting
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnConfirmationRequest);
        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);
        startTime = Time.time;
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // To prevent the closure problem in C#
            buttons[i].onClick.AddListener(() => {
                if (lastSelectedButton != null)
                {
                    lastSelectedButton.GetComponent<Image>().color = defaultColor;
                }
                buttons[index].GetComponent<Image>().color = clickedColor;
                lastSelectedButton = buttons[index];
                ButtonClicked(index);
            });
        }

        controlButtons[0].onClick.AddListener(() => ControlButtonClicked(0, 3));
        controlButtons[1].onClick.AddListener(() => ControlButtonClicked(3, 6));
        controlButtons[2].onClick.AddListener(() => ControlButtonClicked(6, 8));
    }
    void ButtonClicked(int buttonIndex)
    {
        // Set all images to inactive
        foreach (var image in ReferenceImages)
        {
            image.SetActive(false);
        }

        // Set the corresponding image to active
        ReferenceImages[buttonIndex].SetActive(true);
    }

    void ControlButtonClicked(int start, int end)
    {
        // If there is a previously clicked button, reset its color
        if (lastClickedButton != null)
        {
            lastClickedButton.GetComponent<Image>().color = defaultColor;
        }

        // Set the corresponding buttons to active and select the first one
        for (int i = start; i < end; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }
        if (start < buttons.Length)
        {
            buttons[start].onClick.Invoke();
        }

        // Set all buttons to inactive
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        // Set the corresponding buttons to active
        for (int i = start; i < end; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }

        // Change the color of the clicked button
        controlButtons[start / 3].GetComponent<Image>().color = clickedColor;

        // Save the clicked button
        lastClickedButton = controlButtons[start / 3];
    }
    /*
    void OnToggleValueChanged(bool isOn)
    {
        //Debug.Log(isOn);
        if (isOn)
        {
            for (int i = 0; i < dropdowns.Length; i++)
            {
                OnDropdownValueChange(i, dropdowns[i].value);
            }
        }
        else
        {
            // Optional: Do something when the toggle is turned off
        }
    }

    // New method for handling dropdown value changes
    void OnDropdownValueChange(int dropdownIndex, int value)
    {
        //Debug.Log("value and index");
        Debug.Log("value - " + value.ToString());
        Debug.Log("dropdown index - " + dropdownIndex.ToString());
        // Initially disable all reference images
        for (int i = 0; i < ReferenceImages.Length; i++)
        {
            ReferenceImages[i].SetActive(false);
        }

        // Calculate the corresponding reference image index
        int referenceImageIndex = dropdownIndex * 3 + value;
        Debug.Log("ReferenceINdex - " + referenceImageIndex.ToString());

        // Make sure the calculated index is valid
        if (referenceImageIndex >= 0 && referenceImageIndex < ReferenceImages.Length)
        {
            // Enable the corresponding reference image
            ReferenceImages[referenceImageIndex].SetActive(true);
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        // Raycasting to detect which background the user is looking at
        if (menu.activeSelf)
        {
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                int hitBackgroundIndex = Array.IndexOf(backgrounds, hit.transform.gameObject);
                if (hitBackgroundIndex != -1)
                {
                    if (currentBackgroundIndex != hitBackgroundIndex)
                    {
                        if (currentBackgroundIndex != -1)
                        {
                            gazeTimes[currentBackgroundIndex] += Time.time - gazeStartTime;
                        }

                        currentBackgroundIndex = hitBackgroundIndex;
                        gazeStartTime = Time.time;
                    }
                }
                else if (currentBackgroundIndex != -1)
                {
                    gazeTimes[currentBackgroundIndex] += Time.time - gazeStartTime;
                    currentBackgroundIndex = -1;
                }
            }
            else if (currentBackgroundIndex != -1)
            {
                gazeTimes[currentBackgroundIndex] += Time.time - gazeStartTime;
                currentBackgroundIndex = -1;
            }
        }
        if (showButton.action.WasPressedThisFrame())
        {
            bool isMenuActive = menu.activeSelf;
            menu.SetActive(!isMenuActive);

            if (!isMenuActive)
            {
                menuPressedTime = Time.time;
            }
            // Reset gazeTimes
            for (int i = 0; i < gazeTimes.Length; i++)
            {
                gazeTimes[i] = 0;
            }

        }
        for (int i = 0; i < toggles.Length; i++)
        {
            dropdowns[i].gameObject.SetActive(toggles[i].isOn);
            textLabels[i].gameObject.SetActive(toggles[i].isOn);
        }
        //menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance + new Vector3(0, 1, 0).normalized; 
        menu.transform.position = leftController.transform.position + new Vector3(leftController.transform.forward.x, leftController.transform.forward.y, leftController.transform.forward.z).normalized * spawnDistance;
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;

        Camera droneCam = DroneCam.GetComponent<Camera>();
        float minDistance = float.PositiveInfinity;
        // Prepare the layer mask for the raycast
        int layerMask = 1 << LayerMask.NameToLayer("RaycastOnly");
        if (captureButton.action.WasReleasedThisFrame())
        {
            closestPair = null;
            
            foreach (BoxColliderTextMeshProPair pair in boxColliderTextMeshProPairs)
            {
                Vector3 direction = pair.BoxCollider.transform.position - droneCam.transform.position;
                //Debug.DrawRay(droneCam.transform.position, direction, Color.red, 15f);
                RaycastHit hit;
                if (Physics.Raycast(droneCam.transform.position, direction, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider == pair.BoxCollider)
                    {
                        float angle = Vector3.Angle(droneCam.transform.forward, direction);
                        if (angle < droneCam.fieldOfView * 0.5f)
                        {
                            // The box collider is in the frame
                            pair.Distance = hit.distance;
                            if (hit.distance < minDistance)
                            {
                                minDistance = hit.distance;
                                closestPair = pair;
                            }
                        }
                    }
                }
            }

            // Disable all TextMeshPro elements
            foreach (BoxColliderTextMeshProPair pair in boxColliderTextMeshProPairs)
            {
                pair.TextMeshPro.gameObject.SetActive(false);
                pair.EXP.gameObject.SetActive(false);
                pair.noEXP.gameObject.SetActive(false);
            }
            Disabled.gameObject.SetActive(false);
            // Enable the TextMeshPro element associated with the closest box collider
            AI.SetActive(true);
            ExpAI.SetActive(true);
            if (closestPair != null)
            {
                //Debug.Log(closestPair.ID);
                for (int i = 0; i < toggles.Length; i++)
                {
                    toggles[i].isOn = false;
                    dropdowns[i].value = 0;
                }
                ExpRustAI.SetActive(false);
                ExpRefPierAI.SetActive(false);
                ExpDeckAI.SetActive(false);
                ExpAI.SetActive(false);
                RustAI.SetActive(false);
                PierAI.SetActive(false);
                DeckAI.SetActive(false);
                AI.SetActive(false);
                if (closestPair.ID != 0) {
                    if(noAI[closestPair.ID - 1] == false)
                    {
                            if (AiNoExplanation[closestPair.ID - 1] == false)
                            {
                                closestPair.Toggle.isOn = true;
                                closestPair.Dropdown.value = closestPair.Condition;
                                closestPair.AIEXP.SetActive(true);
                                closestPair.EXPAIEXP.SetActive(true);
                                closestPair.EXP.gameObject.SetActive(true);
                            }
                            else
                            {
                                closestPair.Toggle.isOn = true;
                                closestPair.Dropdown.value = closestPair.Condition;
                                closestPair.noEXP.gameObject.SetActive(true);
                            }
                        }
                    else
                    {
                            Disabled.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    void OnConfirmationRequest()
    {
        confirmCanvas.SetActive(true);
    }

    // Handler for when the 'Yes' button on the confirmation canvas is pressed
    void OnYes()
    {
        confirmCanvas.SetActive(false);
        OnSubmit();
    }

    // Handler for when the 'No' button on the confirmation canvas is pressed
    void OnNo()
    {
        confirmCanvas.SetActive(false);
    }

    void OnSubmit()
    {
        // Define CSV path
        string csvPath = Path.Combine(Application.persistentDataPath, filenamePrefix + "results.csv");

        // Check if file exists
        bool fileExists = File.Exists(csvPath);

        // Create a StreamWriter
        using (StreamWriter writer = new StreamWriter(csvPath, true))
        {
            // If file didn't exist, write the headers
            if (!fileExists)
            {
                //writer.WriteLine("Rust,Spall,Crack,Time,Interaction,XAI,Photo");
                writer.WriteLine("Rust,Spall,Crack,Time,Interaction,XAI,GazeTime_Reference,GazeTime_Response,GazeTime_AI,Photo, AI_exp");
            }

            // Write dropdown selections or 'N/A' to CSV
            for (int i = 0; i < dropdowns.Length; i++)
            {
                if (toggles[i].isOn)
                {
                    writer.Write(dropdowns[i].options[dropdowns[i].value].text);
                }
                else
                {
                    writer.Write("N/A");
                }

                if (i < dropdowns.Length - 1) writer.Write(",");
            }

            // Write time from the beginning to CSV
            float timeFromStart = Time.time - startTime;
            writer.Write("," + timeFromStart.ToString());

            // Write interaction time to CSV
            float interactionTime = Time.time - menuPressedTime;
            writer.Write("," + interactionTime.ToString());
            // Write TextMeshPro value of the closest pair to CSV
            if (closestPair != null)
            {
                string cleanedText = ",N/A";
                if (closestPair.ID != 0)
                {
                    if (noAI[closestPair.ID - 1] == false)
                    {
                        if (AiNoExplanation[closestPair.ID - 1] == false)
                        {
                            cleanedText = closestPair.EXP.text.Replace("\n", " "); // Replace newlines with spaces
                        }
                        else
                        {
                            cleanedText = closestPair.noEXP.text.Replace("\n", " "); // Replace newlines with spaces
                        }
                    }
                    else
                    {
                        cleanedText = Disabled.text.Replace("\n", " ");
                    }
                }
                    //string cleanedText = closestPair.TextMeshPro.text.Replace("\n", " "); // Replace newlines with spaces
                writer.Write("," + cleanedText);
            }
            else
            {
                writer.Write(",N/A");
            }
            // Append gaze times for each background to the CSV
            writer.Write("," + gazeTimes[0].ToString()); // GazeTime for Reference
            writer.Write("," + gazeTimes[1].ToString()); // GazeTime for Response
            writer.Write("," + gazeTimes[2].ToString()); // GazeTime for AI


            // Save image to PNG
            // Make sure your DroneCam has a Camera component and a RenderTexture set as Target Texture
            Camera droneCam = DroneCam.GetComponent<Camera>();
            RenderTexture.active = droneCam.targetTexture;
            Texture2D tex = new Texture2D(droneCam.targetTexture.width, droneCam.targetTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, droneCam.targetTexture.width, droneCam.targetTexture.height), 0, 0);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);
            RenderTexture.active = null;

            // Use current date and time as the photo name
            string photoName = filenamePrefix + System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string uniquePhotoName = filenamePrefix + "_exp_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string pngPath = Path.Combine(Application.persistentDataPath, "photos", photoName + ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(pngPath));
            File.WriteAllBytes(pngPath, bytes);

            // Write photo name to CSV
            writer.WriteLine("," + photoName);
            writer.WriteLine("," + uniquePhotoName);
            if (closestPair != null)
            {
                // Get the RawImage component from the EXPAIEXP GameObject
                RawImage rawImageComponent = closestPair.AIEXP.GetComponent<RawImage>();
    
                if(rawImageComponent != null)
                {
                    Texture textureInRawImage = rawImageComponent.texture;
        
                    if (textureInRawImage is RenderTexture renderTextureToSave)
                    {
                        // Activate the RenderTexture to read pixels from it
                        RenderTexture.active = renderTextureToSave;

                        // Create a Texture2D and read the active RenderTexture into it
                        Texture2D tempTexture = new Texture2D(renderTextureToSave.width, renderTextureToSave.height, TextureFormat.RGB24, false);
                        tempTexture.ReadPixels(new Rect(0, 0, renderTextureToSave.width, renderTextureToSave.height), 0, 0);
                        tempTexture.Apply();

                        // Encode Texture2D to PNG
                        byte[] pngBytes = tempTexture.EncodeToPNG();
                        Destroy(tempTexture);
                        RenderTexture.active = null;

                        // Generate PNG file path and name
                        //string uniquePhotoName = filenamePrefix + "_exp_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                        string pngFilePath = Path.Combine(Application.persistentDataPath, "photos", uniquePhotoName + ".png");
                        Directory.CreateDirectory(Path.GetDirectoryName(pngFilePath));

                        // Save PNG to disk
                        File.WriteAllBytes(pngFilePath, pngBytes);

                        // Write photo name to CSV (assuming 'writer' is a StreamWriter or similar object)
                        //writer.WriteLine("," + uniquePhotoName);
                    }
                    else
                    {
                        Debug.LogWarning("The RawImage does not have a RenderTexture as its texture.");
                    }
                }
                else
                {
                    Debug.LogWarning("The EXPAIEXP GameObject does not have a RawImage component.");
                }
            }
            else
            {
                Debug.LogWarning("closestPair is null.");
            }
            

            // Disable the menu
            menu.SetActive(false);

            // Reset toggles and dropdowns
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = false;
                dropdowns[i].value = 0;
            }
        }
    }


}
