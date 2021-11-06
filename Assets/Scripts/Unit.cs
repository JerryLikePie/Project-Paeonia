using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public Vector3 destination;
    [HideInInspector]
    public int hang,lie,tempHang,tempLie;
    [HideInInspector]
    public float movementCooldown, moveTime, nextTileMovecost;
    [HideInInspector]
    public long timeStart; // 进入cd的时刻
    [HideInInspector]
    public bool canBeSelected;
    [HideInInspector]
    public bool canMove;

    float percentageCDtime;
    GameObject doll;
    public bool unitSelection, isMoving = false;
    public Slider ActionCDBar;
    public GameObject CDBarCanvas;
    public GameObject selectionBox;
    public GameObject skillBox;
    public bool unitInMoveCooldown = false;
    public AudioSource EngineSound;
    Timer moveTimer = new Timer();
    bool firstTime;
    DollsProperty dolls;

    // Start is called before the first frame update
    void Start()
    {
        dolls = this.GetComponent<DollsProperty>();
        canMove = true;
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
            MoveToDestinationWithSpeed();
            moveTimer.TimerUpdate();
            if (ActionCDBar.value >= 1f)
                CDBarCanvas.SetActive(false);
            else
                CDBarCanvas.SetActive(true);
            if (unitSelection)
            {
                EngineSound.volume = 0.3f;
            }
            else
            {
                EngineSound.volume = 0f;
            }
        }
    }
    public void MoveToDestination()
    {
        //transform.GetComponent<DollsCombat>().DeFogOfWar();
        //transform.position = destination;
        //transform.GetComponent<DollsCombat>().FogOfWar();
    }
    public void MoveToDestinationWithSpeed()
    {
        Vector3 direction = destination - transform.position;
        Vector3 velocity = direction.normalized * 2;
        velocity = Vector3.ClampMagnitude(velocity, direction.magnitude);
        transform.Translate(velocity);
        if (firstTime && direction.magnitude < 1)
        {
            transform.GetComponent<DollsCombat>().DeFogOfWar();
            firstTime = false;
            transform.GetComponent<DollsCombat>().FogOfWar();
        }

    }
    public void Move()
    {
        canMove = false;
        firstTime = true;
        moveTimer.IsCounting = true;
        moveTimer.timeStart = System.DateTime.Now.Ticks;
        moveTimer.TimeToWait = moveTime;
        moveTimer.unit = this;
    }

}
