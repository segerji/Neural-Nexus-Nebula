using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NNN.Entities;

namespace NNN.Systems;

public class QuadTree
{
    private const int MaxObjects = 10;
    private const int MaxLevels = 5;
    private readonly Rectangle _bounds;
    private readonly int _level;
    private readonly QuadTree[] _nodes;
    private readonly List<IEntity> _objects;

    public QuadTree(int pLevel, Rectangle pBounds)
    {
        _level = pLevel;
        _objects = new List<IEntity>();
        _bounds = pBounds;
        _nodes = new QuadTree[4];
    }

    public void Clear()
    {
        _objects.Clear();
        for (var i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] == null) continue;
            _nodes[i].Clear();
            _nodes[i] = null;
        }
    }

    private void Split()
    {
        var subWidth = _bounds.Width / 2;
        var subHeight = _bounds.Height / 2;
        var x = _bounds.X;
        var y = _bounds.Y;

        _nodes[0] = new QuadTree(_level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
        _nodes[1] = new QuadTree(_level + 1, new Rectangle(x, y, subWidth, subHeight));
        _nodes[2] = new QuadTree(_level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
        _nodes[3] = new QuadTree(_level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    public void Insert(IEntity orb)
    {
        if (_nodes[0] != null)
        {
            var index = GetIndex(orb);

            if (index != -1)
            {
                _nodes[index].Insert(orb);
                return;
            }
        }

        _objects.Add(orb);

        if (_objects.Count > MaxObjects && _level < MaxLevels)
        {
            if (_nodes[0] == null) Split();

            var i = 0;
            while (i < _objects.Count)
            {
                var index = GetIndex(_objects[i]);
                if (index != -1)
                {
                    _nodes[index].Insert(_objects[i]);
                    _objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    private int GetIndex(IEntity orb)
    {
        int index;
        double verticalMidpoint = _bounds.X + _bounds.Width / 2f;
        double horizontalMidpoint = _bounds.Y + _bounds.Height / 2f;

        var topQuadrant = orb.Position.Y < horizontalMidpoint;
        var leftQuadrant = orb.Position.X < verticalMidpoint;
        var rightQuadrant = !leftQuadrant;

        if (topQuadrant)
            index = rightQuadrant ? 0 : 1;
        else
            index = leftQuadrant ? 2 : 3;

        return index;
    }

    public List<IEntity> Retrieve(List<IEntity> returnObjects, IEntity orb)
    {
        var index = GetIndex(orb);
        if (index != -1 && _nodes[0] != null) _nodes[index].Retrieve(returnObjects, orb);

        returnObjects.AddRange(_objects.Where(o => o != orb));
        return returnObjects;
    }
}