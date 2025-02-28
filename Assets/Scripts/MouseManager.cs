using Assets.Scripts.Utils;
using System;
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

    // 地图最大行列数
    private int MAX_ROWS;
    private int MAX_COLS;
    // 整张地图对象
    private List<List<GameObject>> mapTiles;

    //然后是关于移动相机的
    public bool isDragging;
    private readonly float MouseZoomSpeed = 15.0f, TouchZoomSpeed = 0.08f;
    private readonly float ZoomMinBound = 25f, ZoomMaxBound = 60f,
        MIN_Y = 100f, MAX_Y = 200f,
        MAX_X = 1000f, MIN_X = 0f,
        MIN_Z = -40f, MAX_Z = 135f;
    private Vector3 Origin;
    private Vector3 Difference;
    private bool Drag = false;
    public bool isDraggingUI = false;
    bool isOnSelectedUnit = false;
    Ray ray;
    RaycastHit[] f;

    // 从地图格到相机的方向向量
    private Vector3 invCameraDirection = new Vector3(-Mathf.Sqrt(3) / 2, 1, -3f / 2);
    // 当前相机瞄准的 Tile
    private Vector2Int cameraLookAt;

    // 当前显示范围
    [SerializeField, FieldName("相机初始位置")]
    Vector2Int horizonCenter = new Vector2Int(3, 3);
    [SerializeField, FieldName("左下角距中心偏移")]
    Vector2Int horizonTL = new Vector2Int(-6, -8);
    [SerializeField, FieldName("右上角距中心偏移")]
    Vector2Int horizonBR = new Vector2Int(6, 8);

    public Camera cam,cam2,cam3;
    Timer moveTimer = new Timer();
    void Start()
    {
        cam = cam.GetComponent<Camera>();
        cam2 = cam2.GetComponent<Camera>();
        cam3 = cam3.GetComponent<Camera>();
        moveTimer.IsCounting = false;
        animatingTiles = new List<TileAnimation>();
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
        updateCameraLookAt();
        animateOnUpdate();
    }

    private List<TileAnimation> removeList = new List<TileAnimation>();  // 待移除的元素

    private void animateOnUpdate()
    {
        //try
        //{
        //    // 处理地图格渐入渐出动画
        //    foreach (TileAnimation anim in animatingTiles)
        //    {
        //        // 渐入
        //        if (anim.target == TileAnimation.T_DOWN)
        //        {
        //            anim.obj.transform.position += Vector3.down * TILE_ANIM_SPEED * Time.deltaTime; // deltaTime 是频率的倒数
        //            if (anim.obj.transform.position.y <= 0f)
        //            {
        //                // 复位，解决错位问题
        //                anim.obj.transform.position = new Vector3(anim.obj.transform.position.x, 0, anim.obj.transform.position.z);
        //                removeList.Add(anim);
        //            }
        //        }
        //        // 渐隐
        //        else if (anim.target == TileAnimation.T_UP)
        //        {
        //            anim.obj.transform.position += Vector3.up * TILE_ANIM_SPEED * Time.deltaTime;
        //            if (anim.obj.transform.position.y >= TILE_ANIM_END_POS) // TILE_ANIM_END_POS 为动画结束位置
        //            {
        //                // 复位，解决错位问题
        //                anim.obj.transform.position = new Vector3(anim.obj.transform.position.x, TILE_ANIM_END_POS, anim.obj.transform.position.z);
        //                if (anim.obj.transform.childCount > 0)
        //                {
        //                    anim.obj.transform.GetChild(0).gameObject.SetActive(false);
        //                }
        //                removeList.Add(anim);
        //            }
        //        }
        //    }
        //    // 批量删除
        //    foreach (TileAnimation anim in removeList)
        //    {
        //        animatingTiles.Remove(anim);
        //    }
        //}
        //catch
        //{

        //}
    }

    // 初始化地图时设置相机位置
    public void setCameraLookAt(GameObject hexLookAt)
    {
        // 相机中心指向 hexLookAt
        // 其中 UP * 0.6f 是 hex 高度的偏移量
        Camera.main.transform.position = hexLookAt.transform.position + (Vector3.up * 0.6f) + (invCameraDirection * 150f);
        cameraLookAt = Utilities.tileNameToPos(hexLookAt.transform.name);
        horizonCenter = cameraLookAt;
    }

    // 初始化地图时告知地图参数
    public void setMapInfo(int maxRows, int maxCols, List<List<GameObject>> mapTiles)
    {
        this.MAX_ROWS = maxRows;
        this.MAX_COLS = maxCols;
        this.mapTiles = mapTiles;
        // 初始化视野
        Vector2Int TopLeft = horizonCenter + horizonTL;
        Vector2Int BottomRight = horizonCenter + horizonBR;
        for (int row = Math.Max(0, TopLeft.x); row <= Math.Min(MAX_ROWS - 1, (int)BottomRight.x); row++)
        {
            for (int col = Math.Max(0, TopLeft.y); col <= Math.Min(MAX_COLS - 1, (int)BottomRight.y); col++)
            {
                if (mapTiles[row][col].transform.childCount > 0)
                {
                    mapTiles[row][col].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    // 每次刷新时更新相机状态
    public void updateCameraLookAt()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            //绘制相机射线线，仅在Scene视图中可见
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            //输出射线探测到的物体的名称
            if (hit.transform.name.Equals("Hex") && hit.transform.parent != null)
            {
                // Debug.Log("射线探测到的物体：" + hit.transform.parent.name);
                // string tileName = hit.transform.parent.name;
                Vector2Int pos = Utilities.tileNameToPos(hit.transform.parent.name);
                int dRow = pos.x - cameraLookAt.x;
                int dCol = pos.y - cameraLookAt.y;
                cameraLookAt.x = pos.x;
                cameraLookAt.y = pos.y;
                // 调整显示视野
                if (dRow != 0 || dCol != 0)
                {
                    moveHorizon(dRow, dCol);
                }
            }
        }
    }

    // 调整视野范围
    private void moveHorizon(int dRow, int dCol)
    {
        // 前行号后列号
        Vector2Int dPos = new Vector2Int(dRow, dCol);
        Vector2Int oldTopLeft = horizonCenter + horizonTL;
        Vector2Int oldBottomRight = horizonCenter + horizonBR;
        Vector2Int newTopLeft = oldTopLeft + dPos;
        Vector2Int newBottomRight = oldBottomRight + dPos;
        HashSet<GameObject> tilesToShow = new HashSet<GameObject>();
        HashSet<GameObject> tilesToHide = new HashSet<GameObject>();
        // 修改视野
        horizonCenter += dPos;
        // 增加新行
        if (dRow > 0)
        {
            for (int row = Math.Max(0, oldBottomRight.x + 1); row <= Math.Min(MAX_ROWS - 1, newBottomRight.x); row++)
            {
                for (int col = Math.Max(0, newTopLeft.y); col <= Math.Min(MAX_COLS - 1, newBottomRight.y); col++)
                {
                    tilesToShow.Add(mapTiles[row][col]);
                }
            }
            for (int row = Math.Max(0, oldTopLeft.x); row < Math.Min(MAX_ROWS, newTopLeft.x); row++)
            {
                for (int col = Math.Max(0, oldTopLeft.y); col <= Math.Max(MAX_COLS - 1, oldBottomRight.y); col++)
                {
                    tilesToHide.Add(mapTiles[row][col]);
                }
            }
        }
        else if (dRow < 0)
        {
            for (int row = Math.Max(0, newTopLeft.x); row < Math.Min(MAX_ROWS, oldTopLeft.x); row++)
            {
                for (int col = Math.Max(0, newTopLeft.y); col <= Math.Min(MAX_COLS - 1, newBottomRight.y); col++)
                {
                    tilesToShow.Add(mapTiles[row][col]);
                }
            }
            for (int row = Math.Max(0, newBottomRight.x + 1); row <= Math.Min(MAX_ROWS - 1, oldBottomRight.x); row++)
            {
                for (int col = Math.Max(0, oldTopLeft.y); col <= Math.Min(MAX_COLS - 1, oldBottomRight.y); col++)
                {
                    tilesToHide.Add(mapTiles[row][col]);
                }
            }
        }
        // 增加新列
        if (dCol > 0)
        {
            for (int col = Math.Max(0, oldBottomRight.y + 1); col <= Math.Min(MAX_COLS - 1, newBottomRight.y); col++)
            {
                for (int row = Math.Max(0, newTopLeft.x); row <= Math.Min(MAX_ROWS - 1, newBottomRight.x); row++)
                {
                    tilesToShow.Add(mapTiles[row][col]);
                }
            }
            for (int col = Math.Max(0, oldTopLeft.y); col < Math.Min(MAX_COLS, newTopLeft.y); col++)
            {
                for (int row = Math.Max(0, oldTopLeft.x); row <= Math.Min(MAX_ROWS - 1, oldBottomRight.x); row++)
                {
                    tilesToHide.Add(mapTiles[row][col]);
                }
            }
        }
        else if (dCol < 0)
        {
            for (int col = Math.Max(0, newTopLeft.y); col < Math.Min(MAX_COLS, oldTopLeft.y); col++)
            {
                for (int row = Math.Max(0, newTopLeft.x); row <= Math.Min(MAX_ROWS - 1, newBottomRight.x); row++)
                {
                    tilesToShow.Add(mapTiles[row][col]);
                }
            }
            for (int col = Math.Max(0, newBottomRight.y + 1); col <= Math.Min(MAX_COLS - 1, oldBottomRight.y); col++)
            {
                for (int row = Math.Max(0, oldTopLeft.x); row <= Math.Min(MAX_ROWS - 1, oldBottomRight.x); row++)
                {
                    tilesToHide.Add(mapTiles[row][col]);
                }
            }
        }
        showTilesAnimated(tilesToShow);
        hideTilesAnimated(tilesToHide);
    }

    // 地图显隐动画
    private List<TileAnimation> animatingTiles;
    // 显隐动画开始、结束位置
    private readonly float TILE_ANIM_END_POS = 1.5f;
    // 显隐动画播放速度
    private readonly float TILE_ANIM_SPEED = 64f;

    // 地图格运动动画
    class TileAnimation
    {
        public static int T_DOWN = 0;
        public static int T_UP = 1;
        public GameObject obj;
        public int target; // 0 - down to earth; 1 - up to sky
    }

    // 渐入动画
    private void showTilesAnimated(IEnumerable<GameObject> tiles)
    {
        TileAnimation anim;
        foreach (GameObject tile in tiles)
        {
            if (tile.transform.childCount > 0)
            {
                tile.transform.GetChild(0).gameObject.SetActive(true);
            }
            if ((anim = animatingTiles.Find(o => o.obj == tile)) != null)
            {
                anim.target = TileAnimation.T_DOWN;
            }
            else
            {
                tile.transform.position += Vector3.up * TILE_ANIM_END_POS;
                animatingTiles.Add(new TileAnimation { obj = tile, target = TileAnimation.T_DOWN });
            }
        }
    }

    // 渐出动画
    private void hideTilesAnimated(IEnumerable<GameObject> tiles)
    {
        TileAnimation anim;
        foreach (GameObject tile in tiles)
        {
            if ((anim = animatingTiles.Find(o => o.obj == tile)) != null)
            {
                anim.target = TileAnimation.T_UP;
            }
            else
            {
                animatingTiles.Add(new TileAnimation { obj = tile, target = TileAnimation.T_UP });
            }
        }
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
            if (!Input.mousePresent)//如果是触屏
            {
                Debug.Log("移动中");
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
                        // 限制相机 Y 轴高度
                        float CY = Camera.main.transform.position.y;
                        if (CY < MIN_Y)
                        {
                            Camera.main.transform.position += invCameraDirection * 50f;
                        }
                        else if (CY > MAX_Y)
                        {
                            Camera.main.transform.position -= invCameraDirection * 50f;
                        }
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
                    // 限制相机 Y 轴高度
                    float CY = Camera.main.transform.position.y;
                    if (CY < MIN_Y)
                    {
                        Camera.main.transform.position += invCameraDirection * 50f;
                    }
                    else if (CY > MAX_Y)
                    {
                        Camera.main.transform.position -= invCameraDirection * 50f;
                    }
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
            selectedUnit.Select(false);
            selectedUnit.unitSelection = false;
            selectedUnit = null;
        }
    }
    void MouseClickMap(GameObject targetObject)
    {
        lastSelectedUnit.Select(false);
        lastSelectedUnit = selectedUnit;
        Debug.Log(selectedTile.name);
        if (selectedUnit != null)
        {
            if (selectedUnit.unitSelection)
            {
                lastSelectedUnit.unitSelection = false;
                //if (IsAround(selectedUnit, selectedTile) && selectedUnit.canMove)//这个tile是在这个unit的六周之内吗？
                //{
                //}
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
                lastSelectedUnit.Select(false);
                lastSelectedUnit = selectedUnit;
            }
            else
            {
                lastSelectedUnit = selectedUnit;
            }
            
            if (selectedUnit.unitSelection == true)//颜色控制，如果是选中就变红不然就是白色
            {
                selectedUnit.Select(true);
            }
            else
            {
                selectedUnit.Select(false);
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

