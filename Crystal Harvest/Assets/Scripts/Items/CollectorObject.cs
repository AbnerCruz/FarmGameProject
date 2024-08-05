using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="CollectorObject", menuName="Items/New Collector")]
public class CollectorObject : Item
{
    public int collector_level;
    public float collector_timer;
    public float collector_initial_timer;
}
