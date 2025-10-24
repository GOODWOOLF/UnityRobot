using UnityEngine;

public class DetailScanner : MonoBehaviour
{
    [SerializeField]
    private GameObject _robotLogic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        var logicScript = _robotLogic.GetComponent<RobotLogic>();
        print("бунд б йнкюидеп");
        if (!logicScript.IsWorking)
            logicScript.StartWork();
    }

}
