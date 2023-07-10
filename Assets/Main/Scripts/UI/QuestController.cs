using System.Collections;
using System.Collections.Generic;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.Quest_System;
using UnityEngine;

public abstract class QuestController<T> : QuestController where T : QuestData
{
    public T QuestData;

    public virtual void Initialize(T questData)
    {
        QuestData = questData;
        QuestType = questData.QuestType;
        SetupVisuals();
    }

    protected abstract void SetupVisuals();

}

public abstract class QuestController : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public QuestData.Type QuestType { get; protected set; }
    public QuestData.State QuestState { get; protected set; }

    public virtual void StartQuest()
    {
        QuestState = QuestData.State.Started;
    }

    public virtual void OnQuestEnd()
    {
        GameManager.Instance.EventHandler.Notify(GameEvent.OnQuestDone);
    }
  
}
