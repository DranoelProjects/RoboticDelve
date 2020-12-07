using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC : MonoBehaviour
{
    private int N, M, pN, pM, iN, iM, freqTot;
    public int oN, oM;
    private int[,] input, output, patternMap, entropyMap;
    private bool[,][] outputPattern;
    private List<Pattern> patternList;
    private List<List<int[]>> patternRef;
    private List<int> patternFreq;
    private int[,,] tilesMap;
    private int[] possibilities;
    // Start is called before the first frame update
    void Start()
    {
        output = new int[oM, oN];
        N = oN;
        M = oM;
        
        //possible tiles
        possibilities = new int[]{ 0, 1};

        //Input to recreate
        input = new int[,]{ { } };

        //creation of pattern map
        iN = input.GetLength(1);
        iM = input.GetLength(0);
        pN = iN;
        pM = iM;
        patternList = new List<Pattern>();
        patternFreq = new List<int>();
        patternMap = new int[pM, pN];
        for (int i = 0; i < input.GetLength(1); i++)
        {
            for (int j = 0; j < input.GetLength(0); j++)
            {
                //Get the pattern at the current position
                int[,] pattern = PatternAt(i, j);
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
        foreach (Pattern pat in patternList)
        {
            int index = patternList.FindIndex(p => p.getPattern() == pat.getPattern());
            foreach (int[] position in patternRef[index])
            {
                pat.addRule(patternList[patternMap[(position[0] - 1) % oM, (position[1] - 1) % oN]], 0);
                pat.addRule(patternList[patternMap[(position[0] - 1) % oM, position[1]]], 1);
                pat.addRule(patternList[patternMap[(position[0] - 1) % oM, (position[1] + 1) % oN]], 2);
                pat.addRule(patternList[patternMap[position[0] % oM, (position[1] - 1) % oN]], 3);
                pat.addRule(patternList[patternMap[position[0] % oM, (position[1] + 1) % oN]], 5);
                pat.addRule(patternList[patternMap[(position[0] + 1) % oM, (position[1] - 1) % oN]], 6);
                pat.addRule(patternList[patternMap[(position[0] + 1) % oM, position[1]]], 7);
                pat.addRule(patternList[patternMap[(position[0] + 1) % oM, (position[1] + 1) % oN]], 8);
            }
        }

        //Map where we apply WFC
        outputPattern = new bool[oM, oN][];
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {
                outputPattern[j, i] = new bool[patternList.Count];
                for (int k = 0; k < patternList.Count; k++)
                {
                    outputPattern[j, i][k] = true;
                }
            }
        }

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
        for (int j = 0; j < oM; j++)
        {
            for (int i = 0; i < oN; i++)
            {

            }
        }
    }

    void GenerateMap()
    {
        //Choose a random starting point
        int x = (int)Random.Range(0, oN);
        int y = (int)Random.Range(0, oM);
        int nbPat = outputPattern[0, 0].Length;
        //Gen loop
        while (!isGen())
        {
            //If the pattern is already set, retry with another tile
            if (entropyMap[y, x] == 0)
            {
                x = (int)Random.Range(0, oN);
                y = (int)Random.Range(0, oM);
                continue;
            }
            int rngPat = (int)Random.Range(0, nbPat);
            //Check if the pattern is possible at position else try another pattern
            if (outputPattern[0, 0][rngPat] == false)
            {
                continue;
            }
            //Set the pattern
            for (int i = 0; i < nbPat; i++)
            {
                if (i != rngPat)
                {
                    outputPattern[y, x][i] = false;
                }
                entropyMap[y, x] = 0;
            }
            //TODO propagate choice to surrounding tiles
            Propagate(x, y);

        }
    }

    void Propagate(int x, int y)
    {
        List<Pattern> options = patternList.FindAll(p => outputPattern[y, x][p.getId()] == true);
        List<int>[] valid = new List<int>[8];
        for (int dir = 0; dir < 8; dir++)
        {
            foreach (Pattern option in options)
            {
                List<int> rules = option.getRuleI(dir);
                foreach (int rule in rules)
                {
                    if (valid[dir].FindIndex(elmnt => elmnt == rule) == -1)
                    {
                        valid[dir].Add(rule);
                    }
                }
            }
        }
        patternMap[y, x]
    }

    int CalculateEntropy(int i, int j)
    {
        int entropy = 0;
        foreach (bool tile  in outputPattern[j,i])
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
        int index = patternList.FindIndex(p => p.getPattern() == pattern);
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
        return new int[,]{{input[(j - 1) % iM, (i - 1) % iN],
                         input[(j - 1) % iM, i],
                         input[(j - 1) % iM, (i + 1) % iN] },
                         { input[j, (i - 1) % iN],
                         input[j, i],
                         input[j, (i + 1) % iN] },
                         { input[(j + 1) % iM, (i - 1) % iN],
                         input[(j + 1) % iM, i],
                         input[(j + 1) % iM, (i + 1) % iN] } };
    }

    int[,] Flip(int[,] mat, int n)
    {
        int[,] ret = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = mat[n - i, j];
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
