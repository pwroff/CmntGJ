using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class MazeNode
{

    public struct MazeConfiguration
    {
        public Transform tilesParent;
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public Vector2Int gridPosition;
        public float tileDimension;
    }

    [SerializeField, HideInInspector]
    MazeNode[] neighbors = new MazeNode[4];

    MazeConfiguration configuration;
    GameObject gameObject;

    public MazeNode(MazeConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [Conditional("UNITY_EDITOR")]
    void EnsureAndThrow(ref int direction)
    {
        if (direction < 0 || direction > 4)
            throw new System.Exception("There are only 4 available directions in our world");
    }

    /// <summary>
    /// directions:
    /// 0: north/forward
    /// 1: east/right
    /// 2: south/back
    /// 3: west/left
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="node"></param>
    public void SetNeighbor(int direction, MazeNode node)
    {
        EnsureAndThrow(ref direction);
        neighbors[direction] = node;
    }

    public void BuildTile()
    {
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gameObject.name = string.Format("Floor-{0}-{1}", configuration.gridPosition.x, configuration.gridPosition.y);
        gameObject.transform.Rotate(new Vector3(90, 0, 0));
        gameObject.transform.parent = configuration.tilesParent;
        gameObject.transform.localScale = Vector3.one * configuration.tileDimension;
        gameObject.transform.localPosition = new Vector3(configuration.gridPosition.x * configuration.tileDimension, 0, configuration.gridPosition.y * configuration.tileDimension);
        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i] == null)
            {
                var wall =  GameObject.CreatePrimitive(PrimitiveType.Quad);
                wall.name = "wall" + i;
                wall.transform.parent = gameObject.transform;
                wall.transform.localScale = Vector3.one;
                wall.transform.rotation = Quaternion.identity;
                wall.transform.position = gameObject.transform.position + configuration.tileDimension * GetLocalPositionForOrientation(i);
                wall.transform.Rotate(new Vector3(0, i * 90, 0));
            }
        }
    }

    private Vector3 GetLocalPositionForOrientation(int i)
    {
        EnsureAndThrow(ref i);
        switch (i)
        {
            case 0:
                return new Vector3(0, 0.5f, 0.5f);
            case 1:
                return new Vector3(0.5f, 0.5f, 0);
            case 2:
                return new Vector3(0, 0.5f, -0.5f);
            case 3:
                return new Vector3(-0.5f, 0.5f, 0);

            default:
                return default;

        }
    }
}
