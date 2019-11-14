using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clinic
{
	public class Interactable : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Player"))
			{
				print("Player Enter Trigger");
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.collider.CompareTag("Player"))
			{
				print("Player Enter Collision");
			}
		}
	}

}
