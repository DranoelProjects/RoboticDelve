<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class RessourcesSpawner : MonoBehaviour
{
    public Tilemap m_tilemap;
    public ScriptableObject m_mapScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
||||||| 3bb14a7
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class RessourcesSpawner : MonoBehaviour
{
    public GameObject[] m_ressources;
    public Tilemap m_tilemap;
    public ScriptableObject m_mapScript;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_ressources.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Random.RandomRange(0, 101);
                if (m_tilemap)
                {

                }
                Instantiate(m_ressources[i], new Vector3(0, 0, 0), Quaternion.identity);
            }
        }
    }
}
>>>>>>> Enemies
