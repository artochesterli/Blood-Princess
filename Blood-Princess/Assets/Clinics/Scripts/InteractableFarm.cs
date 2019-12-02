using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	[RequireComponent(typeof(Farm))]
	public class InteractableFarm : Interactable
	{
		private Farm m_Farm;
		protected override void Awake()
		{
			base.Awake();
			m_Farm = GetComponent<Farm>();
		}
		protected override void OnCancelInteract()
		{
			m_Farm.OnCloseUI();
		}

		protected override void OnInteract()
		{
			m_Farm.OnOpenUI();
		}
	}

}
