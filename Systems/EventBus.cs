using System;
using System.Collections.Generic;

namespace NNN.Systems;

public class EventBus
{
    private readonly Dictionary<Type, List<Action<object>>> _subscribers = new();

    public void Subscribe<T>(Action<T> subscriber)
    {
        if (!_subscribers.ContainsKey(typeof(T))) _subscribers[typeof(T)] = new List<Action<object>>();

        _subscribers[typeof(T)].Add(x => subscriber((T)x));
    }

    public void Publish<T>(T eventMessage)
    {
        if (_subscribers.ContainsKey(typeof(T)))
            foreach (var subscriber in _subscribers[typeof(T)])
                subscriber(eventMessage);
    }
}