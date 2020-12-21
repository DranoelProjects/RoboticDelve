using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Intermediaire
{
    private System.Drawing.Color[,] _data, colorOutput;
    private Model model;
    //private Thread myThread;
    private bool goToNextScene, toutFini, showProgBar, unableToGen;
    public int outputHeight, outputWidth, maxEntropy, currentEntropy;
    private int[,] finalOutput;
    System.Random random = new System.Random();

    // Start is called before the first frame update
    public Intermediaire(int oHeight, int oWidth)
    {
        goToNextScene = false;
        toutFini = false;
        showProgBar = false;
        unableToGen = false;
        outputHeight = oHeight;
        outputWidth = oWidth;
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

    public int[,] startProcess(int lvl)
    {
        string name = "test";
        List<int[,]> inputs = new List<int[,]>();
        //Format d'ajout de type de niveau
        //inputs.Add(new int[,]{  });
        inputs.Add(new int[,]{  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                {0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0},
                                {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                                {0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0},
                                {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                                {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0},
                                {0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
                                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} });
        inputs.Add(new int[,] { {1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1},
                                {1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                {1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                {1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
                                {1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                {1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1},
                                {1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1},
                                {1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1},
                                {1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1},
                                {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0},
                                {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                {1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1},
                                {1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1},
                                {1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1} });
        int lvlStep = 3;
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.GetInt("Level") < lvlStep)
            {
                index = i;
                break;
            }
            lvlStep += 3;
        }
        model = new OverlappingModel(inputs[index], name, 3, outputWidth, outputHeight, true, false, 8, 0);
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