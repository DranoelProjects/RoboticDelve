using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text.RegularExpressions;
using Pathfinding;

public class MapManager : MonoBehaviour
{
    public Tilemap m_background, m_walls, m_corner1, m_corner2, m_corner3, m_corner4;
    public GameObject[] m_ressources;
    public GameObject m_ressourcesHolder;
    public List<TileBase> m_tileHolder;
    public int m_oMapWidth, m_oMapHeight, m_smoothStep;
    private string[][] m_genMap;
    private int[,] m_metaMap;
    private AstarData m_ASdata;
    private GridGraph m_ASgg;

    // Start is called before the first frame update
    void Start()
    {
        //Generation
        Application.targetFrameRate = 60;
        Intermediaire myWFC = new Intermediaire(m_oMapHeight, m_oMapWidth);
        m_metaMap = myWFC.startProcess();
        //m_metaMap = new int[,]{ };
        updateTileMap();

        //Ressources
        spawnResources();

        //Pathfinding
        m_ASdata = AstarPath.active.data;
        m_ASgg = m_ASdata.gridGraph;
        int width = m_oMapWidth;
        int depth = m_oMapHeight;
        float nodeSize = 1;
        m_ASgg.center = new Vector3(0, 0, 0);
        m_ASgg.SetDimensions(width, depth, nodeSize);
        StartCoroutine(rescan());
        AstarPath.active.logPathResults = PathLog.None;
    }

    IEnumerator rescan()
    {
        yield return new WaitForSeconds(2);
        AstarPath.active.Scan(m_ASgg);
    }

    int[,] generationDuBled(int mapSize)
    {
        int[,] map = new int[mapSize, mapSize];
        int[,] temp = map;
        for (int i = 0; i < mapSize; i++)
        {
            map[0, i] = 1;
            map[i, 0] = 1;
            map[mapSize - 1, i] = 1;
            map[i, mapSize - 1] = 1;

        }
        for (int i = 1; i < mapSize - 1; i++)
        {
            for (int j = 1; j < mapSize - 1; j++)
            {
                map[i, j] = Random.Range(0, 2);
            }
        }
        int val = 0;
        for (int k = 0; k < m_smoothStep; k++)
        {
            for (int i = 1; i < mapSize - 1; i++)
            {
                for (int j = 1; j < mapSize - 1; j++)
                {
                    val = map[i - 1, j - 1]
                        + map[i, j - 1]
                        + map[i + 1, j - 1]
                        + map[i - 1, j]
                        + map[i, j]
                        + map[i + 1, j]
                        + map[i - 1, j + 1]
                        + map[i, j + 1]
                        + map[i + 1, j + 1];
                    val = (int)2 * val / 9;
                    temp[i, j] = val;
                }
            }
            map = temp;
        }
        return map;
    }

    private void updateTileMap()
    {
        for (int i = 0; i < m_oMapWidth; i++)
        {
            m_metaMap[0, i] = -1;
            m_metaMap[m_oMapHeight- 1, i] = -1;
        }
        for (int i = 0; i < m_oMapHeight; i++)
        {
            m_metaMap[i, 0] = -1;
            m_metaMap[i, m_oMapWidth - 1] = -1;
        }
            for (int y = 0; y < m_oMapHeight; y++)
        {
            for (int x = 0; x < m_oMapWidth; x++)
            {
                if (m_metaMap[y, x] == -1)
                    m_background.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[0]);
                else if (m_metaMap[y, x] == 1)
                    m_walls.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[0]);
                else
                    m_background.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[TileFromMap(x, y)]);
            }
        }
    }

    private int TileFromMap(int x, int y)
    {
        Debug.Log(x + "," + y);
        int tileToDisplay = 0;
        bool corner1 = true, corner2 = true, corner3 = true, corner4 = true;
        if (m_metaMap[y - 1, x] == 1)
        {
            tileToDisplay += 1;
            corner1 = corner2 = false;
        }
        if (m_metaMap[y, x - 1] == 1)
        {
            tileToDisplay += 2;
            corner1 = corner3 = false;
        }
        if (m_metaMap[y, x + 1] == 1)
        {
            tileToDisplay += 4;
            corner2 = corner4 = false;
        }
        if (m_metaMap[y + 1, x] == 1)
        {
            tileToDisplay += 8;
            corner3 = corner4 = false;
        }

        if (m_metaMap[y - 1, x - 1] != 1)
            corner1 = false;
        if (m_metaMap[y - 1, x + 1] != 1)
            corner2 = false;
        if (m_metaMap[y + 1, x - 1] != 1)
            corner3 = false;
        if (m_metaMap[y + 1, x + 1] != 1)
            corner4 = false;
        if (corner1)
            m_corner1.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[18]);
        if (corner2)
            m_corner2.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[19]);
        if (corner3)
            m_corner3.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[20]);
        if (corner4)
            m_corner4.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[21]);
        return tileToDisplay;
    }

    private void spawnResources()
    {
        int x = 0, y = 0;
        for (int i = 0; i < m_ressources.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                x = Random.Range(0, m_oMapWidth - 1);
                y = Random.Range(0, m_oMapHeight - 1);
                Debug.Log(x + "," + y);
                while (m_metaMap[y, x] == 1)
                {
                    Debug.Log(x + "," + y);
                    x = Random.Range(0, m_oMapWidth - 1);
                    y = Random.Range(0, m_oMapHeight - 1);
                }
                GameObject ressource = Instantiate(m_ressources[i], new Vector3(x - (int)m_oMapWidth / 2 + 0.5f, y - (int)m_oMapHeight / 2 + 0.5f, 0), Quaternion.identity);
                ressource.transform.parent = m_ressourcesHolder.transform;
            }
        }
    }

    private void loadTest()
    {
        m_genMap = new string[m_oMapWidth][];
        for (int i = 0; i < m_oMapWidth; i++)
        {
            m_genMap[i] = new string[m_oMapHeight];
            for (int j = 0; j < m_oMapHeight; j++)
            {
                m_genMap[i][j] = "0123456789";
            }
        }


    }
}
