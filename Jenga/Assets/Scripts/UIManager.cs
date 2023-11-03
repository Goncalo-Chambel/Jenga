using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Manager mainManager;

    [SerializeField]
    private ChallengeManager challengeManager;

    public int currentGradeIndex = 0;

    bool isBlockSelected = false;
    bool testMyStackSelected = false;
    bool testMyStackStarted, testMyStackFailed = false;
    bool isTestResultPanelOpen = false;

    int blockOnTheGround = 0;

    [SerializeField]
    private TMP_Text timer;
    float startTime = 3f;
    float testMyStackTimer = 5f;
    GameObject gradeToBeTested;

    private Transform selectedBlock;

    [SerializeField]
    private GameObject blockPanel, mainUI, testResultPanel;

    public void TestMyStackButton()
    {
        gradeToBeTested = Instantiate(mainManager.grades[currentGradeIndex].gradeGO);
        foreach (Grade grade in mainManager.grades)
            grade.gradeGO.SetActive(false);

        Camera.main.gameObject.GetComponent<CameraMovement>().ChangeFocus(mainManager.grades[currentGradeIndex].gradeGO.transform.Find("FocusPoint").transform, 15);
        mainUI.SetActive(false);
        testMyStackSelected = true;
        timer.gameObject.SetActive(true);

    }
    void StartTestMyStack()
    {
        challengeManager.TestMyStack(gradeToBeTested);
    }

    public void NextGrade()
    {
        currentGradeIndex++;
        if (currentGradeIndex >= mainManager.grades.Count)
            currentGradeIndex = 0;

        Camera.main.gameObject.GetComponent<CameraMovement>().ChangeFocus(mainManager.grades[currentGradeIndex].gradeGO.transform.Find("FocusPoint").transform);

    }
    public void PreviousGrade()
    {
        currentGradeIndex--;
        if (currentGradeIndex < 0)
            currentGradeIndex = mainManager.grades.Count - 1;

        Camera.main.gameObject.GetComponent<CameraMovement>().ChangeFocus(mainManager.grades[currentGradeIndex].gradeGO.transform.Find("FocusPoint").transform);

    }


    public void Update()
    {
        if (!isBlockSelected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if(int.TryParse(hit.transform.gameObject.name,out int result))
                    {
                        if (hit.transform.parent.parent.parent.parent.name == mainManager.grades[currentGradeIndex].gradeGO.name)
                        {
                            isBlockSelected = true;
                            OpenBlockPanel(hit.transform.gameObject);
                            Camera.main.GetComponent<CameraMovement>().FocusOnBlock(hit.transform);
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Camera.main.gameObject.GetComponent<CameraMovement>().ChangeFocus(mainManager.grades[currentGradeIndex].gradeGO.transform.Find("FocusPoint").transform);
                isBlockSelected = false;
                blockPanel.SetActive(false);
                selectedBlock.GetComponent<Outline>().enabled = false;
            }
        }
        if (testMyStackSelected && !isTestResultPanelOpen)
        {
            startTime -= Time.deltaTime;
            timer.text = startTime.ToString("F0");
            if (startTime < 0)
            {
                testMyStackTimer -= Time.deltaTime;
                if(!testMyStackStarted)
                {
                    timer.gameObject.SetActive(false);
                    StartTestMyStack();
                    testMyStackStarted = true;
                }
                if(testMyStackFailed)
                {
                    //Test failed
                    ShowTestResult(false);        
                }
                else
                {
                    if(testMyStackTimer < 0)
                    {
                        // Test passed
                        ShowTestResult(true);
                    }
                }
            }
        }

    }

    void ShowTestResult(bool passed)
    {
        testResultPanel.SetActive(true);
        isTestResultPanelOpen = true;
        if (passed)
        {
            testResultPanel.transform.Find("Result").GetComponent<TMP_Text>().text = "Test Passed!";
            testResultPanel.transform.Find("Result").GetComponent<TMP_Text>().color = Color.green;
            testResultPanel.transform.Find("Message").GetComponent<TMP_Text>().text = "Your stack is rock solid!";
        }
        else
        {
            testResultPanel.transform.Find("Result").GetComponent<TMP_Text>().text = "Test Failed...";
            testResultPanel.transform.Find("Result").GetComponent<TMP_Text>().color = Color.red;
            testResultPanel.transform.Find("Message").GetComponent<TMP_Text>().text = "There are still some foundations missing";
        }
    }

    public void CloseTestResultPanel()
    {
        testMyStackFailed = false;
        testResultPanel.SetActive(false);
        isTestResultPanelOpen = false;
        testMyStackSelected = false;
        testMyStackStarted = false;
        startTime = 3f;
        testMyStackTimer = 5f;
        foreach (Grade grade in mainManager.grades)
            grade.gradeGO.SetActive(true);
        Destroy(gradeToBeTested);
        Camera.main.gameObject.GetComponent<CameraMovement>().ChangeFocus(mainManager.grades[currentGradeIndex].gradeGO.transform.Find("FocusPoint").transform);
        mainUI.SetActive(true);
        blockOnTheGround = 0;

    }
    void OpenBlockPanel(GameObject block)
    {
        selectedBlock = block.transform;
        blockPanel.SetActive(true);
        int blockIndex = int.Parse(block.name);
        blockPanel.transform.Find("Info/Info1").GetComponent<Text>().text = mainManager.grades[currentGradeIndex].blocks[blockIndex].grade + " : " +
                                                                                mainManager.grades[currentGradeIndex].blocks[blockIndex].domain;

        blockPanel.transform.Find("Info/Info2").GetComponent<Text>().text = mainManager.grades[currentGradeIndex].blocks[blockIndex].cluster;

        blockPanel.transform.Find("Info/Info3").GetComponent<Text>().text = mainManager.grades[currentGradeIndex].blocks[blockIndex].standardid + " : " +
                                                                        mainManager.grades[currentGradeIndex].blocks[blockIndex].standarddescription;
    }

    public void BlockTouchedGround()
    {
        blockOnTheGround++;
        if(blockOnTheGround > 3)
            testMyStackFailed = true;
    }
    public void Exit()
    {
        Application.Quit();
    }

}
