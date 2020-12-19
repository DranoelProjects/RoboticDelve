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
        for (int j = 0; j < pM; j++)
        {
            for (int i = 0; i < pN; i++)
            {
                patternMap[j, i] = -1;
            }
        }
        for (int i = 0; i < iN; i++)
        {
            for (int j = 0; j < iM; j++)
            {
                //Get the pattern at the current position
                int[,] pattern = PatternAt(i, j);
                RecordPattern(pattern, i, j, 0);
                for (int t = 1; t < 4; t++)
                {
                    RecordPattern(this.Rotate(pattern, 3, t), i, j, t);
                }
                //Create a flipped version
                int[,] patternFlip = Flip(pattern, 3);
                if (!this.isEqual(pattern, patternFlip))
                {
                    for (int t = 0; t < 4; t++)
                    {
                        RecordPattern(this.Rotate(patternFlip, 3, t), i, j, t + 4);
                    }
                }
            }
        }
        //Debug
        Debug.Log(displayText(patternMap));
        freqTot = 0;
        foreach (int freq in patternFreq)
        {
            freqTot += freq;
        }

        //foreach (Pattern pat in patternList)
        //{
        //    Debug.Log(pat.getId());
        //    Debug.Log(pat.getType());
        //    Debug.Log(displayText(pat.getPattern()));
        //}

        //Adjency rules and frequency hints
        Debug.Log(patternMap.GetLength(1) + ":" + patternMap.GetLength(0));
        foreach (Pattern pat in patternList)
        {
            //Debug.Log("ID: " + pat.getId());
            //Debug.Log("Type: " + pat.getType());
            //Debug.Log(displayText(pat.getPattern()));
            int index = patternList.FindIndex(p => isEqual(p.getPattern(), pat.getPattern()));
            foreach (int[] position in patternRef[index])
            {
                if (pat.getType() == 0)
                    AddAdj0(pat, position);
                else if (pat.getType() == 1)
                    AddAdj1(pat, position);
                else if (pat.getType() == 2)
                    AddAdj2(pat, position);
                else if (pat.getType() == 3)
                    AddAdj3(pat, position);
                else if (pat.getType() == 4)
                    AddAdj4(pat, position);
                else if (pat.getType() == 5)
                    AddAdj5(pat, position);
                else if (pat.getType() == 6)
                    AddAdj6(pat, position);
                else if (pat.getType() == 7)
                    AddAdj7(pat, position);
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
        Debug.Log(maxTotEnt);
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

    public void AddAdj0(Pattern pat, int[] position)
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

    public void AddAdj1(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 1))), 2);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3, 1))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 1))), 7);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 1))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 1))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 1))), 0);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3, 1))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 1))), 5);
    }

    public void AddAdj2(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 2))), 7);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3, 2))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 2))), 5);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 2))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 2))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 2))), 2);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3, 2))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 2))), 0);
    }

    public void AddAdj3(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 3))), 5);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3, 3))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 3))), 0);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 3))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 3))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3, 3))), 7);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3, 3))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3, 3))), 2);
    }

    public void AddAdj4(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3))), 2);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3))), 0);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3))), 7);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3))), 5);
    }

    public void AddAdj5(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 1))), 7);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3), 3, 1))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 1))), 2);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 1))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 1))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 1))), 5);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3), 3, 1))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 1))), 0);
    }

    public void AddAdj6(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 2))), 5);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3), 3, 2))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 2))), 7);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 2))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 2))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 2))), 0);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3), 3, 2))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 2))), 2);
    }

    public void AddAdj7(Pattern pat, int[] position)
    {
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 3))), 0);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, position[0]]].getPattern(), 3), 3, 3))), 3);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] - 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 3))), 5);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[position[1] % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 3))), 1);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[position[1] % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 3))), 6);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] - 1 + iN) % iN]].getPattern(), 3), 3, 3))), 2);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, position[0]]].getPattern(), 3), 3, 3))), 4);
        pat.addRule(patternList.Find(p => isEqual(p.getPattern(), Rotate(Flip(patternList[patternMap[(position[1] + 1 + iM) % iM, (position[0] + 1 + iN) % iN]].getPattern(), 3), 3, 3))), 7);
    }

    public void GenerateMap()
    {
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
        //Gen loop
        while (!isGen())
        {
            if (stopThread)
            {
                return;
            }
            //If the pattern is already set, retry with another tile
            if (entropyMap[y, x] == 0)
            {
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
            //Set the pattern accordingly
            int freqLeft = 0;
            for (int f = 0; f < patternFreq.Count; f++)
            {
                if (outputPattern[y,x][f])
                {
                    freqLeft += patternFreq[f];
                }
            }
            int rngPatFreq = (int)myRand.Next(freqLeft);
            int trueIndex = -1;
            int freqCumulee = 0;
            for (int i = 0; i < nbPat && trueIndex == -1; i++)
            {
                if (!outputPattern[y, x][i])
                    continue;
                if (freqCumulee < rngPatFreq)
                    freqCumulee += patternFreq[i];
                else
                    trueIndex = i;
            }
            for (int i = 0; i < nbPat; i++)
            {
                if (i != trueIndex)
                {
                    outputPattern[y, x][i] = false;
                }
                entropyMap[y, x] = 0;
            }
            //TODO propagate choice to surrounding tiles
            Propagate(x, y);
        }
        output = CreateFinalMap();
        Debug.Log(displayText(output));
    }

    int CalculateTotalEntropy()
    {
        int totalEntropy = 0;
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {
                totalEntropy += CalculateEntropy(i, j);
            }
        }
        return totalEntropy;
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
                if (modified[0])
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

    void RecordPattern(int[,] pattern, int i, int j, int type)
    {
        //if (pattern == new int[,]{ { 0, 0, 1}, { 0, 0, 0}, { 1, 0, 1} })
        //{
        //    Debug.Log(i + "," + j + ":" + type);
        //}
        int index = patternList.FindIndex(p => p.isEqual(pattern));
        if (index != -1)
        {
            patternFreq[index]++;
            patternRef[index].Add(new int[] { i, j });
            patternMap[j, i] = index;
        }
        else
        {
            patternList.Add(new Pattern(pattern, patternList.Count, type));
            patternFreq.Add(1);
            patternRef.Add(new List<int[]>());
            patternRef[patternRef.Count - 1].Add(new int[] { i, j });
            if (patternMap[j, i] == -1)
            {
                patternMap[j, i] = patternList.Count - 1;
            }
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

    int[,] Rotate(int[,] mat, int n, int rotation)
    {
        int[,] temp = new int[n, n];
        int[,] ret = new int[n, n];
        CopyIn(temp, mat, n);
        CopyIn(ret, mat, n);
        //string debugTxt = "Rotation\n";
        //debugTxt += displayText(temp);
        for (int r = 0; r < rotation; r++)
        {
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = temp[n - j - 1, i];
                }
            }
            CopyIn(temp, ret, n);
        }
        //debugTxt += displayText(ret);
        //Debug.Log(debugTxt);
        return ret;
    }

    public bool isEqual(int[,] mat1, int[,] mat2)
    {
        bool equals = true;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (mat1[i, j] != mat2[i, j])
                {
                    equals = false;
                }
            }
        }
        return equals;
    }

    int[,] CopyIn(int[,] dest, int[,] origin, int n)
    {
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                dest[i, j] = origin[i, j];
            }
        }
        return dest;
    }

    string displayText(int[,] mat)
    {
        string sortie = "";
        for (int j = 0; j < mat.GetLength(0); j++)
        {
            for (int i = 0; i < mat.GetLength(1); i++)
            {
                sortie += mat[j, i] + ",";
            }
            sortie += "\n";
        }
        return sortie;
    }
}
