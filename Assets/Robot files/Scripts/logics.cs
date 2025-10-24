using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor.PackageManager;
using S7.Net;
public class logics : MonoBehaviour
{


    public GameObject Robot1IK;
    public GameObject Robot1Points;
    public GameObject Robot1Magnit;
    public GameObject Robot2Magnit;


    private List<GameObject> R1PointsList;
    //private List<GameObject> PointsR2 = new();
    private InverseKin R1IK;
    private bool R1End = false;
    private InverseKin R2IK;
    private Magnit R1Magnit;
    private Magnit R2Magnit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        R1Magnit = Robot1Magnit.GetComponent<Magnit>();
        //R2Magnit = magnit_2.GetComponent<Magnit>();
        R1IK = Robot1IK.GetComponent<InverseKin>();
        // IKR2 = Robot2.GetComponent<InverseKin>();
        R1PointsList = GetChildren(Robot1Points);
        R1IK.RobotMoveEnd += robotStatus;

        StartCoroutine(cr());
    }
    
     private List<GameObject> GetChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        children = Enumerable.Range(0, parent.transform.childCount)
                            .Select(i => parent.transform.GetChild(i).gameObject)
                            .OrderBy(i => i.name).ToList();
        return children;
    }
    // Update is called once per frame
    private void robotStatus()
    {

        R1End = true;
        UnityEngine.Debug.LogError("Событие конца");
    }
    IEnumerator cr()
    {
        var pc = new Plc(CpuType.S71200,"127.0.0.1", 1, 1);

        while (true)
        {
            foreach (GameObject point in R1PointsList)
            {
                R1End = false;
                UnityEngine.Debug.LogError(point.name+"---"+point.GetComponent<pointConfig>().magnitStatus);
                pointConfig pConf = point.GetComponent<pointConfig>();
                R1IK.StartMove(point);
                yield return new WaitUntil(() => R1End);
                UnityEngine.Debug.LogError("Воздействие на магнит");
                R1Magnit.magnitOn = pConf.magnitStatus == MagnitS.On ? true : 
                                    pConf.magnitStatus == MagnitS.Off ? false : 
                                    R1Magnit.magnitOn;
                yield return new WaitForSeconds(pConf.delay);  
            }
        }
        

    }
}
