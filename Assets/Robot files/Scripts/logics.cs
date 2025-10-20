using System.Collections;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class logics : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public GameObject magnit_1;
    public GameObject magnit_2;
    public GameObject Base;
    public Vector3[] points = { new Vector3(0.187999994f, 0.252999991f, -0.708000004f),
    new Vector3(0.187999994f,0.490999997f,-0.708000004f),
    new Vector3(-0.984000027f,0.565999985f,0.317999989f),
    new Vector3(-0.984000027f,0.252999991f,0.317999989f)
    };
    private Magnit magnit1;
    private Magnit magnit2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magnit1 = magnit_1.GetComponent<Magnit>();
         magnit2 = magnit_2.GetComponent<Magnit>();
        StartCoroutine(act());
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator act()
    {
        while (true)
        {
            point1.transform.position = Base.transform.position + points[0];
        yield return new WaitForSeconds(3);
        magnit1.magnitOn = true;
        yield return new WaitForSeconds(2);
        point1.transform.position = Base.transform.position + points[1];
        yield return new WaitForSeconds(2);
        point1.transform.position = Base.transform.position + points[3];
        yield return new WaitForSeconds(4);
        magnit1.magnitOn = false;
        yield return new WaitForSeconds(0.5f);
        point1.transform.position = Base.transform.position + new Vector3(0.238000005f, 1.15499997f, -0.621999979f);

        point2.transform.position = Base.transform.position + points[3];
        yield return new WaitForSeconds(3);
        magnit2.magnitOn = true;
        yield return new WaitForSeconds(2);
        point2.transform.position = Base.transform.position + points[2];
         yield return new WaitForSeconds(2);
        point2.transform.position = Base.transform.position + points[0];
        yield return new WaitForSeconds(4);
        magnit2.magnitOn = false;
        yield return new WaitForSeconds(0.5f);
        point2.transform.position = Base.transform.position + new Vector3(-1.04299998f,1.15499997f,-0.0829999968f);
        }
        
    }
}
