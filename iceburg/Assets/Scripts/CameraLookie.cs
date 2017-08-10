using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookie : MonoBehaviour
{
		public Transform cam;

    public void LateUpdate()
    {
    	if (cam != null)
    	{
      	transform.rotation = Quaternion.LookRotation(-cam.forward);
      }
    }
}
