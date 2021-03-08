using UnityEngine;
using Cinemachine;

public class InputOnMouseDown : MonoBehaviour, AxisState.IInputAxisProvider
{
    public string HorizontalInput = "Mouse X";
    public string VerticalInput = "Mouse Y";

    public float GetAxisValue(int axis)
    {
        // No input unless left mouse is down
        if (!Input.GetMouseButton(0))
            return 0;

        switch (axis)
        {
            case 0: return Input.GetAxis(HorizontalInput);
            case 1: return Input.GetAxis(VerticalInput);
            default: return 0;
        }
    }
}
