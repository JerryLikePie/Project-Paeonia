using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    
    [SerializeField] public Vector3 targetLocation;
    Unit selectedUnit;
    Unit lastSelectedUnit;
    Unit draggingUnit;
    public GameObject trail;
    GameObject tempTrail;
    bool firstTime = true;
    Hex selectedTile;
    Vector3 previousTile;
    [SerializeField] public GameObject tileSelectedGoldenHex;
    int[] change_hang = { 0, 0, 1, 1, -1, -1};
    int[] change_lie = { -1, 1, 0, 1 , 0, 1,
                       -1, 1, -1, 0, - 1, 0 };//分奇偶层，当前坐标的“六个可能的下个坐标”
    

    //然后是关于移动相机的
    public bool isDragging;
    private readonly float MouseZoomSpeed = 15.0f, TouchZoomSpeed = 0.08f;
    private readonly float ZoomMinBound = 25f, ZoomMaxBound = 60f, 
        MIN_Y = 60f, MAX_Y = 60.1f, 
        MAX_X = 1000f,MIN_X = 0f,
        MIN_Z = -40f,MAX_Z = 135f;
    private Vector3 Origin;
    private Vector3 Difference;
    private bool Drag = false;
    public bool isDraggingUI = false;
    bool isOnSelectedUnit = false;
    Ray ray;
    RaycastHit[] f;

    public Camera cam,cam2,cam3;
    Timer moveTimer = new Timer();
    void Start()
    {
        cam = cam.GetComponent<Camera>();
        cam2 = cam2.GetComponent<Camera>();
        cam3 = cam3.GetComponent<Camera>();
        moveTimer.IsCounting = false;
    }

    //bool unitInMoveCooldown = false; // 是否正在cd
    private Vector3 mouseDownPos1,mouseDownPos2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = false;
            mouseDownPos1 = Input.mousePosition;
            isOnSelectedUnit = SeeIfUnitDrag();
        }
        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(mouseDownPos1, Input.mousePosition) > 10f)
            {
                isDragging = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (tempTrail != null && tempTrail.activeSelf)
            {
                Destroy(tempTrail);
            }
            firstTime = true;
            isOnSelectedUnit = false;
            mouseDownPos2 = Input.mousePosition;
            if (Vector3.Distance(mouseDownPos1, mouseDownPos2) > 10f)
            {
                isDragging = true;
            }
            if (isDragging == false && !EventSystem.current.IsPointerOverGameObject())
            {
                MouseClicked();
            }
        }
        CameraDrag();
    }
    bool SeeIfUnitDrag()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        f = Physics.RaycastAll(ray);
        for (int i = 0; i < f.Length; i++)
        {
            try
            {
                GameObject targetObject = f[i].transform.parent.gameObject;
                if (targetObject.GetComponent<Unit>() != null)//如果目前鼠标在单位上面
                {
                    if (targetObject.GetComponent<Unit>().unitSelection)
                    {
                        draggingUnit = targetObject.GetComponent<Unit>();
                        return true;
                    }
                }
            }
            catch
            {
                
            }
        }
        return false;
    }
    void CameraDrag()
    {
        if (!isDraggingUI && !EventSystem.current.IsPointerOverGameObject() && !isOnSelectedUnit)//如果不在UI上
        {
            if (Input.touchSupported)//如果是触屏
            {
                if (Input.touchCount == 1)//如果只有一个手指
                {
                    if (Input.GetMouseButton(0))//那么一个手指就是拖动
                    {
                        Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                        if (Drag == false)
                        {
                            Drag = true;
                            Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                    }
                    else
                    {
                        Drag = false;
                    }
                    if (Drag == true)
                    {
                        Camera.main.transform.position = Origin - Difference;
                        //Camera.main.transform.position = new Vector3(
                        //  Mathf.Clamp(Camera.main.transform.position.x, MIN_X, MAX_X),
                        //  Mathf.Clamp(Camera.main.transform.position.y, MIN_Y, MAX_Y),
                        //  Mathf.Clamp(Camera.main.transform.position.z, MIN_Z, MAX_Z));
                    }
                }
                else if (Input.touchCount == 2)//如果是两个手指
                {

                    // get current touch positions
                    Touch tZero = Input.GetTouch(0);
                    Touch tOne = Input.GetTouch(1);
                    // get touch position from the previous frame
                    Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                    Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                    float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                    float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                    // get offset value
                    float deltaDistance = oldTouchDistance - currentTouchDistance;
                    Zoom(deltaDistance, TouchZoomSpeed);
                }
            }
            else//如果不是触屏
            {
                if (Input.GetMouseButton(0))//那就直接拖动吧
                {
                    Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                    if (Drag == false)
                    {
                        Drag = true;
                        Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
                else
                {
                    Drag = false;
                }
                if (Drag == true)
                {
                    Camera.main.transform.position = Origin - Difference;
                    //Camera.main.transform.position = new Vector3(
                    //      Mathf.Clamp(Camera.main.transform.position.x, MIN_X, MAX_X),
                    //      Mathf.Clamp(Camera.main.transform.position.y, MIN_Y, MAX_Y),
                    //      Mathf.Clamp(Camera.main.transform.position.z, MIN_Z, MAX_Z));
                }
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                Zoom(scroll, MouseZoomSpeed);
            }
        }
        else if (isOnSelectedUnit && isDragging)//如果在一个已选中的角色上面
        {
            dragUnit();
        }
        if (cam.orthographicSize < ZoomMinBound)
        {
            cam.orthographicSize = 25f;
            cam2.orthographicSize = 25f;
            cam3.orthographicSize = 25f;
        }
        else
        if (cam.orthographicSize > ZoomMaxBound)
        {
            cam.orthographicSize = 90f;
            cam2.orthographicSize = 90f;
            cam3.orthographicSize = 90f;
        }
    }
    void dragUnit()
    {
        if (SeeIfUnitDrag() && selectedUnit.path.Count != 0)
        {
            selectedUnit.path.Clear();
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        f = Physics.RaycastAll(ray);
        if (firstTime)
        {
            tempTrail = Instantiate(trail, draggingUnit.transform.position, Quaternion.identity);
            firstTime = false;
            previousTile = draggingUnit.transform.position;
        }
        for (int i = 0; i < f.Length; i++)
        {
            try
            {
                GameObject targetObject = f[i].transform.parent.gameObject;
                if (targetObject.GetComponent<Hex>() != null)
                {
                    if (Vector3.Distance(f[i].point, targetObject.transform.position) < 8
                        && !selectedUnit.path.Contains(targetObject.GetComponent<Hex>())
                        && Vector3.Distance(previousTile, targetObject.transform.position) < 17.5
                        && Vector3.Distance(previousTile, targetObject.transform.position) > 2)
                    {
                        previousTile = targetObject.transform.position;
                        selectedUnit.path.Enqueue(targetObject.GetComponent<Hex>());
                        tempTrail.transform.position = targetObject.transform.position;
                        //trail.GetComponentInChildren<TrailRenderer>().AddPosition(targetObject.transform.position);
                    }
                }
            }
            catch
            {
            }
        }
    }
    void Zoom(float deltaMagnitudeDiff, float speed)
    {

        cam.orthographicSize += deltaMagnitudeDiff * speed;
        cam2.orthographicSize += deltaMagnitudeDiff * speed;
        cam3.orthographicSize += deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, ZoomMinBound, ZoomMaxBound);
        cam2.orthographicSize = Mathf.Clamp(cam.orthographicSize, ZoomMinBound, ZoomMaxBound);
        cam3.orthographicSize = Mathf.Clamp(cam.orthographicSize, ZoomMinBound, ZoomMaxBound);
    }
    void MouseClicked()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        f = Physics.RaycastAll(ray);
        for (int i = 0; i < f.Length; i++)
        {
            try
            {
                GameObject targetObject = f[i].transform.parent.gameObject;
                if (targetObject.GetComponent<Unit>() != null)//如果目前鼠标在单位上面
                {
                    MouseClickUnit(targetObject);
                    return;
                }
            }
            catch
            {
            }
        }
        if (selectedUnit != null)
        {
            selectedUnit.unitSelection = false;
            selectedUnit.selectionBox.SetActive(false);
            selectedUnit = null;
        }
    }
    void MouseClickMap(GameObject targetObject)
    {
        lastSelectedUnit.selectionBox.SetActive(false);
        lastSelectedUnit = selectedUnit;
        Debug.Log(selectedTile.name);
        if (selectedUnit != null)
        {
            if (selectedUnit.unitSelection)
            {
                lastSelectedUnit.unitSelection = false;
                if (IsAround(selectedUnit, selectedTile) && selectedUnit.canMove)//这个tile是在这个unit的六周之内吗？
                {
                }
            }
        }
    }
    public void UpdateMap(Unit selectedUnit)
    {
        //selectedUnit.GetComponent<DollsCombat>().FogOfar();
    }
    void MouseClickUnit(GameObject targetObject)
    {
        selectedUnit = targetObject.GetComponent<Unit>();
        if (selectedUnit.canBeSelected)
        {
            selectedUnit.unitSelection = !selectedUnit.unitSelection;
            if (lastSelectedUnit != null && selectedUnit != lastSelectedUnit)
            {
                lastSelectedUnit.unitSelection = false;
                lastSelectedUnit.selectionBox.SetActive(false);
                lastSelectedUnit = selectedUnit;
            }
            else
            {
                lastSelectedUnit = selectedUnit;
            }
            
            if (selectedUnit.unitSelection == true)//颜色控制，如果是选中就变红不然就是白色
            {
                selectedUnit.selectionBox.SetActive(true);
            }
            else
            {
                selectedUnit.selectionBox.SetActive(false);
                selectedUnit = null;
            }
        }
    }
    bool IsAround(Unit selectedUnit, Hex selectedTile)
    {
        int current_hang = selectedUnit.hang;
        int current_lie = selectedUnit.lie;
        bool walkable = false;//默认是不能走的
        for (int i = 0; i < 6; i++)//查看左右，上左上右，下左下右，周围的六个格子
        {
            int next_hang = current_hang + change_hang[i];
            if (current_hang % 2 == 0)
            {
                int next_lie = current_lie + change_lie[i];
                if (next_hang == selectedTile.X && next_lie == selectedTile.Z && selectedTile.haveUnit == false && selectedTile.haveEnemy == false)
                {
                    walkable = true;//如果我点的tile的坐标是在这6个数中间，那就可以走
                }
            }
            else
            {
                int next_lie = current_lie + change_lie[i+6];
                if (next_hang == selectedTile.X && next_lie == selectedTile.Z && selectedTile.haveUnit == false && selectedTile.haveEnemy == false)
                {
                    walkable = true;//如果我点的tile的坐标是在这6个数中间，那就可以走
                }
            }
        }
        return walkable;
    }
}

