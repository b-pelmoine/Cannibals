using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConditionnedAction : ActionTask
{

    public ConditionList conditionTasks;
    public ActionList actionTasks;
    public bool repeat = true;

    public override void OnValidate(ITaskSystem ownerSystem)
    {

        base.OnValidate(ownerSystem);
        SetOwnerSystem(ownerSystem);

        if (actionTasks == null)
        {
            actionTasks = (ActionList)Task.Create(typeof(ActionList), ownerSystem);
            actionTasks.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
        }

        if (conditionTasks == null)
        {
            conditionTasks = (ConditionList)Task.Create(typeof(ConditionList), ownerSystem);
            conditionTasks.checkMode = ConditionList.ConditionsCheckMode.AllTrueRequired;
        }

        conditionTasks.OnValidate(ownerSystem);
        actionTasks.OnValidate(ownerSystem);


        conditionTasks.SetOwnerSystem(this.ownerSystem);
        actionTasks.SetOwnerSystem(this.ownerSystem);

        foreach (ConditionTask c in conditionTasks.conditions)
        {
            c.SetOwnerSystem(this.ownerSystem);
        }

        foreach (ActionTask a in actionTasks.actions)
        {
            a.SetOwnerSystem(this.ownerSystem);
        }
    }


    public override Task Duplicate(ITaskSystem newOwnerSystem)
    {
        var newConditionnedAction = (ConditionnedAction)base.Duplicate(newOwnerSystem);
        newConditionnedAction.actionTasks = (ActionList)actionTasks.Duplicate(newOwnerSystem);
        newConditionnedAction.conditionTasks = (ConditionList)conditionTasks.Duplicate(newOwnerSystem);

        return newConditionnedAction;
    }


    protected override void OnExecute()
    {
        conditionTasks.Enable(agent, blackboard);

        if (conditionTasks.CheckCondition(agent, blackboard))
            actionTasks.ExecuteAction(agent, blackboard);

        if (!actionTasks.isRunning && !repeat)
            EndAction();
    }


    protected override void OnUpdate()
    {
        if (conditionTasks.CheckCondition(agent, blackboard))
        {
            actionTasks.ExecuteAction(agent, blackboard);
        }

        if (!actionTasks.isRunning && !repeat)
            EndAction();
    }

    protected override void OnStop()
    {
        actionTasks.EndAction(null);
        conditionTasks.Disable();
    }

    protected override void OnPause()
    {
        actionTasks.PauseAction();
    }


#if UNITY_EDITOR
    protected override void OnTaskInspectorGUI()
    {
        repeat = GUILayout.Toggle(repeat, "repeat");

        foreach (ConditionTask c in conditionTasks.conditions)
        {
            c.SetOwnerSystem(this.ownerSystem);
        }

        foreach (ActionTask a in actionTasks.actions)
        {
            a.SetOwnerSystem(this.ownerSystem);
        }

        GUILayout.Label("Conditions");
        conditionTasks.ShowListGUI();
        conditionTasks.ShowNestedConditionsGUI();

        GUILayout.Space(10);
        GUILayout.Label("Actions");
        actionTasks.ShowListGUI();
        actionTasks.ShowNestedActionsGUI();
    }




    public void DoSavePreset()
    {
#if !UNITY_WEBPLAYER
        var path = EditorUtility.SaveFilePanelInProject("Save Preset", "", "ConditionnedAction", "");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, JSONSerializer.Serialize(typeof(ConditionnedAction), this, true)); //true for pretyJson
            AssetDatabase.Refresh();
        }
#else
            Debug.LogWarning("Preset saving is not possible with WebPlayer as active platform");
#endif
    }

    public void DoLoadPreset()
    {
#if !UNITY_WEBPLAYER
        var path = EditorUtility.OpenFilePanel("Load Preset", "Assets", "ConditionnedAction");
        if (!string.IsNullOrEmpty(path))
        {
            var json = System.IO.File.ReadAllText(path);
            var list = JSONSerializer.Deserialize<ConditionnedAction>(json);
            this.actionTasks = list.actionTasks;
            this.conditionTasks = list.conditionTasks;

            this.actionTasks.SetOwnerSystem(this.ownerSystem);
            this.conditionTasks.SetOwnerSystem(this.ownerSystem);

            foreach (var a in this.actionTasks.actions)
            {
                a.SetOwnerSystem(this.ownerSystem);
            }

            foreach (var c in this.conditionTasks.conditions)
            {
                c.SetOwnerSystem(this.ownerSystem);
            }
        }
#else
            Debug.LogWarning("Preset loading is not possible with WebPlayer as active platform");
#endif
    }

#endif


}
