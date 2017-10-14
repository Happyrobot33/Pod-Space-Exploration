using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasController : MonoBehaviour {


    Transform player;
    PlayerResources resources;
    [HideInInspector] public RectTransform edgeMarker;

    [HideInInspector] public RectTransform menu;
    RectTransform menu1;
    RectTransform menu2;


    public Slider throttleSlider;

    public Slider pitchSlider;
    public Slider yawSlider;
    public Slider rollSlider;

    RectTransform torqueSlider;
    [HideInInspector] public RectTransform torqueKnob;
    Image stabilizeButton;
    [HideInInspector] public bool stabilizeButtonState = false;
    Text velocityText;
    Dropdown targetDropdown;

    Image cameraModeButton;
    [HideInInspector] public bool cameraModeButtonState = false;
    Slider timeSlider;
    Text timeText;
    

    //public Slider fuelSlider;
    //public Image fuelFill;

    [HideInInspector]public float openPixels;
    void Start()
    {
        openPixels = menu.anchoredPosition.x;
        if (Camera.main.GetComponent<CameraController>().mode == CameraController.CameraMode.Free)
            cameraModeButtonState = false;
        else
            cameraModeButtonState = true;
    }

    void OnEnable()
    {
        player = Camera.main.GetComponent<CameraController>().target;
        //resources = player.GetComponent<PlayerResources>();

        // find the ui components
        menu = transform.Find("Menu").GetComponent<RectTransform>();
        menu1 = menu.Find("Menu1").GetComponent<RectTransform>();
        menu2 = menu.Find("Menu2").GetComponent<RectTransform>();

        edgeMarker = menu.Find("Edge Marker").GetComponent<RectTransform>();

        throttleSlider = menu1.Find("Throttle Slider").GetComponent<Slider>();

        pitchSlider = menu1.Find("Pitch Slider").GetComponent<Slider>();
        yawSlider = menu1.Find("Yaw Slider").GetComponent<Slider>();
        rollSlider = menu1.Find("Roll Slider").GetComponent<Slider>();

        torqueSlider = menu1.Find("Torque Slider").GetComponent<RectTransform>();
        torqueKnob = menu1.Find("Torque Slider").Find("Knob Area").Find("Knob").GetComponent<RectTransform>();
        stabilizeButton = menu1.Find("Stabilize Button").GetComponent<Image>();
        velocityText = menu1.Find("Velocity Text").Find("Value").GetComponent<Text>();
        targetDropdown = menu1.Find("Target Dropdown").GetComponent<Dropdown>();

        cameraModeButton = menu2.Find("Camera Mode Button").GetComponent<Image>();
        timeSlider = menu2.Find("Time Slider").GetComponent<Slider>();
        timeText = menu2.Find("Time Text").GetComponent<Text>();
        
        
        
        

        

        //fuelSlider = fuelBackground.transform.extFind("Fuel Slider").GetComponent<Slider>();
        //fuelFill = fuelSlider.transform.extFind("Fill").GetComponent<Image>();
    }

    void LateUpdate()
    {
        //fuelFill.color = Color.Lerp(Color.red, Color.green, fuelSlider.value);
        //fuelSlider.value = resources.fuel / resources.maxFuel;




        // torque control
        if (torqueKnob.gameObject.activeInHierarchy)
        {
            if (draggingTorqueKnob)
            {
                // if torque knob is outside of radius
                //// then lock it's motion to a circle
                // but if inside of radius
                //// then make it move to the position of the pointer

                torqueKnob.anchoredPosition += touchData.position - torqueKnob.extScreenPosition();
                float radius = torqueSlider.sizeDelta.magnitude / 6f;
                if (torqueKnob.anchoredPosition.magnitude > radius)
                    torqueKnob.anchoredPosition = torqueKnob.anchoredPosition.normalized * radius;
            }
            else if (torqueKnob.anchoredPosition != Vector2.zero)
                torqueKnob.anchoredPosition = Vector2.zero;
        }
        
        
        // stabilize - change the color of the button when pressed
        if(stabilizeButton.gameObject.activeInHierarchy)
        {
            if (stabilizeButtonState)
                stabilizeButton.color = Color.green * Color.white;
            else
                stabilizeButton.color = Color.grey * Color.white;
        }
        

        



        // time control, 10^value
        if(Camera.main.GetComponent<TimeController>().followCanvas)
        {
            float boost = Mathf.Pow(10f, timeSlider.value);
            timeText.text = "Time Boost: " + Mathf.Round(boost) + "x";
            Camera.main.GetComponent<TimeController>().timeMultiplier = boost;
        }
        

        // update velocity text
        if(velocityText.gameObject.activeInHierarchy)
        {
            float tmpSpeed = player.GetComponent<Rigidbody>().velocity.magnitude * 10f;
            string speed = Mathf.RoundToInt(tmpSpeed).ToString();
            if (speed.Length >= 7)
                velocityText.fontSize = 25 - (2 * (speed.Length - 7));
            else
                velocityText.fontSize = 29;
            velocityText.text = speed + " m/s";
        }
        



        // camera mode, if fixed..
        if (cameraModeButtonState)
        {
            Camera.main.GetComponent<CameraController>().mode = CameraController.CameraMode.Fixed;
            cameraModeButton.color = new Color(0.4f, 0.4f, 1f);
        }
        else
        {
            Camera.main.GetComponent<CameraController>().mode = CameraController.CameraMode.Free;
            cameraModeButton.color = new Color(0.6f, 0.6f, 1f);
        }


        // apply dropdown value
        player.GetComponent<PlayerMovement>().targetMode = targetDropdown.value;

    }
    

    // this is called when menu is opened or closed
    bool menuOpen = false;
    public void ToggleMenu()
    {
        if(menuOpen== false)
            menu.anchoredPosition += new Vector2(-openPixels, 0f);
        else
            menu.anchoredPosition += new Vector2(openPixels, 0f);
        menuOpen = !menuOpen;
    }

    // this is when submenus are opened
    public List<GameObject> subMenus = new List<GameObject>();
    public List<Image> menuButtons = new List<Image>();
    public void ToggleSubmenu(GameObject menu)
    {
        CloseMenus();
        menu.SetActive(true);
        menuButtons[subMenus.IndexOf(menu)].color = new Color(0.6f, 0.6f, 0.6f);
    }
    public void CloseMenus()
    {
        for (int i = 0; i < subMenus.Count; i++)
            subMenus[i].SetActive(false);
        for (int i = 0; i < menuButtons.Count; i++)
            menuButtons[i].color = Color.grey;
    }


    // torque control
    // when the knob is moved, DuringTorqueDrag() is called, 
    //// which sets draggingTorqueKnob to true
    bool draggingTorqueKnob = false;
    PointerEventData touchData;
    public void DuringTorqueDrag(BaseEventData baseData)
    {
        draggingTorqueKnob = true;
        touchData = (PointerEventData)baseData;
    }
    public void EndTorqueDrag()
    {
        draggingTorqueKnob = false;
    }


    // stabilize button
    public void ToggleStablizilize()
    {
        stabilizeButtonState = !stabilizeButtonState;
    }


    // camera mode
    public void ToggleCameraMode()
    {
        cameraModeButtonState = !cameraModeButtonState;
    }


}
