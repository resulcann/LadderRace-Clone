namespace SRDebugger.UI.Controls.Data
{
    using System;
    using SRF;
    using UnityEngine;
    using UnityEngine.UI;

    public class ColorControl : DataBoundControl
    {
        [RequiredField] public Text Title;

        [RequiredField] public Button Button;
        [RequiredField] public Image PreviewImage;

        [RequiredField] public Toggle PickerToggle;
        [RequiredField] public ColorPicker ColorPicker;

        protected override void Start()
        {
            base.Start();

            ColorPicker.gameObject.SetActive(false);

            Button.onClick.AddListener(ButtonOnClicked);
            PickerToggle.onValueChanged.AddListener(PickerToggleOnValueChanged);
            ColorPicker.onValueChanged.AddListener(ColorPickerOnValueChanged);
        }

        private void ButtonOnClicked()
        {
            
        }

        private void PickerToggleOnValueChanged(bool value)
        {
            ColorPicker.gameObject.SetActive(value);
        }

        private void ColorPickerOnValueChanged(Color color)
        {
            PreviewImage.color = color;
            UpdateValue(color);
        }

        protected override void OnBind(string propertyName, Type t)
        {
            base.OnBind(propertyName, t);

            Title.text = propertyName;
        }

        protected override void OnValueUpdated(object newValue)
        {
            var value = (Color) newValue;
            PreviewImage.color = value;
            ColorPicker.CurrentColor = (Color)newValue;
        }

        public override bool CanBind(Type type, bool isReadOnly)
        {
            return type == typeof (Color);
        }
    }
}
