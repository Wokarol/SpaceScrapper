using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wokarol.SpaceScrapper.UI
{
    public interface IHasName
    {
        string Name { get; }
    }

    public class ButtonList<T> where T : IHasName
    {
        private Settings settings = null;
        private readonly Action<T> callbackOnButtonClick;
        private readonly bool keepButtonPressed;
        private List<SpawnedButton> spawnedButtons = new();
        private Queue<SpawnedButton> cachedButtonQueue = new();


        public ButtonList(Settings settings, Action<T> callbackOnButtonClick, bool keepButtonPressed = false)
        {
            this.settings = settings;
            this.callbackOnButtonClick = callbackOnButtonClick;
            this.keepButtonPressed = keepButtonPressed;
            settings.ButtonTemplate.gameObject.SetActive(false);
        }

        public void InitList(List<T> options)
        {

            var buttonParent = settings.ButtonTemplate.transform.parent;
            var buttonQueue = GetPreviousSpawnedButtons();

            spawnedButtons.Clear();

            SpawnButtons(options, buttonParent, buttonQueue);

            while (buttonQueue.TryDequeue(out var slot))
                UnityEngine.Object.Destroy(slot.Button.gameObject);
        }

        private void SpawnButtons(List<T> options, Transform buttonParent, Queue<SpawnedButton> buttonQueue)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                Button button;

                if (buttonQueue.TryDequeue(out var slot))
                {
                    button = slot.Button;
                }
                else
                {
                    button = UnityEngine.Object.Instantiate(settings.ButtonTemplate, buttonParent);
                    button.gameObject.SetActive(true);

                    int index = i;
                    button.onClick.AddListener(() =>
                    {
                        callbackOnButtonClick(spawnedButtons[index].Option);

                        if (keepButtonPressed)
                        {
                            DeselectAll();

                            var colors = button.colors;
                            colors.normalColor = colors.pressedColor;
                            colors.selectedColor = colors.pressedColor;
                            button.colors = colors;
                        }
                    });

                    if (settings.KeepLastElementInButtonsList)
                        button.transform.SetSiblingIndex(buttonParent.childCount - 2);

                }

                button.GetComponentInChildren<TMPro.TMP_Text>().text = option.Name;

                spawnedButtons.Add(new SpawnedButton(button, option));
            }
        }

        private Queue<SpawnedButton> GetPreviousSpawnedButtons()
        {
            var buttontQueue = cachedButtonQueue;
            cachedButtonQueue.Clear();
            foreach (var slot in spawnedButtons) cachedButtonQueue.Enqueue(slot);
            return buttontQueue;
        }

        public void DeselectAll()
        {
            foreach (var b in spawnedButtons)
            {
                b.Button.colors = settings.ButtonTemplate.colors;
            }
        }

        public readonly struct SpawnedButton
        {
            public readonly Button Button;
            public readonly T Option;

            public SpawnedButton(Button button, T option)
            {
                Button = button;
                Option = option;
            }
        }


        [System.Serializable]
        public class Settings
        {
            [SerializeField] private Button buttonTemplate = null;
            [Tooltip("Used for spacers in scrolled lists")]
            [SerializeField] private bool keepLastElementInButtonsLists = true;

            public Button ButtonTemplate => buttonTemplate;
            public bool KeepLastElementInButtonsList => keepLastElementInButtonsLists;
        }

    }
}
