using UnityEngine;
using Fungus;

/// <summary>
/// The block will execute when the user clicks on the target UI button object.
/// </summary>
[EventHandlerInfo("Narritive",
                  "Objective Complete",
                  "Will execute when the targeted objective is completed.")]
[AddComponentMenu("")]
public class ObjectiveComplete : EventHandler
{   
    [Tooltip("Objective that should be waited for")]
    [SerializeField] protected Objective targetObjective;

    #region Public members

    public virtual void Start()
    {
        if (targetObjective != null)
        {
            targetObjective.complete.AddListener(HandleComplete);
        }
    }
    
    protected virtual void HandleComplete()
    {
        ExecuteBlock();
    }

    public override string GetSummary()
    {
        if (targetObjective != null)
        {
            return targetObjective.name;
        }

        return "None";
    }

    #endregion
}