using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MazeGenerator : MonoBehaviour
{
    public Texture2D source;
    public float tileDimension = 1f;
    public Transform parent;
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    [SerializeField, HideInInspector]
    MazeNode[,] tiles;
    
    void ClearPrev()
    {
        for (int i = parent.childCount; i > 0; i--)
        {
            DestroyImmediate(parent.GetChild(i - 1).gameObject);
        }
    }

    [InspectorButton("CreateMaze")]
    public string generate;

    MazeNode GetNeighbor(int x, int y)
    {
        MazeNode neighbor = null;

        if (x > -1 && x < tiles.GetLength(0) && y > -1 && y < tiles.GetLength(1))
        {
            neighbor = tiles[x, y];
        }

        return neighbor;
    }

    void CreateMaze()
    {
        ClearPrev();
        tiles = new MazeNode[source.width, source.height];
        var pixels = source.GetPixels();
        int width = source.width;
        int height = source.height;
        

        Action<int> initialMaze = (int start) =>
        {
            for (int y = start; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pixels[y * width + x].r > 0)
                    {
                        var config = new MazeNode.MazeConfiguration()
                        {
                            tilesParent = parent,
                            tileDimension = tileDimension,
                            gridPosition = new Vector2Int(x, y),
                            floorPrefab = floorPrefab,
                            wallPrefab = wallPrefab
                        };
                        tiles[x, y] = new MazeNode(config);
                    }

                }
            }
        };

        Action<int> setNighborsMaze = (int start) =>
        {
            for (int y = start; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].SetNeighbor(0, GetNeighbor(x, y + 1));
                        tiles[x, y].SetNeighbor(1, GetNeighbor(x + 1, y));
                        tiles[x, y].SetNeighbor(2, GetNeighbor(x, y - 1));
                        tiles[x, y].SetNeighbor(3, GetNeighbor(x - 1, y));
                    }

                }
            }
        };

        int readyThreads = 0;
        int maxThreads = 8;
        int itemsPerThread = height / maxThreads;
        int reminder = (height) - (itemsPerThread * (maxThreads - 1));

        Action<Action<int>> enableRun = (a) =>
        {
            readyThreads = 0;
            for (int i = 0; i < maxThreads - 1; i++)
            {
                int c = i;
                var t = new Thread(() =>
                {
                    a(i * itemsPerThread);
                    readyThreads++;
                });
                t.Start();
            }
            a(reminder);
            readyThreads++;
            while (readyThreads < maxThreads)
            {
                Thread.Sleep(50);
                Debug.Log("Processing...");
            }
        };

        enableRun(initialMaze);
        enableRun(setNighborsMaze);

        for (int y = 0; y < source.height; y++)
        {
            for (int x = 0; x < source.width; x++)
            {
                if (tiles[x, y] != null)
                    tiles[x, y].BuildTile();
            }
        }
    }
}
