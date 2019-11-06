using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
	Follow
}

public class CameraManager : MonoBehaviour
{
	public CameraState cameraState;
	public GameObject Character;

	private int layermask;

	private float CameraViewWidth;
	private float CameraViewHeight;

	private float MaxY;
	private float MinY;
	private float MaxX;
	private float MinX;
	// Start is called before the first frame update
	void Start()
	{
		layermask = 1 << LayerMask.NameToLayer("Zone");
		var Camera = GetComponent<Camera>();
		CameraViewHeight = Camera.orthographicSize * 2;
		CameraViewWidth = Camera.orthographicSize * 2 * Camera.pixelWidth / Camera.pixelHeight;
		Character = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void LateUpdate()
	{
		GetRestrictInfo();
		SetCameraBehavior();
	}

	private void GetRestrictInfo()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, layermask))
		{
			Vector2 ZonePos = hit.collider.gameObject.transform.position;
			Vector2 ZoneOffset = hit.collider.gameObject.GetComponent<BoxCollider>().center;
			Vector2 ZoneSize = hit.collider.gameObject.GetComponent<BoxCollider>().size;

			MinX = ZonePos.x + ZoneOffset.x - ZoneSize.x / 2;
			MaxX = ZonePos.x + ZoneOffset.x + ZoneSize.x / 2;
			MinY = ZonePos.y + ZoneOffset.y - ZoneSize.y / 2;
			MaxY = ZonePos.y + ZoneOffset.y + ZoneSize.y / 2;
		}
	}

	private void SetCameraBehavior()
	{
		switch (cameraState)
		{
			case CameraState.Follow:

				Vector3 Target = new Vector3(Character.transform.position.x, Character.transform.position.y, transform.position.z);
				if (!CharacterOutofCamera())
				{
					Target.x = Utility.GetConstraintValue(Target.x, MinX + CameraViewWidth / 2, MaxX - CameraViewWidth / 2);
					Target.y = Utility.GetConstraintValue(Target.y, MinY + CameraViewHeight / 2, MaxY - CameraViewHeight / 2);
				}
				transform.position = Target;

				break;
		}
	}

	private bool CharacterOutofCamera()
	{
		return Character.transform.position.x > transform.position.x + CameraViewWidth / 2 || Character.transform.position.x < transform.position.x - CameraViewWidth / 2
			|| Character.transform.position.y > transform.position.y + CameraViewHeight / 2 || Character.transform.position.y < transform.position.y - CameraViewHeight / 2;
	}
}
