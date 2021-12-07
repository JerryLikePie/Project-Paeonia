using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utilities;

public class Unit : MonoBehaviour
{
    Vector3 destination;
    Hex next;
    [HideInInspector]
    public int hang,lie,tempHang,tempLie;
    public float movementCooldown;
    [HideInInspector]
    public float moveTime, nextTileMovecost;
    [HideInInspector]
    public long timeStart; // ����cd��ʱ��
    [HideInInspector]
    public bool canBeSelected;
    [HideInInspector]
    public bool canMove;

    float percentageCDtime;
    GameObject doll;
    public GameObject tileSelectedGoldenHex;
    public bool unitSelection, isMoving = false;
    public Slider actionCDBar;
    public GameObject CDBarCanvas;
    public GameObject selectionBox;
    public GameObject skillBox;
    public bool unitInMoveCooldown = false;
    public AudioSource engineSound;
    Timer moveTimer = new Timer();
    bool firstTime = true, firstTime2 = false;
    DollsProperty dolls;
    public Queue<Hex> path = new Queue<Hex>();
    DollsCombat combat;

    // Start is called before the first frame update
    void Start()
    {
        dolls = this.GetComponent<DollsProperty>();
        combat = GetComponent<DollsCombat>();
        canMove = false;
        selectionBox.SetActive(false);
        destination = transform.position;
        unitSelection = false;
        moveTimer.IsCounting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dolls.dolls_type != 3)
        {
            if (path.Count != 0 && !Input.GetMouseButton(0) && !path.Peek().haveEnemy)
            {
                if (firstTime)
                {
                    next = path.Peek();
                    GameObject obj = Instantiate(tileSelectedGoldenHex, next.transform.position, Quaternion.identity);//�ڴ˴�����һ������
                    moveTime = movementCooldown * next.movecost;
                    Destroy(obj, moveTime);
                    firstTime = false;
                    Move();
                }
                if (canMove)
                {
                    GameObject.Find("Map" + hang + "_" + lie).GetComponent<Hex>().haveUnit = false;
                    destination = next.transform.position;
                    path.Dequeue();
                    canMove = false;
                    firstTime2 = true;
                }
            }
            if (!Vector3Equal(destination, transform.position))
            {
                MoveThroughPath();
            }
            else if (firstTime2)
            {
                combat.deFogOfWar();
                hang = next.X;
                lie = next.Z;
                combat.height = next.height;
                combat.dodgeBuff = next.dodgeBuff;
                combat.rangeBuff = next.rangeBuff;
                next.haveUnit = true;
                
                combat.CheckStatus();
                combat.FogOfWar();
                firstTime2 = false;
                firstTime = true;
            }
            moveTimer.TimerUpdate();
            if (actionCDBar.value >= 1f)
                CDBarCanvas.SetActive(false);
            else
                CDBarCanvas.SetActive(true);
            if (unitSelection)
            {
                engineSound.volume = 0.15f;
            }
            else
            {
                engineSound.volume = 0f;
            }
        }
    }
    public void MoveThroughPath()
    {
        Vector3 direction = destination - transform.position;
        Vector3 velocity = direction.normalized * (direction.magnitude / moveTime);
        //velocity = Vector3.ClampMagnitude(velocity, direction.magnitude);
        
        transform.Translate(velocity);
    }
    public void Move()
    {
        canMove = false;
        moveTimer.IsCounting = true;
        moveTimer.timeStart = System.DateTime.Now.Ticks;
        moveTimer.TimeToWait = moveTime;
        moveTimer.unit = this;
    }

}
