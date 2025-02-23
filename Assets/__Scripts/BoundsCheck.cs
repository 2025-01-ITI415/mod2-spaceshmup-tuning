using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject on screen.
/// Works only for an orthographic Main Camera at [0,0,0].
/// </summary>
public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenLocs
    {
        onScreen = 0,
        offRight = 1,
        offLeft = 2,
        offUp = 4,
        offDown = 8
    }

    public enum eType { center, inset, outset };

    [Header("Inscribed")]
    public eType boundsType = eType.center;

    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public eScreenLocs screenLocs = eScreenLocs.onScreen;
    public float camWidth;
    public float camHeight;

    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        float checkRadius = (boundsType == eType.inset) ? -radius :
                            (boundsType == eType.outset) ? radius : 0;

        Vector3 pos = transform.position;
        offRight = offLeft = offUp = offDown = false;
        screenLocs = eScreenLocs.onScreen;

        if (pos.x > camWidth + checkRadius)
        {
            offRight = true;
            pos.x = camWidth + checkRadius;
        }
        if (pos.x < -camWidth - checkRadius)
        {
            offLeft = true;
            pos.x = -camWidth - checkRadius;
        }
        if (pos.y > camHeight + checkRadius)
        {
            offUp = true;
            pos.y = camHeight + checkRadius;
        }
        if (pos.y < -camHeight - checkRadius)
        {
            offDown = true;
            pos.y = -camHeight - checkRadius;
        }

        // Update screenLocs using bitwise OR
        if (offRight) screenLocs |= eScreenLocs.offRight;
        if (offLeft) screenLocs |= eScreenLocs.offLeft;
        if (offUp) screenLocs |= eScreenLocs.offUp;
        if (offDown) screenLocs |= eScreenLocs.offDown;

        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            offRight = offLeft = offUp = offDown = false;
            screenLocs = eScreenLocs.onScreen;
        }
    }

    public bool isOnScreen
    {
        get { return !(offRight || offLeft || offUp || offDown); }
    }
    public bool LocIs( eScreenLocs checkLoc)
    {
        if (checkLoc == eScreenLocs.onScreen) return isOnScreen;
        return ((screenLocs & checkLoc) == checkLoc);
    }
    // Draws the bounds in the Scene view
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Camera.main.transform.position, boundSize);
    }
}
