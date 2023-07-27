using System;
using UnityEngine;
using UnityEngine.UI;

namespace poetools.Console
{
    public class RuntimeConsoleView : MonoBehaviour
    {
        public Text textDisplay;
        public Text autoCompleteDisplay;
        public InputField inputFieldDisplay;

        private string _text;
        private bool _textChanged;

        public event Action<bool, bool> OnVisibilityChanged;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                    _textChanged = true;

                _text = value;
            }
        }

        private void Update()
        {
            if (_textChanged)
            {
                textDisplay.text = Text;
                _textChanged = false;
            }
        }

        public void SetVisible(bool isVisible)
        {
            bool wasVisible = gameObject.activeSelf;
            gameObject.SetActive(isVisible);
            OnVisibilityChanged?.Invoke(wasVisible, isVisible);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

    }
}
