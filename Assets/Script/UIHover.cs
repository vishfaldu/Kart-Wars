using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        Vector3 cachedScale;

        void Start()
        {

            cachedScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            transform.localScale = cachedScale;
        }
    }

