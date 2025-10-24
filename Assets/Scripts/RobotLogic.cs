using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotLogic : MonoBehaviour
{
    public GameObject RobotIK;
    public GameObject RobotPoints;
    public GameObject RobotMagnit;

    private List<GameObject> R1PointsList;
    //private List<GameObject> PointsR2 = new();
    private InverseKin R1IK;
    private bool R1End = false;
    private Magnit R1Magnit;
    private bool isWorking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        R1Magnit = RobotMagnit.GetComponent<Magnit>();
        //R2Magnit = magnit_2.GetComponent<Magnit>();
        R1IK = RobotIK.GetComponent<InverseKin>();
        // IKR2 = Robot2.GetComponent<InverseKin>();
        R1PointsList = GetChildren(RobotPoints);
        R1IK.RobotMoveEnd += robotStatus;
    }

    private List<GameObject> GetChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        children = Enumerable.Range(0, parent.transform.childCount)
                            .Select(i => parent.transform.GetChild(i).gameObject)
                            .OrderBy(i => i.name).ToList();
        return children;
    }
    public void StartWork()
    {
        isWorking = true;
        StartCoroutine(cr());
    }

    public bool IsWorking => isWorking;
    // Update is called once per frame
    private void robotStatus()
    {
        R1End = true;
        UnityEngine.Debug.LogError("Событие конца");
    }
    IEnumerator cr()
    {
        while (isWorking)
        {
            foreach (GameObject point in R1PointsList)
            {
                R1End = false;
                UnityEngine.Debug.LogError(point.name + "---" + point.GetComponent<pointConfig>().magnitStatus);
                pointConfig pConf = point.GetComponent<pointConfig>();
                R1IK.StartMove(point);
                yield return new WaitUntil(() => R1End);
                UnityEngine.Debug.LogError("Воздействие на магнит");
                R1Magnit.magnitOn = pConf.magnitStatus == MagnitS.On ? true :
                                    pConf.magnitStatus == MagnitS.Off ? false :
                                    R1Magnit.magnitOn;
                yield return new WaitForSeconds(pConf.delay);
            }
            isWorking = false;
        }


    }
}
