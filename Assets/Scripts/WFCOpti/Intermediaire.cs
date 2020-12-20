using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Intermediaire
{
    private System.Drawing.Color[,] _data, colorOutput;
    private Model model;
    private Thread myThread;
    private bool goToNextScene, stopThread, toutFini, showProgBar, unableToGen;
    public int outputHeight, outputWidth, maxEntropy, currentEntropy;
    private int[,] finalOutput;
    System.Random random = new System.Random();

    // Start is called before the first frame update
    public Intermediaire()
    {
        goToNextScene = false;
        stopThread = false;
        toutFini = false;
        showProgBar = false;
        unableToGen = false;

        //myThread = new Thread(startProcess);
        //myThread.Start();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (unableToGen)
    //    {
    //        Debug.Log("Unable to generate map");
    //        unableToGen = false;
    //    }
    //    if (toutFini)
    //    {
    //        Debug.Log(displayText(finalOutput));
    //        toutFini = false;
    //    }
    //    if (showProgBar)
    //    {
    //        currentEntropy = (int)model.totalEntropy();
    //    }
    //}

    public int[,] startProcess()
    {
        string name = "test";
        int[,] input = new int[,]{ {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                            {0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0},
                            {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                            {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0},
                            {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                            {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                            {0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} };

        model = new OverlappingModel(input, name, 3, outputWidth, outputHeight, true, false, 8, 0);
        bool finished = false;
        int attempts = 0;
        //maxEntropy = (int) model.totalEntropy();
        //currentEntropy = maxEntropy;
        //showProgBar = true;
        while (!finished && attempts < 10)
        {
            int seed = random.Next();
            finished = model.Run(seed, 0);
        }

        System.Drawing.Color[,] mapCouleur;
        if (finished)
        {
            mapCouleur = model.GetMap();
            finalOutput = colorToInt(mapCouleur, outputHeight, outputWidth);
            toutFini = true;
            return finalOutput;
        }
        else
        {
            unableToGen = true;
            return null;
        }
    }
    public int[,] colorToInt(System.Drawing.Color[,] colorData, int height, int width)
    {
        int[,] ret = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (colorData[i, j].B == 0)
                {
                    ret[i, j] = 0;
                }
                else if (colorData[i, j].B == 255)
                {
                    ret[i, j] = 1;
                }
            }
        }
        return ret;
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