using System.Collections.Generic;
using UnityEngine;

public class CollectableController
{
    private Player _player;
    private readonly List<ICollectable> _collectableItems = new();

    public void Init()
    {
        _player = GameController.Instance.Player;
    }

    public void Add(ICollectable item)
    {
        if (_collectableItems.Contains(item))
            return;
        _collectableItems.Add(item);
    }

    public void Remove(ICollectable item)
    {
        if (_collectableItems.Contains(item))
            _collectableItems.Remove(item);
    }

    public void Reset()
    {
        for (var i = 0; i < _collectableItems.Count; i++)
        {
            _collectableItems[i].Destroy();
        }

        _collectableItems.Clear();
    }

    public void Update()
    {
        for (var i = 0; i < _collectableItems.Count; i++)
        {
            var e = _collectableItems[i];
            if (Vector2.SqrMagnitude(_player.transform.position -
                                     e.GetPosition()) >
                _player.CurrentData.MagnetRange *
                _player.CurrentData.MagnetRange) continue;
            ((CollectableItemComponent)e).MoveTo(_player.transform.position);
            _player.CollectItem(e);
            Remove(e);
        }
    }
}