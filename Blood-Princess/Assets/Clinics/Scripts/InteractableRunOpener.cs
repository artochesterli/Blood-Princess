using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clinic;

public class InteractableRunOpener : Interactable
{
	protected override void OnCancelInteract()
	{
		return;
	}

	protected override void OnEnterZone()
	{
		transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
		return;
	}

	protected override void OnExitZone()
	{
		transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
	}

	protected override void OnInteract()
	{
		Services.GameStateManager.EnterRunState();
	}
}
