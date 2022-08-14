using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCanvas;

    [SerializeField]
    private GameObject menuTable;
    private MenuMngr menuMngr;

    [SerializeField]
    public fishbone_generator fishbone1;
    public fishbone_generator fishbone2;

    private bool leftControllerFound = false;
    private bool rightControllerFound = false;
    private InputDevice leftControllerDevice;
    private InputDevice rightControllerDevice;
    private float speed = 5.0f;

    private bool calibrationMode = false;
    private bool editorMode = false;

    [SerializeField]
    private OVRHand leftHand;
    [SerializeField]
    private OVRHand rightHand;

    [SerializeField]
    private GameObject centerEye;
    [SerializeField]
    private GameObject bodyCollider;

    private OVRHand[] m_hands;

    float timeSincePressControllerButton = 0;

    private void OnValidate()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        closeMenu();
        m_hands = new OVRHand[]
        {
            leftHand,
            rightHand
        };
        menuMngr = menuTable.GetComponent<MenuMngr>();
        resetPose();
        openMenu();
    }

    void resetPose()
    {
        transform.localRotation = Quaternion.Euler(0, -centerEye.transform.localRotation.eulerAngles.y, 0);
        transform.localPosition = transform.localRotation * new Vector3(-centerEye.transform.localPosition.x, 0, -centerEye.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        bool pinched = false;
        if(leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index) || rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            pinched = true;
        if(calibrationMode) {
            menuMngr.enableGround();
            resetPose();
            if(pinched){
                calibrationMode = false;
                menuMngr.disableGround();
            }
        }

        Vector3 eyePos = centerEye.transform.position;

        bodyCollider.transform.position = new Vector3(eyePos.x, eyePos.y - 1, eyePos.z);

        if(Mathf.Abs(centerEye.transform.position.z) < 0.5f && !editorMode) {
            menuCanvas.SetActive(false);
            fishbone1.setRotating(true);
            fishbone2.setRotating(true);
        }
        if(Mathf.Abs(centerEye.transform.position.z) > 6.0f && !editorMode) {
            menuCanvas.SetActive(true);
            Vector3 pos = bodyCollider.transform.position;
            menuCanvas.transform.position = new Vector3(pos.x, pos.y + 1.0f, pos.z + 1.5f);
            menuCanvas.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Congratulation!!!";
        }
        //Debug.Log("update");
        if(!leftControllerFound)
        {
            //Debug.Log("search left controller");
            List<InputDevice> leftControllerDevices = new List<InputDevice>();
            InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, leftControllerDevices);
            if(leftControllerDevices.Count > 0){
                Debug.Log("left controller found:");
                leftControllerDevice = leftControllerDevices[0];
                leftControllerFound = true;
                Debug.Log(leftControllerDevice.name + leftControllerDevice.characteristics);
            }
        } else {
            //Debug.Log("left controller handling:");
            if(leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickVal))
            {
                transform.Translate(new Vector3(0,0.1f * speed * joystickVal.y * Time.deltaTime, 0));
                //Debug.Log("joystick left: "+joystickVal.x+" "+joystickVal.y);
            }
        }

        if(!rightControllerFound)
        {
            //Debug.Log("search right controller");
            List<InputDevice> rightControllerDevices = new List<InputDevice>();
            InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, rightControllerDevices);
            if(rightControllerDevices.Count > 0){
                Debug.Log("right controller found:");
                rightControllerDevice = rightControllerDevices[0];
                rightControllerFound = true;
                Debug.Log(rightControllerDevice.name + rightControllerDevice.characteristics);
            }
        } else {
            //Debug.Log("right controller handling:");
            if(rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickVal))
            {
                transform.Translate(new Vector3(0,0,speed * joystickVal.y * Time.deltaTime));
                //Debug.Log("joystick: "+joystickVal.x+" "+joystickVal.y);
            }
            if(rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonVal))
            {
                if(buttonVal) {
                    Debug.Log("button: "+buttonVal);
                    if(timeSincePressControllerButton > 1) {
                        if(calibrationMode) {
                            calibrationMode = false;
                            menuMngr.disableGround();
                        } else {
                            openMenu();
                        }
                        timeSincePressControllerButton = 0;
                    }
                } else {
                    timeSincePressControllerButton += Time.deltaTime;
                }
            }
        }

        if(leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index) && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            openMenu();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!editorMode) {
            Debug.Log("collision");
            menuCanvas.SetActive(true);
            menuCanvas.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "You lost, go back to the start!!!";
            Vector3 pos = bodyCollider.transform.position;
            menuCanvas.transform.position = new Vector3(pos.x, pos.y + 1.0f, pos.z + 1.5f);
            fishbone1.setRotating(false);
            fishbone2.setRotating(false);
        }
    }

    public void openAction(string str){
        if(str == "menu")
            openMenu();
        else if(str == "calibration")
            menuMngr.onClickCalibration();
    }

    public void openMenu()
    {
        Debug.Log("open menu");
        //menuCanvas.SetActive(true);
        calibrationMode = false;
        editorMode = false;
        menuMngr.openMainMenu();
    }

    public void closeMenu()
    {
        //Debug.Log("close menu");
        menuCanvas.SetActive(false);
    }

    public void startCalibration(){
        calibrationMode = true;
        editorMode = false;
    }

    public void startEditing() {
        editorMode = true;
        calibrationMode = false;

    }

    public void startPlayMode() {
        editorMode = false;
        calibrationMode = false;
    }
}
