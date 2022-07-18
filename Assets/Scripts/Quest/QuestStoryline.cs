using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestStoryline : MonoBehaviour
{
    [SerializeField] QuestBase[] storyline;
    [SerializeField] Text questDescriptionText;

    int currentQuestNo = 0;
    //public event Action OnQuestCompleted;
    List<QuestBase> questBase = new List<QuestBase>();
    List<Quest> quest = new List<Quest>();

    // Start is called before the first frame update
    QuestList questList;
    void Start()
    {
        questList = QuestList.GetQuestList();
        questList.OnUpdated += UpdateQuestDescription;

        quest = questList.Quests;
        QuestListToBaseQuestArray(questList);
        //UpdateQuestDescription();
        SetFirstQuestDescription();

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
            return false;
        }
        else
            return false;
    }

    public void SetFirstQuestDescription()
    {
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
