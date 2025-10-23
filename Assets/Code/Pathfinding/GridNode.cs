using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public bool walkable;
    public Vector2 position;

    public GridNode(bool _walkable, Vector2 _position)
    {
        walkable = _walkable;
        position = _position;
    }
}
