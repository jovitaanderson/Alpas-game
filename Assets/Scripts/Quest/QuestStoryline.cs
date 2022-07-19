using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestStoryline : MonoBehaviour
{
    [SerializeField] QuestBase[] storyline;
    [SerializeField] Text questDescriptionText;

    QuestList questList;
    List<Quest> quest = new List<Quest>();
    List<QuestBase> questBase = new List<QuestBase>();

    int currentQuestNo = 0;

    void Start()
    {
        questList = QuestList.GetQuestList();
        questList.OnUpdated += UpdateQuestDescription;

        SetFirstQuestDescription();
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

    public void SetFirstQuestDescription()
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
