using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
	Rigidbody thisRigidbody;

	Objective currentObjective;

	public void Awake()
	{
		thisRigidbody = GetComponent<Rigidbody>();
	}

	public void CompleteObjective(Objective what)
	{
		if (what == currentObjective)
		{
			what.PostCompletion();

			Debug.LogFormat("NPC ({0}): completed {1}", name, what);

			StartCoroutine(YayCoroutine());

			currentObjective = null;
		}
		else
		{
			Debug.LogWarningFormat("NPC ({0}): can't complete objective {1} because it isn't the current objective {2}", name, what, currentObjective);
		}
	}

	public Objective StartObjective(Objective what)
	{
		if (currentObjective == null)
		{
			currentObjective = what;
			what.SetOwner(this);
			what.Activate();

			Debug.LogFormat("NPC ({0}): started {1}", name, what);
		}
		return currentObjective;
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
