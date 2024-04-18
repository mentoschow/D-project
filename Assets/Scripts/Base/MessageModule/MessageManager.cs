using System.Collections.Generic;
using UnityEngine.Events;

public class MessageManager : Singleton<MessageManager>
{
    private Dictionary<string, UnityAction<MessageData>> dictionaryMessage;
    private Dictionary<string, UnityAction> dictionaryMessageNoData;

    public MessageManager()
    {
        InitData();
    }

    private void InitData()
    {
        dictionaryMessage = new Dictionary<string, UnityAction<MessageData>>();
        dictionaryMessageNoData = new Dictionary<string, UnityAction>();
    }

    public void Register(string key, UnityAction<MessageData> action)
    {
        if (!dictionaryMessage.ContainsKey(key))
        {
            dictionaryMessage.Add(key, action);
        }
        else
        {
            dictionaryMessage[key] += action;
        }
    }

    public void Register(string key, UnityAction action)
    {
        if (!dictionaryMessageNoData.ContainsKey(key))
        {
            dictionaryMessageNoData.Add(key, action);
        }
        else
        {
            dictionaryMessageNoData[key] += action;
        }
    }

    public void Remove(string key, UnityAction<MessageData> action)
    {
        if (dictionaryMessage.ContainsKey(key))
        {
            dictionaryMessage[key] -= action;
        }
    }

    public void Remove(string key, UnityAction action)
    {
        if (dictionaryMessageNoData.ContainsKey(key))
        {
            dictionaryMessageNoData[key] -= action;
        }
    }

    public void Send(string key, MessageData data)
    {
        UnityAction<MessageData> action = null;
        if (dictionaryMessage.TryGetValue(key, out action))
        {
            action(data);
        }
    }

    public void Send(string key)
    {
        UnityAction action = null;
        if (dictionaryMessageNoData.TryGetValue(key, out action))
        {
            action();
        }
    }

    public void Clear()
    {
        dictionaryMessage.Clear();
    }
}