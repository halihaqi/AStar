using AStar;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum E_ClickType
{
    Start,Fin,Obs,Null
}
public class NavMeshPanel : MonoBehaviour
{
    public Button btnStart;
    public Button btnFin;
    public Button btnObs;
    public Button btnAStar;
    public Button btnClear;
    public RectTransform gridMap;
    
    public Text txtMove;
    public Image imgObs;
    
    private MapGrid[,] _map = new MapGrid[10,10];
    private Texture2D _cursor;
    
    public static E_ClickType clickType = E_ClickType.Null;
    public static MapGrid start;
    public static MapGrid fin;

    private void Start()
    {
        _cursor = Resources.Load<Texture2D>("Cursor");
        var prefab = Resources.Load<GameObject>("grid");
        btnStart.onClick.AddListener(OnStartClick);
        btnFin.onClick.AddListener(OnFinClick);
        btnObs.onClick.AddListener(OnObsClick);
        btnAStar.onClick.AddListener(OnAStarClick);
        btnClear.onClick.AddListener(OnClearClick);
        var trigger = imgObs.gameObject.AddComponent<EventTrigger>();
        var entry1 = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entry1.callback.AddListener(data =>
        {
            MapGrid.canDrag = true;
            imgObs.raycastTarget = false;
            imgObs.rectTransform.sizeDelta *= 0.5f;
            imgObs.color = Color.cyan;
        });
        var entry2 = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entry2.callback.AddListener(data =>
        {
            MapGrid.canDrag = false;
            imgObs.raycastTarget = true;
            SetCursor(false);
            imgObs.gameObject.SetActive(false);
            clickType = E_ClickType.Null;
            imgObs.rectTransform.sizeDelta *= 2;
            imgObs.color = Color.red;
        });
        trigger.triggers.Add(entry1);
        trigger.triggers.Add(entry2);

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j ++)
            {
                var obj = Instantiate(prefab);
                var trans = obj.transform as RectTransform;
                trans.SetParent(gridMap, false);
                trans.anchoredPosition = new Vector2(30 * i, -30 * j);
                _map[i, j] = obj.GetComponent<MapGrid>();
            }
        }
    }

    private void Update()
    {
        btnAStar.interactable = start && fin;
        if(clickType == E_ClickType.Null) return;
        if (clickType == E_ClickType.Start || clickType == E_ClickType.Fin)
        {
            txtMove.rectTransform.position = Input.mousePosition;
        }
        else if (clickType == E_ClickType.Obs)
        {
            imgObs.rectTransform.position = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0) && clickType != E_ClickType.Obs)
        {
            SetCursor(false);
            txtMove.gameObject.SetActive(false);
            imgObs.gameObject.SetActive(false);
        }
    }

    private void SetCursor(bool isChoose)
    {
        Cursor.SetCursor(isChoose ? _cursor : null, Vector2.zero, CursorMode.Auto);
    }

    private void OnStartClick()
    {
        if (start != null)
        {
            start.ChangeState(E_GridType.Null);
            start = null;
        }
        SetCursor(true);
        MapGrid.canClick = true;
        clickType = E_ClickType.Start;
        
        //文字跟随鼠标
        txtMove.text = "起点";
        txtMove.gameObject.SetActive(true);
        txtMove.rectTransform.anchoredPosition = Input.mousePosition;
    }
    
    private void OnFinClick()
    {
        if (fin != null)
        {
            fin.ChangeState(E_GridType.Null);
            fin = null;
        }
        SetCursor(true);
        MapGrid.canClick = true;
        clickType = E_ClickType.Fin;
        
        //文字跟随鼠标
        txtMove.text = "终点";
        txtMove.gameObject.SetActive(true);
        txtMove.rectTransform.position = Input.mousePosition;
    }
    
    private void OnObsClick()
    {
        SetCursor(true);
        MapGrid.canClick = true;
        clickType = E_ClickType.Obs;
        
        //图片跟随鼠标
        imgObs.gameObject.SetActive(true);
        imgObs.rectTransform.position = Input.mousePosition;
    }

    private void OnClearClick()
    {
        foreach (var grid in _map)
        {
            grid.ChangeState(E_GridType.Null);
        }
        
        btnStart.interactable = true;
        btnFin.interactable = true;
        btnObs.interactable = true;
        start = null;
        fin = null;
    }
    
    private void OnAStarClick()
    {
        var path = AStarMgr.Instance.GetPath(_map, out var openList, out var closeList);
        foreach (var pos in openList)
        {
            _map[pos.x, pos.y].ChangeState(E_GridType.Open);
        }
        foreach (var pos in closeList)
        {
            _map[pos.x, pos.y].ChangeState(E_GridType.Close);
        }
        foreach (var pos in path)
        {
            _map[pos.x, pos.y].ChangeState(E_GridType.Path);
        }

        MapGrid.canClick = false;
        btnStart.interactable = false;
        btnFin.interactable = false;
        btnObs.interactable = false;
    }
}
