using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializeField]
public class Block: IComparable
{
    public int id;
    public string subject;
    public string grade;
    public int mastery;
    public string domainid;
    public string domain;
    public string cluster;
    public string standardid;
    public string standarddescription;

    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as Block;

        int result = a.domain.CompareTo(b.domain);
        if (result == 0)
        {
            result = a.cluster.CompareTo(b.cluster);
            if (result == 0)
            {
                return a.standardid.CompareTo(b.standardid);
            }
            else
                return result;
        }
        else
            return result;

    }
}

[SerializeField]
public class Grade
{
    public List<Block> blocks = new List<Block>();
    public List<GameObject> blocksGO = new List<GameObject>();
    public string grade;
    public GameObject gradeGO;
}
