using System.Linq;
using UnityEngine;

public class Magnit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float magnitF = 10f;
    public float magnitR = 0.1f;
    public string[] magneticTags = { "Magnetic" };
    public bool magnitOn = false;
    private MeshRenderer renderer;
    private bool isObjectMagnet;
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (magnitOn)
        {
            if (isObjectMagnet) return;
            Collider[] colliders = Physics.OverlapSphere(transform.position, magnitR);
            foreach (Collider col in colliders)
            {
                if (magneticTags.Contains(col.gameObject.tag))
                {
                    var rb = col.GetComponent<Rigidbody>();
                    rb.linearVelocity = (transform.position - (rb.transform.position + rb.centerOfMass)) * magnitF* Time.fixedDeltaTime;
                    //Vector3 direction = (transform.position - col.gameObject.transform.position).normalized;
                    //.AddForce(direction * magnitF);
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        isObjectMagnet = true;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (isObjectMagnet && magnitOn)
        {
            var rb = collision.rigidbody;
            
            rb.transform.parent = transform;
            rb.useGravity = false;
            //print(transform.position.y + " | "  + size.y + " = " + (transform.position.y - size.y));
            //rb.position = Vector3.MoveTowards(transform.position.x, transform.position.y - size.y, transform.position.z);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        var rb = collision.rigidbody;
        isObjectMagnet = false;
        rb.useGravity = true;
    }
}
