using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Colector", menuName="ColectorObject")]
public class CollectorObject : Item
{
    public int collector_level;
    public float collector_timer;
    public float collector_initial_timer;
}
