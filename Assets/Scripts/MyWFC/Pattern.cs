using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern
{
    int id, type;
    int[] freqTot;
    int[,] pattern;
    List<int>[] adjRules, hintFreq;

    public Pattern(int[,] pat, int idtf, int patType)
    {
        pattern = pat;
        id = idtf;
        type = patType % 8;
        adjRules = new List<int>[8];
        hintFreq = new List<int>[8];
        for (int i = 0; i < 8; i++)
        {
            adjRules[i] = new List<int>();
            hintFreq[i] = new List<int>();
        }
        freqTot = new int[8];
    }

    void setPattern(int[,] pat)
    {
        pattern = pat;
    }

    public bool isEqual(int[,] other)
    {
        bool equals = true;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (this.pattern[i, j] != other[i, j])
                {
                    equals = false;
                }
            }
        }
        return equals;
    }

    public int[,] getPattern()
    {
        return pattern;
    }

    public void addRule(Pattern pat, int i)
    {
        int index = adjRules[i].FindIndex(idx => idx == pat.getId());
        if (index != -1)
        {
            hintFreq[i][index]++;
        }
        else
        {
            adjRules[i].Add(pat.getId());
            hintFreq[i].Add(1);
        }
    }

    //public Pattern FlipPattern(int i)
    //{
    //    Pattern flipPat = new Pattern(this.Flip(this.pattern, 3), i);
    //    flipPat
    //}

    private Pattern Flip(int n)
    {
        int[,] temp = new int[n, n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                temp[i, j] = pattern[n - i - 1, j];
            }
        }
        Pattern ret = new Pattern(temp, 0, type + 4);
        return ret;
    }

    private int[,] Rotate(int[,] mat, int n, int nbRot)
    {
        int[,] ret = new int[n, n];
        for (int r = 0; r < nbRot; r++)
        {
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = mat[n - j - 1, i];
                }
            }
        }
        return ret;
    }

    public List<int> getRuleI(int i)
    {
        return adjRules[i];
    }

    public void calcFreq()
    {
        for (int i = 0; i < 8; i++)
        {
            freqTot[i] = 0;
            for (int k = 0; k < hintFreq[i].Count; k++)
            {
                freqTot[i] += hintFreq[i][k];
            }
        }
    }

    public int getId()
    {
        return id;
    }

    public int getTile()
    {
        return pattern[1, 1];
    }

    public int getType()
    {
        return type;
    }
}
