using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestStoryline : MonoBehaviour
{
    [SerializeField] QuestBase[] storyline;
    [SerializeField] Text questDescriptionText;
    [SerializeField] QuestList questList;

    List<QuestBase> questBase = new List<QuestBase>();
    List<Quest> quest = new List<Quest>();

    int currentQuestNo = 0;
    int questCount = 0;

    void Start()
    {
        quest = questList.Quests;
        QuestListToBaseQuestArray(questList);
        SetFirstIncompleteQuestDes();

        questList.OnUpdated += UpdateQuestDescription;
    }

    private void Update()
    {
        if(quest.Count != questCount)
        {
            SetFirstIncompleteQuestDes();
            questCount = quest.Count;
        }
    }

    private void RefreshList()
    {
        questList = QuestList.GetQuestList();
        quest = questList.Quests;
        QuestListToBaseQuestArray(questList);
    }

    private void QuestListToBaseQuestArray(QuestList questList)
    {
        quest = questList.Quests;
        for (int i = 0; i < quest.Count; i++)
        {
            if (!questBase.Contains(quest[i].Base))
                questBase.Add(quest[i].Base);
        }
    }

    private bool isQuestIncomplete(QuestBase quests)
    {
        quest = questList.Quests;
        if (questBase.Contains(quests))
        {
            int idx = questBase.IndexOf(quests);
            if (idx >= 0)
            {
                if (quest[idx].Status == QuestStatus.Started)
                    return true;
            }
        }
        return false;
    }

    public void SetFirstIncompleteQuestDes()
    {
        RefreshList();
        for (int i = currentQuestNo; i < storyline.Length; i++)
        {
            if (!questBase.Contains(storyline[i]) || isQuestIncomplete(storyline[i]))
            {
                questDescriptionText.text = storyline[i].Description;
                break;
            }
            currentQuestNo++;
        }
    }

    public void UpdateQuestDescription()
    {
        //add newquest from questlist to questBase
        quest = questList.Quests;
        if (!questBase.Contains(quest[quest.Count - 1].Base))
            questBase.Add(quest[quest.Count - 1].Base);

        for (int i = currentQuestNo; i < storyline.Length; i++)
        {
            if (!questBase.Contains(storyline[i]) || isQuestIncomplete(storyline[i]))
            {
                questDescriptionText.text = storyline[i].Description;
                break;
            }
            currentQuestNo++;
        }
    }
}
