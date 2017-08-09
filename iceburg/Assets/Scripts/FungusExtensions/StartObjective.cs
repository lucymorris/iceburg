using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Fungus;


// TODO(elliot): jump to different target blocks depending on question result
// TODO(elliot): output quest results into variables

[CommandInfo("Narrative", 
             "Objective", 
             "Starts an objective")]
[AddComponentMenu("")]
public class StartObjective : Command //, ILocalizable
{
    // [Tooltip("Text to display on the menu button")]
    // [SerializeField] protected string text = "";

    // [Tooltip("Notes about the option text for other authors, localization, etc.")]
    // [SerializeField] protected string description = "";

    // [Tooltip("NOT IMPLEMENTED YET! Block to execute when this option is selected")]
    // [SerializeField] protected Block targetBlockSuccess;

    // [Tooltip("Hide this option if the target block has been executed previously")]
    // [SerializeField] protected bool skipIfVisited;

    // [Tooltip("If false, the menu option will be displayed but will not be selectable")]
    // [SerializeField] protected BooleanData interactable = new BooleanData(true);

   // [Tooltip("A custom Menu Dialog to use to display this menu. All subsequent Menu commands will use this dialog.")]
    // [SerializeField] protected MenuDialog setMenuDialog;

    [Tooltip("The NPC who will own the objective.")]
    [SerializeField] protected NPC npc;

    [Tooltip("The objective to start.")]
    [SerializeField] protected DepositObjective objective;

    [Tooltip("How many?")]
    [SerializeField] protected IntegerData requiredAmount = new IntegerData(1);

    [Tooltip("What?")]
    [SerializeField] protected Holdable.ItemType requiredType;

    #region Public members

    //public MenuDialog SetMenuDialog  { get { return setMenuDialog; } set { setMenuDialog = value; } }

    public override void OnEnter()
    {
        // if (setMenuDialog != null)
        // {
        //     // Override the active menu dialog
        //     MenuDialog.ActiveMenuDialog = setMenuDialog;
        // }

        //bool hideOption = (skipIfVisited && targetBlockSuccess != null && targetBlockSuccess.GetExecutionCount() > 0);

        // if (!hideOption)
        // {
        //     var menuDialog = MenuDialog.GetMenuDialog();
        //     if (menuDialog != null)
        //     {
        //         menuDialog.SetActive(true);

        //         var flowchart = GetFlowchart();
        //         string displayText = flowchart.SubstituteVariables(text);

        //         menuDialog.AddOption(displayText, interactable, targetBlockSuccess);
        //     }
        // }

        objective.requiredAmount = requiredAmount;
        objective.requiredType = requiredType;

        objective.complete.AddListener(HandleComplete);
        npc.StartObjective(objective);
    }

    public override void OnExit()
    {
        objective.complete.RemoveListener(HandleComplete);
    }

    private void HandleComplete()
    {
        Continue();
    }

    // public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
    // {
    //     if (targetBlockSuccess != null)
    //     {
    //         connectedBlocks.Add(targetBlockSuccess);
    //     }
    // }

    public override string GetSummary()
    {
        // if (targetBlockSuccess == null)
        // {
        //     return "Error: No target block selected";
        // }

        // if (text == "")
        // {
        //     return "Error: No button text selected";
        // }

        if (objective == null)
        {
            return "Error: no objective set";
        }

        if (npc == null)
        {
            return "Error: no NPC set";
        }

        return "Start " + objective.name + " with " + npc.name;
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 210, 184, 255);
    }

    #endregion

    #region ILocalizable implementation

    // public virtual string GetStandardText()
    // {
    //     return text;
    // }

    // public virtual void SetStandardText(string standardText)
    // {
    //     text = standardText;
    // }
    
    // public virtual string GetDescription()
    // {
    //     return description;
    // }
    
    // public virtual string GetStringId()
    // {
    //     return "QUEST." + GetFlowchartLocalizationId() + "." + itemId;
    // }

    #endregion
}