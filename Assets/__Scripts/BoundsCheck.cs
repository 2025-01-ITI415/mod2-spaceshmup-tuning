using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject on screen.
/// Note that this ONLY works for an orthographic Main Camera at [0,0,0].
/// </summary>
public class BoundsCheck : MonoBehaviour {
    public enum eType { center, inset, outset };
    [Header("Inscribed")]
    public eType boundsType = eType.center;
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
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
        float checkRadius = 0;
            if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        if (pos.x > camWidth + checkRadius)
        {
            pos.x = camWidth + checkRadius;
            offRight = true;
        }

        if (pos.x < -camWidth - checkRadius)
        {
            pos.x = -camWidth - checkRadius;
            offLeft = true;
        }

        if (pos.y > camHeight + checkRadius)
        {
            pos.y = camHeight + checkRadius;
            offUp = true;
        }

        if (pos.y < -camHeight - checkRadius)
        {
            pos.y = -camHeight - checkRadius;
            offDown = true;
        }

        isOnScreen = !(offRight || offLeft || offUp || offDown);
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }

    }

    // Draw the bounds in the Scene pane using OnDrawGizmos()
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
