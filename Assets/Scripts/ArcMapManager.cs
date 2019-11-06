using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcMapManager : MonoBehaviour
{
    public static ArcMapManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RefreshArcMap()
    {

    }


}
