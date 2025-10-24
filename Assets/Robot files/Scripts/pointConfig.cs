using UnityEngine;

public class pointConfig : MonoBehaviour
{
    //к точке
    public float step = 100;
    public float time = 0;

    //магнит
    public MagnitS magnitStatus = MagnitS.NotControl;

    //в точке
    public float delay = 0;

}
public enum MagnitS
{
    On,
    Off,
    NotControl
}
