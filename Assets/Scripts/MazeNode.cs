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
        public GameObject innerCornerPrefab;
        public GameObject outerCornerPrefab;
        public Vector2Int gridPosition;
        public float tileDimension;
    }

    [SerializeField, HideInInspector]
    MazeNode[] neighbors = new MazeNode[8];

    MazeConfiguration configuration;
    GameObject gameObject;

    public MazeNode(MazeConfiguration configuration)
    {
        this.configuration = configuration;
        neighbors = new MazeNode[8];
    }

    [Conditional("UNITY_EDITOR")]
    void EnsureAndThrow(ref int direction)
    {
        if (direction < 0 || direction > 8)
            throw new System.Exception("There are only 8 available directions in our world");
    }

    /// <summary>
    /// directions:
    /// 0: north/forward
    /// 1: east/right
    /// 2: south/back
    /// 3: west/left
    /// 4: north-east
    /// 5: east-south
    /// 6: south-west
    /// 7: west-north
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="node"></param>
    public void SetNeighbor(int direction, MazeNode node)
    {
        EnsureAndThrow(ref direction);
        neighbors[direction] = node;
    }

    public void BuildTile(bool applyScale)
    {
        gameObject = GameObject.Instantiate(configuration.floorPrefab);
        gameObject.name = string.Format("Floor-{0}-{1}", configuration.gridPosition.x, configuration.gridPosition.y);
        gameObject.transform.parent = configuration.tilesParent;
        gameObject.transform.localScale = applyScale ? Vector3.one * configuration.tileDimension : Vector3.one;
        gameObject.transform.localPosition = new Vector3(configuration.gridPosition.x * configuration.tileDimension, 0, configuration.gridPosition.y * configuration.tileDimension);
        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i] == null)
            {
                var wall =  GameObject.Instantiate(configuration.wallPrefab);
                wall.name = "wall" + i;
                wall.transform.parent = gameObject.transform;
                wall.transform.localScale = Vector3.one;
                wall.transform.rotation = Quaternion.identity;
                wall.transform.position = gameObject.transform.position + configuration.tileDimension * GetLocalPositionForOrientation(i);
                wall.transform.Rotate(new Vector3(0, i * 90, 0));
            }
        }

        BuildCorners();
    }

    void BuildCorners()
    {
        for (int i = 0; i < 4; i++)
        {
            // Calculate for patch 2x2
            var n = neighbors[i];
            var adj = neighbors[i + 4];
            var n1 = neighbors[(i + 1) % 4];
            if (n != null && adj != null && n1 != null)
            {
                // no corner, tiles are touching each other in the middle
                continue;
            }

            if (n == null && n1 != null || n1 == null && n != null)
            {
                // corridor
                continue;
            }
            GameObject corner;
            if (n1 != null && n != null)
            {
                // solve for outer corner
                if (configuration.outerCornerPrefab == null)
                    continue;
                corner = GameObject.Instantiate(configuration.outerCornerPrefab);
            }
            else
            {
                if (configuration.innerCornerPrefab == null)
                    continue;
                // solve for inner corner
                corner = GameObject.Instantiate(configuration.innerCornerPrefab);
            }
            Vector3 cornerRotation = new Vector3(0, (i + 1) * 90 - 45, 0);
            Vector3 cornerLocation = gameObject.transform.position + configuration.tileDimension * GetCornerLocation(i);
            corner.transform.parent = gameObject.transform;
            corner.transform.localScale = Vector3.one;
            corner.transform.rotation = Quaternion.identity;
            corner.transform.position = cornerLocation;
            corner.transform.Rotate(cornerRotation);
        }
    }

    Vector3 GetCornerLocation(int i)
    {
        EnsureAndThrow(ref i);
        switch (i)
        {
            case 0:
                return new Vector3(0.5f, 0, 0.5f);
            case 1:
                return new Vector3(0.5f, 0, -0.5f);
            case 2:
                return new Vector3(-0.5f, 0, -0.5f);
            case 3:
                return new Vector3(-0.5f, 0, 0.5f);

            default:
                return default;
        }
    }

    private Vector3 GetLocalPositionForOrientation(int i)
    {
        EnsureAndThrow(ref i);
        switch (i)
        {
            case 0:
                return new Vector3(0, 0, 0.5f);
            case 1:
                return new Vector3(0.5f, 0, 0);
            case 2:
                return new Vector3(0, 0, -0.5f);
            case 3:
                return new Vector3(-0.5f, 0, 0);

            default:
                return default;

        }
    }
}
