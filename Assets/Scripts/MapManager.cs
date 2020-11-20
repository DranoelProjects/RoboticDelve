using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class MapManager : MonoBehaviour
{
    public Tilemap m_background, m_walls;
    public List<TileBase> m_tileHolder;
    public int m_genmapWidth, m_genmapHeight, m_mapWidth, m_mapHeight;
    private string[][] m_genMap;
    private int[,] m_metaMap;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        m_metaMap = new int[,] {    { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    { -1,  1,  0,  0,  0,  0,  0,  1,  1, -1},
                                    { -1,  1,  0,  0,  1,  1,  0,  0,  0, -1},
                                    { -1,  1,  0,  0,  1,  1,  0,  0,  0, -1},
                                    { -1,  1,  1,  1,  0,  0,  0,  0,  0, -1},
                                    { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};
        updateTileMap();
    }

    private void updateTileMap()
    {
        for (int x = 0; x < m_mapWidth; x++)
        {
            for (int y = 0; y < m_mapHeight; y++)
            {
                if (m_metaMap[y, x] == -1)
                    m_background.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_mapWidth / 2), Mathf.FloorToInt(-y + m_mapHeight / 2), 0), m_tileHolder[0]);
                else if (m_metaMap[y, x] == 1)
                    m_walls.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_mapWidth / 2), Mathf.FloorToInt(-y + m_mapHeight / 2), 0), m_tileHolder[0]);
                else
                    m_background.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_mapWidth / 2), Mathf.FloorToInt(-y + m_mapHeight / 2), 0), m_tileHolder[TileFromMap(x, y)]);
            }
        }
    }

    private int TileFromMap(int x, int y)
    {
        int tileToDisplay = 0;
        if (m_metaMap[y - 1, x] == 1)
            tileToDisplay += 1;
        if (m_metaMap[y, x - 1] == 1)
            tileToDisplay += 2;
        if (m_metaMap[y, x + 1] == 1)
            tileToDisplay += 4;
        if (m_metaMap[y + 1, x] == 1)
            tileToDisplay += 8;
        if (tileToDisplay == 0)
        {
            if (m_metaMap[y - 1, x - 1] == 1)
                tileToDisplay += 1;
            if (m_metaMap[y - 1, x + 1] == 1)
                tileToDisplay += 2;
            if (m_metaMap[y + 1, x - 1] == 1)
                tileToDisplay += 4;
            if (m_metaMap[y + 1, x + 1] == 1)
                tileToDisplay += 8;
            if (tileToDisplay != 0)
                tileToDisplay += 16;
        }
        if (tileToDisplay != 0)
            return tileToDisplay;
        else
            return 0;
    }

    private void loadTest()
    {
        m_genMap = new string[m_genmapWidth][];
        for (int i = 0; i < m_genmapWidth; i++)
        {
            m_genMap[i] = new string[m_genmapHeight];
            for (int j = 0; j < m_genmapHeight; j++)
            {
                m_genMap[i][j] = "0123456789";
            }
        }

        
    }
}
