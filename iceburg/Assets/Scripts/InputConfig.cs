
using UnityEngine;

[CreateAssetMenu(fileName = "InputConfig", menuName = "Config/InputConfig", order = 1)]
public class InputConfig : ScriptableObject
{
  public string moveForwardBack = "Vertical";
  public string moveRightLeft = "Horizontal";
  public string camPan ="Mouse X";
  public string camPitch ="Mouse Y";
  public string camZoom ="Mouse ScrollWheel";
  public string toggleZoom = "";
  public string interact = "A_1";
  public bool useMouse = false;
  public float camZoomSensitivity = 1;
  public float camTurnSensitivity = 1;
}
