using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text.RegularExpressions;
using Pathfinding;
using System;

public class MapManager : MonoBehaviour
{
    public Tilemap m_background, m_walls, m_corner1, m_corner2, m_corner3, m_corner4;
    public GameObject[] m_ressources, m_enemies;
    public GameObject m_ressourcesHolder, m_enemiesHolder, m_player, m_exit;
    public List<TileBase> m_tileHolder;
    public int m_oMapWidth, m_oMapHeight, m_smoothStep, m_lvl, m_enmLvl;
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
        m_lvl = PlayerPrefs.GetInt("Level");
        m_metaMap = myWFC.startProcess(m_lvl);
        int m_enmLvl = m_lvl;
        if (m_enmLvl >= 3)
            m_enmLvl--;
        if (m_enmLvl > m_enemies.Length)
            m_enmLvl = m_enemies.Length;
        updateTileMap();

        //Add exit and spawn
        exitAndSpawn();

        //Ressources
        //Version finale a faire
        spawnResources();

        //Ennemis
        spawnEnemies();

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

    private void spawnEnemies()
    {
        int myRng;
        int x = UnityEngine.Random.Range(0, m_oMapWidth - 1);
        int y = UnityEngine.Random.Range(0, m_oMapHeight - 1);
        for (int i = 0; i < 100 * m_lvl; i++)
        {
            x = UnityEngine.Random.Range(0, m_oMapWidth - 1);
            y = UnityEngine.Random.Range(0, m_oMapHeight - 1);
            while (m_metaMap[y, x] == 1)
            {
                x = UnityEngine.Random.Range(0, m_oMapWidth - 1);
                y = UnityEngine.Random.Range(0, m_oMapHeight - 1);
            }
            int dist = (int) (Math.Pow((x - (int)m_player.transform.position.x), 2) + Math.Pow((y - (int)m_player.transform.position.y), 2));
            if (dist < 100)
            {
                continue;
            }
            if (dist < 400)
            {
                myRng = UnityEngine.Random.Range(0, 1);
                if (myRng == 1)
                    continue;
            }
            int selectedE = UnityEngine.Random.Range(0, m_enmLvl);
            GameObject enemy = Instantiate(m_enemies[selectedE], new Vector3(x - (int)m_oMapWidth / 2 + 0.5f, y - (int)m_oMapHeight / 2 + 0.5f, 0), Quaternion.identity);
            enemy.transform.parent = m_enemiesHolder.transform;
            if (UnityEngine.Random.Range(0, 4) == 4)
            {
                int nbItem = m_ressources.Length;
                int index = 0;
                myRng = UnityEngine.Random.Range(0, nbItem*nbItem);
                for (int itemID = 1; itemID < nbItem; itemID++)
                {
                    if (myRng <= nbItem*nbItem)
                    {
                        index = itemID;
                        break;
                    }
                }
                EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
                enemyScript.setDoDrop(true);
                enemyScript.setDropItem(m_ressources[index]);
            }
        }
    }

