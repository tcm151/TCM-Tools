using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeReference] private Image baseImage;
        [SerializeReference] private Image fillImage;
        [SerializeReference] private RectMask2D fillMask;

        private Rect rect;
        private float width, step;

        public float min, max = 100f;
        public float value {get; private set;}

        //> INITIALIZATION
        private void Awake()
        {
            rect = GetComponent<RectTransform>().rect;
            width = rect.width;
            step = width / (max - min);
        }

        //> PROCESS NEW VALUE
        public void SetValue(float newValue)
        {
            value = newValue;
            float fill = (max - value) * step;
            fillMask.padding = new Vector4(0f, 0f, fill, 0f);
        }

        //> SET BASE OR FILL COLOR
        public void SetBaseColor(Color newColor) => baseImage.color = newColor;
        public void SetFillColor(Color newColor) => fillImage.color = newColor;
    }
}
