using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpeedManager))]
public class GravityManager : MonoBehaviour
{
	public float Gravity = 9.8f;
	private SpeedManager m_SpeedManager;

	private void Awake()
	{
		m_SpeedManager = GetComponent<SpeedManager>();
	}

	// Update is called once per frame
	void Update()
	{
		m_SpeedManager.SelfSpeed.y -= Gravity * Time.deltaTime * 10f;
	}
}
