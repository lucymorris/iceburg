using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
	// TODO: get rid of this string and pass in a string form the dialog system as the FormatDialog format
	public string dialogText;
	public string completeDialogText;

	public abstract void SetOwner(NPC owner);
	public abstract string FormatDialog(string format);
	public abstract void Activate();
	public abstract bool IsComplete();
	public abstract void PostCompletion();
}

public class NPC : MonoBehaviour
{
	public List<Objective> objectives = new List<Objective>();

	Rigidbody thisRigidbody;

	int currentObjectiveIndex = 0;

	public void Awake()
	{
		thisRigidbody = GetComponent<Rigidbody>();
	}

	public void Start()
	{
		for (int i = 0; i < objectives.Count; ++i)
		{
			objectives[i].SetOwner(this);
		}
		currentObjectiveIndex = -1;
		ActivateNextObjective();
	}

	public void CompleteObjective(Objective what)
	{
		if (objectives.IndexOf(what) == currentObjectiveIndex)
		{
			what.PostCompletion();

			string objectiveText = what.FormatDialog(what.completeDialogText); 
			Debug.LogFormat("NPC ({0}): {1}", name, objectiveText);

			StartCoroutine(YayCoroutine());

			ActivateNextObjective();
		}
	}

	public void ActivateNextObjective()
	{
		currentObjectiveIndex++;
		if (currentObjectiveIndex < objectives.Count)
		{
			Objective next = objectives[currentObjectiveIndex];
			next.Activate();

			string objectiveText = next.FormatDialog(next.dialogText); 
			Debug.LogFormat("NPC ({0}): {1}", name, objectiveText);
		}
		else
		{
			Debug.LogFormat("NPC ({0}): All objective complete", name);
		}
	}

	IEnumerator YayCoroutine()
	{
		Quaternion startRotation = thisRigidbody.rotation;
		float t = 0;
		while (t < 1.0f)
		{
			t += Time.deltaTime;

			Quaternion newRotation = startRotation * Quaternion.AngleAxis(t * 360, transform.up);
			thisRigidbody.MoveRotation(newRotation);
			yield return 0;
		}
	}
}
