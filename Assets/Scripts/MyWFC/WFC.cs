using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WFC : MonoBehaviour
{
    System.Random myRand;
    public Image mask;
    private int N, M, pN, pM, iN, iM, freqTot, maxTotEnt, currentEnt;
    public int oN, oM;
    private int[,] input, output, patternMap, entropyMap;
    private bool[,][] outputPattern;
    private bool goToNextScene;
    private List<Pattern> patternList;
    private List<List<int[]>> patternRef;
    private List<int> patternFreq;
    private int[,,] tilesMap;
    private int[] possibilities;
    private Thread myThread;
    private bool stopThread;

    private void Awake()
    {
        Debug.Log("autoscroll");
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        myRand = new System.Random();
        goToNextScene = false;
        stopThread = false;
        myThread = new Thread(startProcess);
        myThread.Start();
        //this.GetComponentInChildren<>
    }

    void Update()
    {
        //Go to next scene and keep the data
        if (goToNextScene)
        {
            //return output;
        }
    }

    private void OnApplicationQuit()
    {
        stopThread = true;
    }

    public void startProcess()
    {
        output = new int[oM, oN];
        N = oN;
        M = oM;

        //possible tiles
        possibilities = new int[] { 0, 1 };

        //Input to recreate
        input = new int[,]{ {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
                            {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
                            {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0},
                            {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} };

        //creation of pattern map
        iN = input.GetLength(1);
        iM = input.GetLength(0);
        Debug.Log(iN + ":" + iM);
        pN = iN;
        pM = iM;
        patternList = new List<Pattern>();
        patternFreq = new List<int>();
        patternRef = new List<List<int[]>>();
        patternMap = new int[pM, pN];
        for (int i = 0; i < iN; i++)
        {
            for (int j = 0; j < iM; j++)
            {
                //Get the pattern at the current position
                int[,] pattern = PatternAt(i, j);
                //A déplacer
                //Create a flipped version
                int[,] patternFlip = Flip(pattern, 3);
                //Record every rotation of both of them
                for (int k = 0; k < 4; k++)
                {
                    RecordPattern(pattern, i, j);
                    RecordPattern(patternFlip, i, j);
                    pattern = Rotate(pattern, 3);
                    patternFlip = Rotate(patternFlip, 3);
                }
            }
        }
        freqTot = 0;
        foreach (int freq in patternFreq)
        {
            freqTot += freq;
        }

        //Adjency rules and frequency hints
        Debug.Log(patternMap.GetLength(1) + ":" + patternMap.GetLength(0));
        foreach (Pattern pat in patternList)
        {
            //Si la version flip ou rotate n'existe pas dans la liste, les créer et mettre un bouléen vrai pour ajouter les règles correspondantes
            int index = patternList.FindIndex(p => p.getPattern() == pat.getPattern());
            foreach (int[] position in patternRef[index])
            {
                pat.addRule(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]], 0);
                pat.addRule(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]], 1);
                pat.addRule(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]], 2);
                pat.addRule(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]], 3);
                pat.addRule(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]], 4);
                pat.addRule(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]], 5);
                pat.addRule(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]], 6);
                pat.addRule(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]], 7);
            }
            pat.calcFreq();
        }
        Debug.Log(patternList.Count);

        //Map where we apply WFC
        outputPattern = new bool[oM, oN][];
        entropyMap = new int[oM, oN];
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {
                outputPattern[j, i] = new bool[patternList.Count];
                for (int k = 0; k < patternList.Count; k++)
                {
                    outputPattern[j, i][k] = true;
                }
                entropyMap[j, i] = CalculateEntropy(i, j);
            }
        }
        maxTotEnt = CalculateTotalEntropy();
        Debug.Log("Test1");
        //Apply algorithm
        GenerateMap();

        //tilesMap = new int[oM, oN, possibilities.Length];
        //for (int i = 0; i < oN; i++)
        //{
        //    output[0, i] = 1;
        //    output[oM - 1, i] = 1;
        //}
        //for (int i = 0; i < oM; i++)
        //{
        //    output[i, 0] = 1;
        //    output[i, oN - 1] = 1;
        //}
        //for (int j = 0; j < oM; j++)
        //{
        //    for (int i = 0; i < oN; i++)
        //    {

        //    }
        //}
    }

    public void GenerateMap()
    {
        Debug.Log("Test 2");
        //Choose a random starting point in case multiple tiles have the same low entropy
        int x = (int)myRand.Next(oN);
        int y = (int)myRand.Next(oM);
        //Choose tile with lowest entropy
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {
                if (entropyMap[j, i] < entropyMap[y, x] && entropyMap[j, i] > 0)
                {
                    x = i;
                    y = j;
                }
            }
        }
        int entropy = entropyMap[y, x];
        int nbPat = outputPattern[0, 0].Length;
        Debug.Log("Test 3");
        //Gen loop
        while (!isGen())
        {
            if (stopThread)
            {
                return;
            }
            Debug.Log("Test W1");
            //If the pattern is already set, retry with another tile
            if (entropyMap[y, x] == 0)
            {
                Debug.Log("Test W1Sub1");
                entropy = outputPattern[y, x].Length;
                for (int j = 0; j < oM; j++)
                {
                    for (int i = 0; i < oN; i++)
                    {
                        if (entropyMap[j, i] < entropy && entropyMap[j, i] > 0)
                        {
                            x = i;
                            y = j;
                            entropy = entropyMap[j, i];
                        }
                    }
                }
                continue;
            }
            Debug.Log("Test W2");
            int rngPat = (int)myRand.Next(entropy);
            //Set the pattern accordingly
            int index = 0;
            int trueIndex = -1;
            for (int i = 0; i < nbPat; i++)
            {
                if (outputPattern[y, x][i])
                {
                    if (index == rngPat)
                    {
                        trueIndex = i;
                    }
                    index++;
                }
            }
            for (int i = 0; i < nbPat; i++)
            {
                if (i != trueIndex)
                {
                    outputPattern[y, x][i] = false;
                }
                entropyMap[y, x] = 0;
            }
            Debug.Log("Test W3");
            //TODO propagate choice to surrounding tiles
            Propagate(x, y);
            Debug.Log("Test W4");
        }
        output = CreateFinalMap();
    }

    int CalculateTotalEntropy()
    {
        int entropy = 0;
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {
                entropy += CalculateEntropy(i, j);
            }
        }
        return entropy;
    }

    int[,] CreateFinalMap()
    {
        int[,] temp = new int[oM, oN];
        int nbPat = outputPattern[0, 0].Length;
        int index = -1;
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {
                for (int k = 0; k < nbPat; k++)
                {
                    if (outputPattern[j, i][k])
                    {
                        index = k;
                    }
                }
                if (index != -1)
                {
                    temp[j, i] = patternList[index].getTile();
                }
                else
                {
                    Debug.Log("Problème de génération inconnu");
                    temp[j, i] = 0;
                }
            }
        }
        for (int j = 0; j < oM; j++)
        {
            temp[j, 0] = 1;
            temp[j, oN - 1] = 1;
        }
        for (int i = 0; i < oN; i++)
        {
            temp[0, i] = 1;
            temp[oM - 1, i] = 1;
        }
        return temp;
    }

    void Propagate(int x, int y)
    {
        //mask.fillAmount = (float)CalculateTotalEntropy() / (float)maxTotEnt;
        Debug.Log("Entropy: " + CalculateTotalEntropy());
        bool[] modified = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            modified[i] = false;
        }
        entropyMap[y, x] = CalculateEntropy(x, y);
        if (entropyMap[y, x] == -1)
        {
            Debug.Log("Contradiction de génération");
        }
        if (entropyMap[y, x] == 0)
            return;
        //For each direction, get every possible pattern given the possible pattern at the current tile
        List<Pattern> myPL = patternList.FindAll(p => outputPattern[y, x][p.getId()] == true);
        List<int>[] valid = new List<int>[8];
        for (int dir = 0; dir < 8; dir++)
        {
            foreach (Pattern pat in myPL)
            {
                List<int> rules = pat.getRuleI(dir);
                foreach (int rule in rules)
                {
                    if (valid[dir].FindIndex(elmnt => elmnt == rule) == -1)
                    {
                        valid[dir].Add(rule);
                    }
                }
            }
        }
        int nbPat = outputPattern[0, 0].Length;
        //If the pattern isn't valid, set it to false
        for (int i = 0; i < nbPat; i++)
        {
            if (y > 0)
            {
                if (x > 0)
                {
                    if (valid[0].FindIndex(elmt => elmt == i) == -1 && outputPattern[y - 1, x - 1][i])
                    {
                        outputPattern[y - 1, x - 1][i] = false;
                        modified[0] = true;
                    }
                }
                if (valid[1].FindIndex(elmt => elmt == i) == -1 && outputPattern[y - 1, x][i])
                {
                    outputPattern[y - 1, x][i] = false;
                    modified[1] = true;
                }
                if (x < oN - 1)
                {
                    if (valid[2].FindIndex(elmt => elmt == i) == -1 && outputPattern[y - 1, x + 1][i])
                    {
                        outputPattern[y - 1, x + 1][i] = false;
                        modified[2] = true;
                    }
                }
            }
            if (x > 0)
            {
                if (valid[3].FindIndex(elmt => elmt == i) == -1 && outputPattern[y, x - 1][i])
                {
                    outputPattern[y, x - 1][i] = false;
                    modified[3] = true;
                }
            }
            if (x < oN - 1)
            {
                if (valid[4].FindIndex(elmt => elmt == i) == -1 && outputPattern[y, x + 1][i])
                {
                    outputPattern[y, x + 1][i] = false;
                    modified[4] = true;
                }
            }
            if (y < oM - 1)
            {
                if (x > 0)
                {
                    if (valid[5].FindIndex(elmt => elmt == i) == -1 && outputPattern[y + 1, x - 1][i])
                    {
                        outputPattern[y + 1, x - 1][i] = false;
                        modified[5] = true;
                    }
                }
                if (valid[6].FindIndex(elmt => elmt == i) == -1 && outputPattern[y + 1, x][i])
                {
                    outputPattern[y + 1, x][i] = false;
                    modified[6] = true;
                }
                if (x < oN - 1)
                {
                    if (valid[7].FindIndex(elmt => elmt == i) == -1 && outputPattern[y + 1, x + 1][i])
                    {
                        outputPattern[y + 1, x + 1][i] = false;
                        modified[7] = true;
                    }
                }
            }
        }
        Debug.Log("Test P2");
        //Propagate to surrounding tiles
        Debug.Log("Test P3");
        if (y > 0)
        {
            if (x > 0)
            {
                if(modified[0])
                    Propagate(x - 1, y - 1);
            }
            if (modified[1])
                Propagate(x, y - 1);
            if (x < oN - 1)
            {
                if (modified[2])
                    Propagate(x + 1, y - 1);
            }
        }
        if (x > 0)
        {
            if (modified[3])
                Propagate(x - 1, y);
        }
        if (x < oN - 1)
        {
            if (modified[4])
                Propagate(x + 1, y);
        }
        if (y < oM - 1)
        {
            if (x > 0)
            {
                if (modified[5])
                    Propagate(x - 1, y + 1);
            }
            if (modified[6])
                Propagate(x, y + 1);
            if (x < oN - 1)
            {
                if (modified[7])
                    Propagate(x + 1, y + 1);
            }
        }
        Debug.Log("Test P4");
    }

    int CalculateEntropy(int i, int j)
    {
        int entropy = 0;
        foreach (bool tile in outputPattern[j, i])
        {
            if (tile)
                entropy++;
        }
        return entropy - 1;
    }

    bool isGen()
    {
        foreach (int entropy in entropyMap)
        {
            if (entropy != 0)
                return false;
        }
        return true;
    }

    void RecordPattern(int[,] pattern, int i, int j)
    {
        int index = patternList.FindIndex(p => p.isEqual(pattern));
        if (index != -1)
        {
            patternFreq[index]++;
            patternRef[index].Add(new int[]{ i, j});
            patternMap[j, i] = index;
        }
        else
        {
            patternList.Add(new Pattern(pattern, patternList.Count - 1));
            patternFreq.Add(1);
            patternRef.Add(new List<int[]>());
            patternRef[patternRef.Count - 1].Add(new int[] { i, j });
            patternMap[j, i] = patternList.Count - 1;

        }
    }

    int[,] PatternAt(int i, int j)
    {
        //Debug.Log(i + ":" + j);
        //Debug.Log(input.GetLength(0) + ":" + input.GetLength(1));
        //Debug.Log((j - 1 + iM) % iM + ":" + (i - 1 + iN) % iN);
        return new int[,]{{input[(j - 1 + iM) % iM, (i - 1 + iN) % iN],
                         input[(j - 1 + iM) % iM, i],
                         input[(j - 1 + iM) % iM, (i + 1 + iN) % iN] },
                         { input[j, (i - 1 + iN) % iN],
                         input[j, i],
                         input[j, (i + 1 + iN) % iN] },
                         { input[(j + 1 + iM) % iM, (i - 1 + iN) % iN],
                         input[(j + 1 + iM) % iM, i],
                         input[(j + 1 + iM) % iM, (i + 1 + iN) % iN] } };
    }

    int[,] Flip(int[,] mat, int n)
    {
        int[,] ret = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = mat[n - i - 1, j];
            }
        }
        return ret;
    }
    int[,] Rotate(int[,] mat, int n)
    {
        int[,] ret = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = mat[n - j - 1, i];
            }
        }
        return ret;
    }
}
