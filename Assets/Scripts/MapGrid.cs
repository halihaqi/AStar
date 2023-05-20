using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public enum E_GridType
    {
        Null,Open,Close,Obs,Path
    }
    public class MapGrid : MonoBehaviour
    {
        public Image img;
        public Image imgPath;
        public Text txt;
        public E_GridType myType;

        public static bool canClick = false;
        public static bool canDrag = false;

        private void Start()
        {
            img.color = Color.white;
            imgPath.gameObject.SetActive(false);
            txt.text = "";
            var trigger = gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(data =>
            {
                if(!canClick) return;
                switch (NavMeshPanel.clickType)
                {
                    case E_ClickType.Start:
                        SetOriFin(true);
                        break;
                    case E_ClickType.Fin:
                        SetOriFin(false);
                        break;
                    case E_ClickType.Null:
                        ChangeState(E_GridType.Null);
                        break;
                    case E_ClickType.Obs:
                        ChangeState(E_GridType.Obs);
                        break;
                }
                canClick = false;
                NavMeshPanel.clickType = E_ClickType.Null;
            });
            trigger.triggers.Add(entry);
            var entry2 = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry2.callback.AddListener(data =>
            {
                if(!canDrag) return;
                ChangeState(E_GridType.Obs);
            });
            trigger.triggers.Add(entry2);
        }

        public void SetOriFin(bool isOri)
        {
            if (isOri)
            {
                txt.text = "起点";
                NavMeshPanel.start = this;
            }
            else
            {
                txt.text = "终点";
                NavMeshPanel.fin = this;
            }
            img.color = Color.white;
        }

        public void ChangeState(E_GridType type)
        {
            imgPath.gameObject.SetActive(false);
            myType = type;
            switch (type)
            {
                case E_GridType.Null:
                    img.color = Color.white;
                    txt.text = "";
                    break;
                case E_GridType.Obs:
                    img.color = Color.red;
                    txt.text = "";
                    break;
                case E_GridType.Open:
                    img.color = Color.yellow;
                    break;
                case E_GridType.Close:
                    img.color = Color.green;
                    break;
                case E_GridType.Path:
                    imgPath.gameObject.SetActive(true);
                    break;
            }
        }
    }
}