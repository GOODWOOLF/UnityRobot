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
    private List<Vector3> attachmentOffsets = new();
    private Vector3 lastMagnetPosition;

    void FixedUpdate()
    {
        if (!magnitOn)
        {
            // Размагничивание
            for (int i = magnited.Count - 1; i >= 0; i--)
            {
                var rb = magnited[i]?.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
            }

            magnited.Clear();
            attachmentOffsets.Clear();
            return;
        }

        // Притяжение объектов в радиусе
        Collider[] colliders = Physics.OverlapSphere(transform.position, magnitR);
        foreach (Collider col in colliders)
        {
            if (!magneticTags.Contains(col.gameObject.tag)) continue;
            if (magnited.Contains(col.gameObject)) continue;

            var rb = col.attachedRigidbody;
            if (rb == null) continue;

            Vector3 closestPoint = GetComponent<Collider>().ClosestPoint(col.transform.position);
            Vector3 direction = (closestPoint - rb.transform.position).normalized;
            rb.AddForce(direction * magnitF, ForceMode.Force);
        }
    }

    void LateUpdate()
    {
        // Двигаем прикреплённые объекты после всех апдейтов, без лагов
        if (magnitOn && magnited.Count > 0)
        {
            for (int i = magnited.Count - 1; i >= 0; i--)
            {
                var obj = magnited[i];
                if (obj == null)
                {
                    magnited.RemoveAt(i);
                    attachmentOffsets.RemoveAt(i);
                    continue;
                }

                obj.transform.position = transform.position + attachmentOffsets[i];
            }
        }

        lastMagnetPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!magnitOn) return;

        var rb = collision.rigidbody;
        if (rb == null) return;

        if (magnited.Contains(collision.gameObject)) return;

        // Фиксируем объект в точке контакта
        rb.isKinematic = true;
        magnited.Add(collision.gameObject);

        Vector3 contactPoint = GetContactPoint(collision.collider);
        Vector3 offset = collision.transform.position - contactPoint;
        attachmentOffsets.Add(contactPoint + offset - transform.position);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Пока магнит включен — не размагничиваем
        if (magnitOn) return;

        var rb = collision.rigidbody;
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        magnited.Remove(collision.gameObject);
        int index = attachmentOffsets.Count > 0 ? magnited.IndexOf(collision.gameObject) : -1;
        if (index >= 0) attachmentOffsets.RemoveAt(index);

        isObjectMagnet = false;
    }

    public Color magnetColor = new Color(0, 1, 1, 0.3f);

    void OnDrawGizmos()
    {
        if (magnitOn)
        {
            Gizmos.color = magnetColor;
            Gizmos.DrawWireSphere(transform.position, magnitR);
            Gizmos.color = new Color(magnetColor.r, magnetColor.g, magnetColor.b, 0.1f);
            Gizmos.DrawSphere(transform.position, magnitR);
        }
    }

    private Vector3 GetContactPoint(Collider otherCollider)
    {
        Vector3 closestPointOnMagnet = GetComponent<Collider>().ClosestPoint(otherCollider.transform.position);
        Vector3 closestPointOnObject = otherCollider.ClosestPoint(closestPointOnMagnet);
        return (closestPointOnMagnet + closestPointOnObject) * 0.5f;
    }
}