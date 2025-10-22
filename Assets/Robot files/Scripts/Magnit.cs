using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magnit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float magnitF = 10f;
    public float magnitR = 0.1f;
    public string[] magneticTags = { "Magnetic" };
    public bool magnitOn = false;
    private bool isObjectMagnet = false;
    private List<GameObject> magnited = new();
    void Start()
    {
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
                    
                    // Получаем ближайшую точку на коллайдере магнита
                    Vector3 closestPoint = GetComponent<Collider>().ClosestPoint(col.transform.position);
                    
                    // Направление к ближайшей точке на магните
                    Vector3 direction = (closestPoint - rb.transform.position).normalized;
                    
                    rb.AddForce(direction * magnitF, ForceMode.Force);
                }
            }
        }
        else if (isObjectMagnet)
        {
            foreach (GameObject obj in magnited)
            {
                obj.transform.parent.SetParent(null);
                obj.GetComponent<Rigidbody>().isKinematic = false;
            }
            magnited.Clear();
            isObjectMagnet = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (magnitOn)
        {
            var rb = collision.rigidbody;
            rb.transform.parent.SetParent(transform);
            magnited.Add(collision.gameObject);
            rb.isKinematic = true;
            isObjectMagnet = true;
        }

    }

     public Color magnetColor = new Color(0, 1, 1, 0.3f);
    
    void OnDrawGizmos()
    {
        if (magnitOn)
        {
            // Рисуем сферу радиуса магнита
            Gizmos.color = magnetColor;
            Gizmos.DrawWireSphere(transform.position, magnitR);
            
            // Дополнительно заливаем прозрачной сферой
            Gizmos.color = new Color(magnetColor.r, magnetColor.g, magnetColor.b, 0.1f);
            Gizmos.DrawSphere(transform.position, magnitR);
        }
    }
}