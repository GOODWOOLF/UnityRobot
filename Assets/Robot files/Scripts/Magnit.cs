using System.Linq;
using UnityEngine;

public class Magnit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float magnitF = 10f;
    public float magnitR = 0.1f;
    public string[] magneticTags = { "Magnetic" };
    public bool magnitOn = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (magnitOn)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, magnitR);
            foreach (Collider col in colliders)
            {
                if (magneticTags.Contains(col.gameObject.tag))
                {
                    Vector3 direction = (transform.position - col.gameObject.transform.position).normalized;
                    col.GetComponent<Rigidbody>().AddForce(direction * magnitF);
                }
            }
        }
    }
}
