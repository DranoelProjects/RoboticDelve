//using QM2D.Generator;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using UnityEngine;

//using Color = System.Drawing.Color;
//using WFCInput = QM2D.Generator.Input;

//public class Intermediaire : MonoBehaviour
//{
//    private Color[,] _data, colorOutput;
//    private Thread myThread;
//    private bool goToNextScene, stopThread, toutFini, showProgBar;
//    public int outputHeight, outputWidth, maxEntropy, currentEntropy;
//    private WFCInput myInput;
//    private State myState;
//    private int[,] finalOutput;
//    void Start()
//    {
//        Application.targetFrameRate = 60;
//        goToNextScene = false;
//        stopThread = false;
//        toutFini = false;
//        showProgBar = false;
//        myThread = new Thread(startProcess);
//        myThread.Start();
//    }
    
//    void FixedUpdate()
//    {
//        if (toutFini)
//        {
//            Debug.Log(displayText(finalOutput));
//            toutFini = false;
//        }
//        //if (showProgBar)
//        //{
//        //}
//    }

//    private void OnApplicationQuit()
//    {
//        stopThread = true;
//    }

//    public void startProcess()
//    {
//        int[,] input = new int[,]{  {0, 0, 0, 0, 0, 0, 0, 0},
//                                    {0, 1, 1, 1, 1, 0, 0, 0},
//                                    {0, 1, 0, 0, 1, 0, 0, 0},
//                                    {0, 1, 0, 0, 1, 0, 0, 0},
//                                    {0, 1, 1, 0, 1, 0, 1, 1},
//                                    {0, 0, 0, 0, 1, 0, 0, 1},
//                                    {0, 0, 0, 0, 1, 0, 0, 1},
//                                    {0, 0, 0, 0, 1, 1, 1, 1} };
                                    
//        int height = input.GetLength(0), width = input.GetLength(1);
//        _data = intToColor(input, height, width);
//        Vector2i outputDim = new Vector2i(outputHeight, outputWidth);
//        myInput = new WFCInput(_data, new Vector2i(3, 3), false, false, false, false);
//        System.Random rng = new System.Random();
//        int seed = rng.Next();
//        myState = new State(myInput, outputDim, false, false, seed);
//        HashSet<Vector2i> failPoints = new HashSet<Vector2i>();
//        bool isGen = false;
//        int echecs = 0;
//        maxEntropy = myState.getTotEntropy();
//        showProgBar = true;
//        while (!isGen)
//        {
//            var etat = myState.Iterate(ref failPoints);
//            if (etat == true)
//            {
//                isGen = true;
//            }
//            if (etat == false)
//            {
//                seed = rng.Next();
//                myState.Reset(null, seed);
//                echecs++;
//                if (echecs > 16)
//                {
//                    stopThread = true;
//                }
//            }
//            currentEntropy = myState.getTotEntropy();
//            if (stopThread)
//            {
//                return;
//            }
//        }
//        colorOutput = getColorOutput(height, width);
//        finalOutput = colorToInt(colorOutput, height, width);
//        toutFini = true;
//    }

//    public Color[,] intToColor(int[,] intData, int height, int width)
//    {
//        Color[,] ret = new Color[height, width];
//        int pixelColor = 0;
//        for (int i = 0; i < height; i++)
//        {
//            for (int j = 0; j < width; j++)
//            {
//                if (intData[i, j] == 0)
//                {
//                    pixelColor = 0;
//                }
//                else if (intData[i, j] == 1)
//                {
//                    pixelColor = 255;
//                }
//                ret[i, j] = Color.FromArgb(pixelColor, pixelColor, pixelColor);
//            }
//        }
//        return ret;
//    }

//    public int[,] colorToInt(Color[,] colorData, int height, int width)
//    {
//        int[,] ret = new int[height, width];
//        for (int i = 0; i < height; i++)
//        {
//            for (int j = 0; j < width; j++)
//            {
//                if (colorData[i, j].B == 0)
//                {
//                    ret[i, j] = 0;
//                }
//                else if (colorData[i, j].B == 255)
//                {
//                    ret[i, j] = 1;
//                }
//            }
//        }
//        return ret;
//    }
//    public Color[,] getColorOutput(int height, int width)
//    {
//        Color[,] ret = new Color[height, width];
//        for (int i = 0; i < height; i++)
//        {
//            for (int j = 0; j < width; j++)
//            {
//                var col = myState.OutputColorGetter(new Vector2i(i, j));
//                if (col == null)
//                {
//                    col = Color.FromArgb(0, 0, 0);
//                }
//                ret[i, j] = (Color) col;
//            }
//        }
//        return ret;
//    }

//    string displayText(int[,] mat)
//    {
//        string sortie = "";
//        for (int j = 0; j < mat.GetLength(0); j++)
//        {
//            for (int i = 0; i < mat.GetLength(1); i++)
//            {
//                sortie += mat[j, i] + ",";
//            }
//            sortie += "\n";
//        }
//        return sortie;
//    }
//}
