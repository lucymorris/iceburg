using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CompleteEvent : UnityEvent
{
}

public abstract class Objective : MonoBehaviour
{
	// TODO: get rid of this string and pass in a string form the dialog system as the FormatDialog format
	public string dialogText;
	public string completeDialogText;

	public CompleteEvent complete;

	public abstract void SetOwner(NPC owner);
	public abstract string FormatDialog(string format);
	public abstract void Activate();
	public abstract bool IsComplete();
	public abstract void PostCompletion();
}