using UnityEngine;
using UnityEngine.EventSystems;

namespace CityLifeIndia.UI
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Joystick Visual Components")]
        [SerializeField] private RectTransform backgroundRect;
        [SerializeField] private RectTransform handleRect;

        [Header("Settings")]
        [SerializeField] private float handleRange = 100f;
        [SerializeField] private float deadZone = 0.05f;

        private Vector2 inputVector = Vector2.zero;

        public float Horizontal => inputVector.x;
        public float Vertical => inputVector.y;
        public Vector2 Direction => inputVector;

        private void Start()
        {
            if (backgroundRect == null) backgroundRect = GetComponent<RectTransform>();
            if (handleRect == null && transform.childCount > 0)
            {
                handleRect = transform.GetChild(0).GetComponent<RectTransform>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, eventData.position, eventData.pressEventCamera, out position))
            {
                position.x = (position.x / backgroundRect.sizeDelta.x);
                position.y = (position.y / backgroundRect.sizeDelta.y);

                inputVector = new Vector2(position.x * 2 - 1, position.y * 2 - 1);
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

                if (inputVector.magnitude < deadZone)
                {
                    inputVector = Vector2.zero;
                }

                if (handleRect != null)
                {
                    handleRect.anchoredPosition = new Vector2(inputVector.x * (backgroundRect.sizeDelta.x / 2), inputVector.y * (backgroundRect.sizeDelta.y / 2));
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}
