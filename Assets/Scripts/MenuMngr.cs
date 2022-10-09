using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuMngr : MonoBehaviour
{
    [SerializeField]
    GameObject fishbone_container;

    GameObject deskObject;
    GameObject deskTopObject;
    GameObject mainMenu;
    GameObject editorMenu;
    GameObject insertButton;

    [SerializeField]
    fishbone_generator fishbone1;
    [SerializeField]
    fishbone_generator fishbone2;

    [SerializeField]
    PlayerController playerController;

    int fishbone_rotation_step = 0;
    int fishbone_offset = 0;
    // Start is called before the first frame update
    void Start()
    {
        deskObject = gameObject.transform.Find("desk").gameObject;
        deskTopObject = gameObject.transform.Find("desk_top").gameObject;
        mainMenu = gameObject.transform.Find("MainMenu").gameObject;
        editorMenu = gameObject.transform.Find("EditorMenu").gameObject;
        insertButton = editorMenu.transform.Find("insertButton").gameObject;

        openMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openMainMenu()
    {
        Debug.Log("openMainMenu");
        deskObject.SetActive(true);
        deskTopObject.SetActive(true);
        fishbone_container.SetActive(false);
        mainMenu.SetActive(true);
        editorMenu.SetActive(false);
    }

    public void startPlayModeWithoutDelay(){
        Debug.Log("startPlayModeWithoutDelay");
        fishbone_container.SetActive(true);
        fishbone_container.transform.localRotation = Quaternion.Euler(0,0,0);
        fishbone_container.transform.localPosition = new Vector3(0,0,1);
        playerController.startPlayMode();
        fishbone1.setOffsetPos(0);
        fishbone2.setOffsetPos(0);
        fishbone1.setRotating(true);
        fishbone1.setRotationStep(0);
        fishbone2.setRotating(true);
        fishbone2.setRotationStep(0);
    }

    public IEnumerator startPlayMode(float delay) {
        yield return new WaitForSeconds(delay);
        startPlayModeWithoutDelay();
        yield return null;
    }

    public void onClickPlay() {
        hideMenu();
        StartCoroutine(startPlayMode(1));
    }

    public void onClickCalibration() {
        Debug.Log("onClickCalibration");
        hideMenu();
        startPlayModeWithoutDelay();
        playerController.startCalibration();
    }

    public IEnumerator SetActiveAfterDelay(GameObject obj, float delay, bool state) {
        yield return new WaitForSeconds(delay);
        obj.SetActive(state);
        yield return null;
    }

    public IEnumerator startEditorMode(float delay) {
        yield return new WaitForSeconds(delay);
        fishbone_container.SetActive(true);
        fishbone_container.transform.localRotation = Quaternion.Euler(0,90,0);
        fishbone_container.transform.localPosition = new Vector3(0,0,5);
        playerController.startEditing();
        fishbone1.setRotating(false);
        fishbone1.setRotationStep(0);
        fishbone2.setRotating(false);
        fishbone2.setRotationStep(0);
        yield return null;
    }

    public void onClickRotateFishbone() {
        fishbone_rotation_step = (fishbone_rotation_step+1)%8;
        fishbone1.setRotationStep(fishbone_rotation_step*360/8);
        fishbone2.setRotationStep(fishbone_rotation_step*360/8);
        updateInsertButton();
    }

    public void setFishboneOffset(int offset)
    {
        fishbone_offset = offset;
        fishbone1.setOffsetPos(fishbone_offset);
        fishbone2.setOffsetPos(fishbone_offset);
    }

    public void onClickNextFishbone() {
        setFishboneOffset((fishbone_offset + 1) % 10);
        updateInsertButton();
    }

    public void onClickPreviousFishbone() {
        setFishboneOffset((fishbone_offset + 9) % 10);
        updateInsertButton();
    }

    public void onClickInsertFishbone() {
        bool currentVal = getCurrentFishbone().getVatAt(getCurrentFishboneLocalOffset(), getCurrentFishboneLocalRotationStep());
        getCurrentFishbone().setValAt(getCurrentFishboneLocalOffset(), getCurrentFishboneLocalRotationStep(), !currentVal);
        updateInsertButton();
    }

    public void onClickClearFishbone() {
        fishbone1.clearFishbone();
        fishbone2.clearFishbone();
    }

    public fishbone_generator getCurrentFishbone()
    {
        if(fishbone_offset >= 7)
            return fishbone2;
        else return fishbone1;
    }

    public int getCurrentFishboneLocalOffset()
    {
        if(fishbone_offset >= 7)
            return fishbone_offset - 7;
        else return fishbone_offset;
    }

    public int getCurrentFishboneLocalRotationStep()
    {
        return (fishbone_rotation_step+4)%8;
    }

    public void updateInsertButton() {
        bool currentVal = getCurrentFishbone().getVatAt(getCurrentFishboneLocalOffset(), getCurrentFishboneLocalRotationStep());
        
        string text;
        if(currentVal)
            text = "-";
        else text = "+";
        insertButton.GetComponent<PressButton>().setText(text);
    }

    public void onClickEditor() {
        mainMenu.SetActive(false);
        fishbone_container.transform.localRotation = Quaternion.Euler(0,90,0);
        fishbone_container.transform.localPosition = new Vector3(0,0,5);
        fishbone_rotation_step = 0;
        fishbone_offset = 0;
        StartCoroutine(SetActiveAfterDelay(editorMenu, 0.5f, true));
        StartCoroutine(startEditorMode(1));
    }

    public bool isGroundVisible() {
        return fishbone_container.transform.Find("steps").gameObject.activeSelf;
    }

    public void disableGround() {
        Debug.Log("disable ground");
        fishbone_container.transform.Find("steps").gameObject.SetActive(false);
        fishbone_container.transform.Find("steps2").gameObject.SetActive(false);
    }

    public void enableGround() {
        Debug.Log("enable ground");
        fishbone_container.transform.Find("steps").gameObject.SetActive(true);
        fishbone_container.transform.Find("steps2").gameObject.SetActive(true);
    }

    public void enableAction(string str){
        if(str == "collision")
            enableCollision();
        else if(str == "ground")
            enableGround();
    }

    public void disableAction(string str){
        if(str == "collision")
            disableCollision();
        else if(str == "ground")
            disableGround();
    }

    public void disableCollision() {
        Debug.Log("disable collision");
        fishbone1.setCollision(false);
        fishbone2.setCollision(false);
    }

    public void enableCollision() {
        Debug.Log("enable collision");
        fishbone1.setCollision(true);
        fishbone2.setCollision(true);
    }

    public void hideMenu() {
        deskObject.SetActive(false);
        deskTopObject.SetActive(false);
        mainMenu.SetActive(false);
        editorMenu.SetActive(false);
    }

    public void showMenu() {
        gameObject.SetActive(true);
    }

    public void setSpeedPercentage(string value) {
        Debug.Log($"setSpeedPercentage : '{value}'");
        int percent = 100;
        try
        {
            percent = Int32.Parse(value);
        }
        catch (FormatException)
        {
            Debug.Log($"setSpeedPercentage : Unable to parse '{value}'");
        }
        float ratio = percent * 0.01f;
        fishbone1.setSpeedRatio(ratio);
        fishbone2.setSpeedRatio(ratio);
    }
}
