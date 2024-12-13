using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {


    [SerializeField]
    private int yGridlines;

    internal int YGridlines {
        get { return yGridlines; }
        private set { yGridlines = value; }
    }

    [SerializeField]
    private int xGridlines;

    internal int XGridlines {
        get { return xGridlines; }
        private set { xGridlines = value; }
    }

    [SerializeField]
    private float gridlineSpacing;

    internal float GridlineSpacing {
        get { return gridlineSpacing; }
        private set { gridlineSpacing = value; }
    }


    [SerializeField]
    private GameObject gridDebug;

    [SerializeField]
    private Transform horizontalPreDigParent;

    [SerializeField]
    private Transform verticalPreDigParent;

    [SerializeField]
    private Transform debugGridlineParent;

    internal Transform DebugGridlineParent {
        get { return debugGridlineParent; }
        private set { debugGridlineParent = value; }
    }

    [SerializeField]
    private Transform DigMaskParent;

    [SerializeField]
    private GameObject DigMaskPrefab;

    [SerializeField, Tooltip("How close a position has to be to another for it to be considered connected / walkable")]
    private float disconnectedThreshold;

    internal float DisconnectedThreshold {
        get { return disconnectedThreshold; }
        private set { disconnectedThreshold = value; }
    }


    internal List<Vector2> dugPositions { get; private set; }

    void Start () {

        dugPositions = new List<Vector2>();

        foreach (Transform preDig in horizontalPreDigParent) {

            Vector2 nearestGridlines = GetNearestGridlines(preDig.position);

            DoDigAt(new Vector2(preDig.position.x, nearestGridlines.y));

            Destroy(preDig.gameObject);

        }

        foreach (Transform preDig in verticalPreDigParent) {

            Vector2 nearestGridlines = GetNearestGridlines(preDig.position);

            DoDigAt(new Vector2(nearestGridlines.x, preDig.position.y));

            Destroy(preDig.gameObject);

        }

    }
	
    internal void DoDigAt(Vector2 position) {

        if (position.y < -GridlineSpacing/8) {

            Instantiate(DigMaskPrefab, new Vector3(position.x, position.y, 1.5f), Quaternion.identity, DigMaskParent);
        }

        dugPositions.Add(position);

    }

    internal bool IsAlreadyDug(Vector2 pos) {
        foreach (Vector2 dig in dugPositions) {
            if (Vector2.Distance(dig, pos) < .5) {
                return true;
            }
        }
        return false;
    }

    internal bool IsConnected(Vector2 pos1, Vector2 pos2) {

        if (Vector2.Distance(pos1, pos2) < DisconnectedThreshold) {
            return true;
        }

        return false;

    }

    internal bool IsConnected(Vector2 pos1, Vector2 pos2, float disconnectedThreshold) {

        if (Vector2.Distance(pos1, pos2) < disconnectedThreshold) {
            return true;
        }

        return false;

    }

    private void PlaceGridDebugMarkers() {

        for (float x = 0;x > XGridlines * -gridlineSpacing;x -= gridlineSpacing) {
            for (float y = 0;y > YGridlines * -gridlineSpacing;y -= gridlineSpacing) {
                GameObject newGridDebug = Instantiate(gridDebug, DebugGridlineParent);
                newGridDebug.transform.position = new Vector3(x, y, 2);
            }
        }
    }

    internal Vector2 GetNearestGridlines(Vector3 position) {

        Vector2 nearestGridlines = new Vector2(1000, 1000);

        for (float x = 0;x > XGridlines * -gridlineSpacing;x -= gridlineSpacing) {
            if (Mathf.Abs(x - position.x) < Mathf.Abs(nearestGridlines.x - position.x)) {
                nearestGridlines.x = x;
            }
        }

        for (float y = 0;y > YGridlines * -gridlineSpacing;y -= gridlineSpacing) {
            if (Mathf.Abs(y - position.y) < Mathf.Abs(nearestGridlines.y - position.y)) {
                nearestGridlines.y = y;
            }
        }

        return nearestGridlines;

    }
}
