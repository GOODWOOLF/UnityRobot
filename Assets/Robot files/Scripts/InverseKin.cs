using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System.Globalization;
using System.Threading;
using System;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEditor.PackageManager;

public class InverseKin : MonoBehaviour
{
    public bool realtime = false;
    private List<float?[]> points;
    private bool move = false;
    private float thetha1, thetha2, thetha3, thetha4, thetha5, thetha6;

    // point to calculate ik
    public float ox, oy , oz = 0;  
    private float old_ox, old_oy, old_oz;  

    public bool RobotOperate;

    private float L1 = 177.19f*2.54f;
    private float L2 = 175.98f*2.54f;
    private float L3 = 448.51f*2.54f;
    private float L4 = 142.04f*2.54f;
    private float L5 = 534.97f*2.54f;
    private float L6 = 277.59f*2.54f;

    // to calculate inverse kin manually
    public GameObject POINT;
     private Vector3 point;
    public GameObject RobotBase;
    public GameObject configRobot;

    void Start()
    {
        point = RobotBase.transform.InverseTransformPoint(POINT.transform.position);
        oy = point.x * 1000;
        oz = (point.y - 0.4f) * 1000;
        ox = point.z * 1000;
        old_ox = ox;
        old_oy = oy;
        old_oz = oz;
        

}
    private void FixedUpdate()
    {
        point = RobotBase.transform.InverseTransformPoint(POINT.transform.position);
        oy = point.x * 1000;
        oz = (point.y - 0.4f) * 1000;
        ox = point.z * 1000;
        if (!move && checkNewXYZ())
        {
            CalculateIK();
            old_ox = ox;
            old_oy = oy;
            old_oz = oz;
        }

    }
    private bool checkNewXYZ()
    {
        if (realtime)//
        {
            return true;
        }
        else if ((ox == old_ox) && (oy == old_oy) && (oz == old_oz))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void ChangeStateRobotOperate(bool state)
    {
        if (!state)
        {
            RobotOperate = true;
        }
        else
        {
            RobotOperate = false;
        }
    }
    public void CalculateIK()
    {
        points = new();
        Vector3 start = new Vector3(old_ox, old_oy, old_oz);
        Vector3 current = start;
        Vector3 end = new Vector3(ox, oy, oz);
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        print("Дистанция:" + distance);
        float traveled = 0f;
        float step = 100f;
        if (!realtime)
        {
            while (traveled < distance)
            {
                float sizeVector = Mathf.Min(step, distance - traveled);// раст между кажд точкой
                current += direction * sizeVector;
                ox = current.x;
                oy = current.y;
                oz = current.z;
                CalculateInverseKinematics();
                traveled += sizeVector;
            }
            StartCoroutine(ModifyRobot(points));
        }
        else
        {
            CalculateInverseKinematics();
            ModifyRobotRealtime();
        }
        
    }
    public void CalculateInverseKinematics()
    {
        try
        {
            print("start");
            Stopwatch time = new Stopwatch();
            time.Start();
            // Transformation matrix of the ik point
            /*
            Matrix4x4 T = new Matrix4x4(new Vector4(1, 0, 0, ox),
                                        new Vector4(0, 0, 1, oy),
                                        new Vector4(0, -1, 0, oz),
                                        new Vector4(0, 0, 0, 1));
            */

            Matrix4x4 T = new Matrix4x4(new Vector4(-1, 0, 0, ox),
                                        new Vector4(0, 1, 0, oy),
                                        new Vector4(0, 0, -1, oz),
                                        new Vector4(0, 0, 0, 1));

            // Rotation matrix of the ik point
            Matrix4x4 R = new Matrix4x4(new Vector4(T[0, 0], T[1, 0], T[2, 0], 0),
                                        new Vector4(T[0, 1], T[1, 1], T[2, 1], 0),
                                        new Vector4(T[0, 2], T[1, 2], T[2, 2], 0),
                                        new Vector4(0, 0, 0, 1));

            Vector3 o = new Vector3(ox, oy, oz); // center point calculation
            float xc = o.x - L6 * R[2, 0];
            float yc = o.y - L6 * R[2, 1];
            float zc = o.z - L6 * R[2, 2];
            // calculate thetha1
            thetha1 = Mathf.Atan2(yc, xc) * Mathf.Rad2Deg;

            // calculate thetha3
            float a = Mathf.Sqrt(L4 * L4 + L5 * L5);
            float r = Mathf.Sqrt(xc * xc + yc * yc) - L1;
            float s = zc - L2;
            float b = Mathf.Sqrt(r * r + s * s);
            float D = (L3 * L3 + a * a - b * b) / (2 * L3 * a);
            float fi = Mathf.Acos(D) * Mathf.Rad2Deg;
            float beta = 180 - fi;
            float alpha = Mathf.Atan2(L4, L5) * Mathf.Rad2Deg;
            // thetha3 = beta - alpha + 90; // Config I
            thetha3 = -(beta + alpha) + 90; // Config II

            // calculate thetha2
            float fi2 = Mathf.Atan2(s, r) * Mathf.Rad2Deg;
            float D1 = (L3 * L3 + b * b - a * a) / (2 * L3 * b);
            float fi1 = Mathf.Acos(D1) * Mathf.Rad2Deg;
            // thetha2 = fi2 - fi1; // Config I
            thetha2 = fi2 + fi1; // Config II

            // calculate thetha4, thetha5, thetha6

            float alpha1 = 90, alpha2 = 0, alpha3 = 90, r1 = L1, r2 = L3, r3 = L4, d1 = L2, d2 = 0, d3 = 0;
            // matrix T1
            Matrix4x4 T1 = new Matrix4x4(new Vector4(Mathf.Cos(thetha1 * Mathf.Deg2Rad), -Mathf.Sin(thetha1 * Mathf.Deg2Rad) * Mathf.Cos(alpha1 * Mathf.Deg2Rad), Mathf.Sin(thetha1 * Mathf.Deg2Rad) * Mathf.Sin(alpha1 * Mathf.Deg2Rad), r1 * Mathf.Cos(thetha1 * Mathf.Deg2Rad)),
                                        new Vector4(Mathf.Sin(thetha1 * Mathf.Deg2Rad), Mathf.Cos(thetha1 * Mathf.Deg2Rad) * Mathf.Cos(alpha1 * Mathf.Deg2Rad), -Mathf.Cos(thetha1 * Mathf.Deg2Rad) * Mathf.Sin(alpha1 * Mathf.Deg2Rad), r1 * Mathf.Sin(thetha1 * Mathf.Deg2Rad)),
                                        new Vector4(0, Mathf.Sin(alpha1 * Mathf.Deg2Rad), Mathf.Cos(alpha1 * Mathf.Deg2Rad), d1),
                                        new Vector4(0, 0, 0, 1));

            // matrix T2
            Matrix4x4 T2 = new Matrix4x4(new Vector4(Mathf.Cos(thetha2 * Mathf.Deg2Rad), -Mathf.Sin(thetha2 * Mathf.Deg2Rad) * Mathf.Cos(alpha2 * Mathf.Deg2Rad), Mathf.Sin(thetha2 * Mathf.Deg2Rad) * Mathf.Sin(alpha2 * Mathf.Deg2Rad), r2 * Mathf.Cos(thetha2 * Mathf.Deg2Rad)),
                                        new Vector4(Mathf.Sin(thetha2 * Mathf.Deg2Rad), Mathf.Cos(thetha2 * Mathf.Deg2Rad) * Mathf.Cos(alpha2 * Mathf.Deg2Rad), -Mathf.Cos(thetha2 * Mathf.Deg2Rad) * Mathf.Sin(alpha2 * Mathf.Deg2Rad), r2 * Mathf.Sin(thetha2 * Mathf.Deg2Rad)),
                                        new Vector4(0, Mathf.Sin(alpha2 * Mathf.Deg2Rad), Mathf.Cos(alpha2 * Mathf.Deg2Rad), d2),
                                        new Vector4(0, 0, 0, 1));

            // matrix T3
            Matrix4x4 T3 = new Matrix4x4(new Vector4(Mathf.Cos(thetha3 * Mathf.Deg2Rad), -Mathf.Sin(thetha3 * Mathf.Deg2Rad) * Mathf.Cos(alpha3 * Mathf.Deg2Rad), Mathf.Sin(thetha3 * Mathf.Deg2Rad) * Mathf.Sin(alpha3 * Mathf.Deg2Rad), r3 * Mathf.Cos(thetha3 * Mathf.Deg2Rad)),
                                        new Vector4(Mathf.Sin(thetha3 * Mathf.Deg2Rad), Mathf.Cos(thetha3 * Mathf.Deg2Rad) * Mathf.Cos(alpha3 * Mathf.Deg2Rad), -Mathf.Cos(thetha3 * Mathf.Deg2Rad) * Mathf.Sin(alpha3 * Mathf.Deg2Rad), r3 * Mathf.Sin(thetha3 * Mathf.Deg2Rad)),
                                        new Vector4(0, Mathf.Sin(alpha3 * Mathf.Deg2Rad), Mathf.Cos(alpha3 * Mathf.Deg2Rad), d3),
                                        new Vector4(0, 0, 0, 1));

            // matrix T03
            Matrix4x4 T03 = T1.transpose * T2.transpose * T3.transpose;

            Matrix4x4 R03 = new Matrix4x4(T03.GetColumn(0), T03.GetColumn(1), T03.GetColumn(2), Vector4.zero);

            // matrix R03T
            Matrix4x4 R03T = R03.transpose;

            // matrix R36
            Matrix4x4 R36 = R03T * R.transpose;

            // calculate thetha4
            thetha4 = Mathf.Atan2(-R36[1, 2], -R36[0, 2]) * Mathf.Rad2Deg;

            // calculate thetha5
            thetha5 = Mathf.Acos(-R36[2, 2]) * Mathf.Rad2Deg - 180;

            // calculate thetha6
            thetha6 = Mathf.Atan2(-R36[2, 1], R36[2, 0]) * Mathf.Rad2Deg;

            thetha3 = -thetha3 + 90;
            if (checkRotate())
            {
                if (!realtime)
                {
                    addPointsList();
                }
                
                print("End");
                time.Stop();
                print("ВРЕМЯ АЛГОРИТМА:" + time.ElapsedTicks);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Выход за пределы расчетов!");

            }
        }
        catch(Exception e)
        {
             UnityEngine.Debug.LogError("Ошибка в расчетах ИК!"+e);
        }
        
         
    }

    bool checkRotate()
    {
        if (float.IsNaN(thetha1) || float.IsNaN(thetha2) || float.IsNaN(thetha3)|| float.IsNaN(thetha4) ||
        float.IsNaN(thetha5) || float.IsNaN(thetha6))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
   
    public void RobotStop() // stop the robot
    {
        RobotOperate = false;
    }

    private IEnumerator ModifyRobot(List<float?[]> points)
    {
        move = true;
        RobotTest robotIns = configRobot.GetComponent<RobotTest>();
        foreach (float?[] p in points)
        {
            if (RobotOperate)
            {
                robotIns.J1Angle = p[0] ?? robotIns.J1Angle;
                robotIns.J2Angle = p[1] ?? robotIns.J2Angle;
                robotIns.J3Angle = p[2] ?? robotIns.J3Angle;
                robotIns.J4Angle = p[3] ?? robotIns.J4Angle;
                robotIns.J5Angle = p[4] ?? robotIns.J5Angle;
                robotIns.J6Angle = p[5] ?? robotIns.J6Angle;
                yield return new WaitForSeconds(0);
            }

        }
        move = false;
    }
    void ModifyRobotRealtime()
    {
        RobotTest robotIns = configRobot.GetComponent<RobotTest>();
        robotIns.J1Angle = thetha1;
        robotIns.J2Angle = thetha2;
        robotIns.J3Angle = thetha3;
        robotIns.J4Angle = thetha4;
        robotIns.J5Angle = thetha5;
        robotIns.J6Angle = thetha6;
    }
    float?[] oldPoints = new float?[6];
    void addPointsList()
    {
        float?[] angles = { null, null, null, null, null, null };
        if (thetha1 >= -175 & thetha1 <= 175)
        {
            angles[0] = thetha1;
            oldPoints[0] = thetha1;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ОГР А1");
        }
        if (thetha2 >= 20 && thetha2 <= 140)
        {
            angles[1] = thetha2;
            oldPoints[1] = thetha2;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ОГР А2");
        }
        if (thetha3 <= 140 && thetha3 >= 20)
        {
            angles[2] = thetha3;
            oldPoints[2] = thetha3;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ОГР А3");
        }
        if (thetha4 >= -180 && thetha4 <= 180)
        {
            angles[3] = thetha4;
            oldPoints[3] = thetha4;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ОГР А4");
        }
        if (thetha5 >= -105 && thetha5 <= 105)
        {
            angles[4] = thetha5;
            oldPoints[4] = thetha5;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ОГР А5");
        }
        if (thetha6 >= -360 && thetha6 <= 360)
        {
            angles[5] = thetha6;
            oldPoints[5] = thetha6;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ОГР А6");
        }
        points.Add(angles);
    }

}
