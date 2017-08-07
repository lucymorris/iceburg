using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expandy : MonoBehaviour
{    
    public float expandTime = 0.5f;
    public float maxScale = 1.0f;

    float scale;

    bool active;
    float becomeActiveTime;

    public void Show()
    {
      active = true;
    }

    public void Hide()
    {
    	active = false;
    }

    public void Update()
    {
      if (active)
      {
        if (becomeActiveTime == 0)
        {
          // Set initial progress
          becomeActiveTime = 0;
        }

        becomeActiveTime += Time.deltaTime;

        float m = Mathf.Clamp01(becomeActiveTime / expandTime);

        scale = Mathf.SmoothStep(0, maxScale, m);

        // activeRaidusIndicator.gameObject.SetActive(true);
      }
      else
      {
        becomeActiveTime -= Time.deltaTime;
        if (becomeActiveTime < 0)
        {
          becomeActiveTime = 0;
        }

        float m = Mathf.Clamp01(becomeActiveTime / expandTime);

        scale = Mathf.SmoothStep(0, maxScale, m);
      }

      Vector3 scaleVec = new Vector3(scale, scale, scale);
      transform.localScale = scaleVec;
    }
}
