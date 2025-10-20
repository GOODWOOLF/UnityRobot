using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotTest : MonoBehaviour
{
    public GameObject J1;
    public float J1Angle;
    public GameObject J2;
    public float J2Angle;
    public GameObject J3;
    public float J3Angle;
    public GameObject J4;
    public float J4Angle;
    public GameObject J5;
    public float J5Angle;
    public GameObject J6;
    public float J6Angle;

    public GameObject EndEffector, BaseOrigin;

    public InputField J1string, J2string, J3string, J4string, J5string, J6string, Xstring, Ystring, Zstring;
    float y;

    // Start is called before the first frame update
    void Start()
    {
        y = J1Angle / 50;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {



       // StartCoroutine(ky());
        J1.transform.localRotation = Quaternion.AngleAxis(J1Angle + J2.transform.rotation.x, new Vector3(0, 1, 0));
        J2.transform.localRotation = Quaternion.AngleAxis(-J2Angle + J2.transform.rotation.x, new Vector3(1, 0, 0));
        J3.transform.localRotation = Quaternion.AngleAxis(J3Angle + J3.transform.rotation.x, new Vector3(1, 0, 0));
        J4.transform.localRotation = Quaternion.AngleAxis(J4Angle + J4.transform.rotation.z, new Vector3(0, 0, 1));
        J5.transform.localRotation = Quaternion.AngleAxis(-J5Angle + J5.transform.rotation.x, new Vector3(1, 0, 0));
        J6.transform.localRotation = Quaternion.AngleAxis(J6Angle + J6.transform.rotation.z, new Vector3(0, 0, 1));
        Physics.SyncTransforms();
        /*J1.GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(J1Angle, J1.GetComponentInParent<Transform>().TransformDirection(new Vector3(0,1,0))));
        J2.GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(-J2Angle,J2.GetComponentInParent<Transform>().TransformDirection( new Vector3(1, 0, 0))));
        J3.GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(J3Angle, J3.GetComponentInParent<Transform>().TransformDirection(new Vector3(1, 0, 0))));
        J4.GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(J4Angle, J4.GetComponentInParent<Transform>().TransformDirection(new Vector3(0, 0, 1))));
        J5.GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(-J5Angle, J5.GetComponentInParent<Transform>().TransformDirection(new Vector3(1, 0, 0))));
        J6.GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(J6Angle, J6.GetComponentInParent<Transform>().TransformDirection(new Vector3(0, 0, 1))));
*/

        /*J1string.text = Mathf.Round(-J1Angle).ToString();
        J2string.text = Mathf.Round(-J2Angle).ToString();
        J3string.text = Mathf.Round(J3Angle).ToString();
        J4string.text = Mathf.Round(J4Angle).ToString();
        J5string.text = Mathf.Round(J5Angle).ToString();
        J6string.text = Mathf.Round(-J6Angle).ToString();

        // EE position
        Xstring.text = ((EndEffector.transform.position.z - BaseOrigin.transform.position.z) * 1000).ToString();
        Ystring.text = ((EndEffector.transform.position.x - BaseOrigin.transform.position.x) * 1000).ToString();
        Zstring.text = ((EndEffector.transform.position.y - BaseOrigin.transform.position.y) * 1000).ToString();*/
    }
    IEnumerator ky()
    {
        float ygol = -175;
        for (int i = 0; i < 50; i++)
        {
            ygol += y;
            J1.transform.localRotation = Quaternion.AngleAxis( y, new Vector3(0, 1, 0));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
