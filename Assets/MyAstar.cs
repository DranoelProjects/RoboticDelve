using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MyAstar : MonoBehaviour
{
    AstarData m_ASdata;
    GridGraph m_ASgg;

    // Start is called before the first frame update
    void Start()
    {
        m_ASdata = AstarPath.active.data;
        m_ASgg = m_ASdata.AddGraph(typeof(GridGraph)) as GridGraph;
        int width = 100;
        int depth = 100;
        float nodeSize = 1;
        m_ASgg.center = new Vector3(0, 0, 0);
        m_ASgg.SetDimensions(width, depth, nodeSize);
        AstarPath.active.Scan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
