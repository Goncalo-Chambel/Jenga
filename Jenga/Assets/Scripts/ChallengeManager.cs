using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{


    public void TestMyStack(GameObject grade)
    {
        grade.transform.Find("GradeLabelPrefab(Clone)").gameObject.SetActive(false);
        TransverseChildren(grade.transform);     
    }

    void TransverseChildren(Transform grade)
    {
        foreach(Transform child in grade)
        {
            if (child.name.Contains("Block"))
            {
                if (child.childCount > 0)
                {
                    if (child.GetChild(0).GetComponent<MeshRenderer>().material.name.Contains("Glass"))
                        child.gameObject.SetActive(false);
                    else
                        child.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                }
            }
            TransverseChildren(child);
        }
    }
}
