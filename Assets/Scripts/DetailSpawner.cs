using System.Collections;
using UnityEngine;

public class DetailSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private float _spawnInterval = 2f;
    [SerializeField]
    private int _maxPrefab = 10;
    private int spawnedCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       StartCoroutine(SpawnRoutine());
        
    }
    IEnumerator SpawnRoutine()
    {
        if (_prefab != null)
        {
            while (spawnedCount < _maxPrefab)
            {
                Instantiate(_prefab, transform.position, transform.rotation);
                spawnedCount++;
                yield return new WaitForSeconds(_spawnInterval);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
