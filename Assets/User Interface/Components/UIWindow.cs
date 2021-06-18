
using System;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(GraphicRaycaster))]
    abstract public class UIWindow : MonoBehaviour
    {
        virtual public void Show()
        {
            //@ add tweening animation
            group.alpha = 1;
        }

        virtual public void Hide()
        {
            //@ add tweening animation
            group.alpha = 0;
        }

        abstract public void GoBack();

        public float Height => transform.rect.height;
        public float Width => transform.rect.width;
        
        protected Canvas canvas;
        protected CanvasGroup group;
        protected GraphicRaycaster raycaster;
        new protected CanvasRenderer renderer;
        new protected RectTransform transform;
        
        virtual protected void Awake()
        {
            canvas = GetComponent<Canvas>();
            group  = GetComponent<CanvasGroup>();
            renderer = GetComponent<CanvasRenderer>();
            raycaster = GetComponent<GraphicRaycaster>();
            transform = GetComponent<RectTransform>();
        }
    }
}

