using System.Threading;
using UnityEngine;

public class conveer : MonoBehaviour
{
    public float speed = 1;
    public float coef = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        Vector3 pos = rb.position;
        rb.position += Vector3.back * speed*Time.fixedDeltaTime;
        gameObject.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(1,0) * coef * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
        
        
    }

    /*void OnCollisionStay(Collision collision)
    {
        collision.rigidbody.linearVelocity = Vector3.forward * speed;
    }*/
}
