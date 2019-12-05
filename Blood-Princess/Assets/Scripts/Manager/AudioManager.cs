using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
	private AudioScriptableObject m_AudioData;

	public AudioManager(AudioScriptableObject m_AudioData)
	{
		this.m_AudioData = m_AudioData;
	}

	public void Destroy()
	{

	}
}
