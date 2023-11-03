using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;


public class Manager : MonoBehaviour
{
    Block[] blocks;

    [SerializeField]
    private GameObject jengaBlockPrefab, jengaLevelPrefab, gradeLabelPrefab;

    public List<Grade> grades = new List<Grade>();

    [SerializeField]
    private Material glassMat, woodMat, stoneMat;

    [SerializeField]
    private GameObject loadingText, mainUI;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetStacksInfo());
    }


    void PlaceJengaBlocks()
    {

        Vector3 startingPos = Vector3.zero;

        foreach (Grade grade in grades)
        {
            // Sort blocks first
            grade.blocks.Sort();

            GameObject obj = new GameObject(grade.grade + " Jenga");
            grade.gradeGO = obj;

            GameObject jengaLevelsParent = new GameObject("ActualJenga");
            jengaLevelsParent.transform.parent = obj.transform;

            GameObject label = Instantiate(gradeLabelPrefab, obj.transform.position + new Vector3(0, 1, -5), gradeLabelPrefab.transform.rotation, obj.transform);
            label.GetComponent<TMP_Text>().text = grade.grade;

            GameObject focusPoint = new GameObject("FocusPoint");
            focusPoint.transform.parent = obj.transform;

            obj.transform.position = startingPos;
            startingPos += new Vector3(10, 0, 0);
            int blockCount = 0;
            int currentHeight = 0;
            GameObject currentLevel = Instantiate(jengaLevelPrefab, jengaLevelsParent.transform);
            bool isLevelRotated = false;
            for (int i = 0; i < grade.blocks.Count; i++)
            {
                if (blockCount > 2)
                {
                    isLevelRotated = !isLevelRotated;
                    currentHeight += 1;
                    currentLevel = Instantiate(jengaLevelPrefab, jengaLevelsParent.transform);
                    currentLevel.transform.position += new Vector3(0, currentHeight, 0);
                    if (isLevelRotated)
                        currentLevel.transform.Rotate(new Vector3(0, 90, 0));
                    blockCount = 0;
                }

                GameObject newBlock = Instantiate(jengaBlockPrefab, currentLevel.transform.GetChild(blockCount));
                switch (grade.blocks[i].mastery)
                {
                    case 0:
                        newBlock.GetComponent<MeshRenderer>().material = glassMat;
                        break;
  
                    case 1:
                        newBlock.GetComponent<MeshRenderer>().material = woodMat;
                        break;
                        
                    case 2:
                        newBlock.GetComponent<MeshRenderer>().material = stoneMat;
                        break;

                }
                newBlock.name = i.ToString();
                grade.blocksGO.Add(newBlock);
                blockCount++;

            }
            focusPoint.transform.position += new Vector3(0, currentHeight / 2, 0);

        }

        // focus to the first grade on default
        Camera.main.gameObject.GetComponent<CameraMovement>().ChangeFocus(grades[0].gradeGO.transform.Find("FocusPoint").transform);
    }



    IEnumerator GetStacksInfo()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack"))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                string json = www.downloadHandler.text;

                blocks = JsonConvert.DeserializeObject<Block[]>(json);

                List<string> existingGrades = new List<string>();
                foreach(Block block in blocks)
                {
                    if (existingGrades.Contains(block.grade))
                    {
                        // find the existing Grade
                        foreach (Grade grade in grades)
                        {
                            if (grade.grade == block.grade)
                            {
                                grade.blocks.Add(block);
                                break;
                            }
                        }
                    }
                    else
                    {
                        // creates a new Grade
                        Grade newGrade = new Grade();
                        newGrade.blocks.Add(block);
                        newGrade.grade = block.grade;

                        grades.Add(newGrade);
                        existingGrades.Add(block.grade);
                    }                      
                }

                loadingText.SetActive(false);
                mainUI.SetActive(true);
                PlaceJengaBlocks();
            }
        }
    }

}