    private void exitAndSpawn()
    {
        int quadX = UnityEngine.Random.Range(0, 1);
        int quadY = UnityEngine.Random.Range(0, 1);
        int x = UnityEngine.Random.Range(0, m_oMapWidth / 2 - 1);
        int y = UnityEngine.Random.Range(0, m_oMapHeight / 2 - 1);
        while (m_metaMap[y + (m_oMapHeight / 2) * quadY, x + (m_oMapWidth / 2) * quadX] == 1)
        {
            x = UnityEngine.Random.Range(0, m_oMapWidth / 2 - 1);
            y = UnityEngine.Random.Range(0, m_oMapHeight / 2 - 1);
        }
        m_player.transform.SetPositionAndRotation(new Vector3(x + (m_oMapWidth / 2) * (quadX - 1), y + (m_oMapHeight / 2) * (quadY - 1), 0), Quaternion.identity);
        Debug.Log(x + (m_oMapWidth / 2) * quadX + "," + y + (m_oMapHeight / 2) * quadY);
        quadX = (quadX + 1) % 2;
        quadY = (quadY + 1) % 2;
        x = UnityEngine.Random.Range(0, m_oMapWidth / 2 - 1);
        y = UnityEngine.Random.Range(0, m_oMapHeight / 2 - 1);
        while (m_metaMap[y + (m_oMapHeight / 2) * quadY, x + (m_oMapWidth / 2) * quadX] == 1)
        {
            x = UnityEngine.Random.Range(0, m_oMapWidth / 2 - 1);
            y = UnityEngine.Random.Range(0, m_oMapHeight / 2 - 1);
        }
        GameObject sortie = Instantiate(m_exit, new Vector3(x + (int) (m_oMapWidth / 2) * (quadX - 1) + 0.5f, y + (int) (m_oMapHeight / 2) * (quadY - 1) + 0.5f, 0), Quaternion.identity);
        sortie.transform.parent = this.transform;
        if (m_lvl % 3 == 0)
        {
            GameObject theBoss = Instantiate(m_enemies[m_lvl], new Vector3(x + (int)(m_oMapWidth / 2) * (quadX - 1) + 0.5f, y + (int)(m_oMapHeight / 2) * (quadY - 1) + 0.5f, 0), Quaternion.identity);
            theBoss.transform.parent = m_enemiesHolder.transform;
            EnemyAI bossScript = theBoss.GetComponent<EnemyAI>();
            bossScript.setDoDrop(true);
            bossScript.setDropItem(m_ressources[0]);
        }
        else
        {
            int quadK = UnityEngine.Random.Range(0, 1);
            quadX = (quadX + 1 + quadK) % 2;
            quadY = (quadY + quadK) % 2;
            x = UnityEngine.Random.Range(0, m_oMapWidth / 2 - 1);
            y = UnityEngine.Random.Range(0, m_oMapHeight / 2 - 1);
            while (m_metaMap[y + (m_oMapHeight / 2) * quadY, x + (m_oMapWidth / 2) * quadX] == 1)
            {
                x = UnityEngine.Random.Range(0, m_oMapWidth / 2 - 1);
                y = UnityEngine.Random.Range(0, m_oMapHeight / 2 - 1);
            }
            GameObject key = Instantiate(m_ressources[0], new Vector3(x + (int)(m_oMapWidth / 2) * (quadX - 1) + 0.5f, y + (int)(m_oMapHeight / 2) * (quadY - 1) + 0.5f, 0), Quaternion.identity);
            key.transform.parent = m_ressourcesHolder.transform;
        }
    }

    IEnumerator rescan()
    {
        yield return new WaitForSeconds(2);
        AstarPath.active.Scan(m_ASgg);
    }

    private void updateTileMap()
    {
        for (int i = 0; i < m_oMapWidth; i++)
        {
            m_metaMap[0, i] = 1;
            m_metaMap[m_oMapHeight- 1, i] = 1;
        }
        for (int i = 0; i < m_oMapHeight; i++)
        {
            m_metaMap[i, 0] = 1;
            m_metaMap[i, m_oMapWidth - 1] = 1;
        }
            for (int y = 0; y < m_oMapHeight; y++)
        {
            for (int x = 0; x < m_oMapWidth; x++)
            {
                //if (m_metaMap[y, x] == -1)
                //    m_background.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[0]);
                //else
                if (m_metaMap[y, x] == 1)
                    m_walls.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[0]);
                else
                    m_background.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[TileFromMap(x, y)]);
            }
        }
    }

    private int TileFromMap(int x, int y)
    {
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
            m_corner1.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[17]);
        if (corner2)
            m_corner2.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[18]);
        if (corner3)
            m_corner3.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[19]);
        if (corner4)
            m_corner4.SetTile(new Vector3Int(Mathf.FloorToInt(x - m_oMapWidth / 2), Mathf.FloorToInt(-y + m_oMapHeight / 2), 0), m_tileHolder[20]);
        return tileToDisplay;
    }

    private void spawnResources()
    {
        int x = 0, y = 0;
        for (int i = 1; i < m_ressources.Length; i++)
        {
            int amount = Math.Max(i * i / 4, i);
            for (int j = 0; j < amount; j++)
            {
                x = UnityEngine.Random.Range(0, m_oMapWidth - 1);
                y = UnityEngine.Random.Range(0, m_oMapHeight - 1);
                while (m_metaMap[y, x] == 1)
                {
                    x = UnityEngine.Random.Range(0, m_oMapWidth - 1);
                    y = UnityEngine.Random.Range(0, m_oMapHeight - 1);
                }
                GameObject ressource = Instantiate(m_ressources[i], new Vector3(x - (int)m_oMapWidth / 2 + 0.5f, y - (int)m_oMapHeight / 2 + 0.5f, 0), Quaternion.identity);
                ressource.transform.parent = m_ressourcesHolder.transform;
            }
        }
    }
}
