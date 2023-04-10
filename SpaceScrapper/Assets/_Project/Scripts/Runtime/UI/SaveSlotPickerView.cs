using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.SpaceScrapper.Saving;
using static Wokarol.SpaceScrapper.Saving.SaveSystem;

namespace Wokarol.SpaceScrapper.UI
{
    public class SaveSlotPickerView : MonoBehaviour
    {
        [Header("Saves")]
        [SerializeField] private ButtonList<SaveSlotOption>.Settings saveButtonList = null;
        [Header("Subsaves")]
        [SerializeField] private ButtonList<SubsaveOption>.Settings subsaveButtonList = null;
        [SerializeField] private CanvasGroup noSaveCoverImage = null;

        private ButtonList<SaveSlotOption> saveButtons = null;
        private ButtonList<SubsaveOption> subsaveButtons = null;

        public event Action<FileNameAndMetadata> SelectedSavePath;


        private void Awake()
        {
            saveButtons = new(saveButtonList, PressedSaveSlotButton, keepButtonPressed: true);
            subsaveButtons = new(subsaveButtonList, PressedSubsaveButton);
        }

        public void Initialize(List<SaveSlotOption> options)
        {
            noSaveCoverImage.DOKill();
            noSaveCoverImage.alpha = 1;
            noSaveCoverImage.gameObject.SetActive(true);

            saveButtons.DeselectAll();

            saveButtons.InitList(options);
        }


        private void PressedSaveSlotButton(SaveSlotOption pickedSave)
        {
            noSaveCoverImage
                .DOFade(0f, 0.2f)
                .OnComplete(() => noSaveCoverImage.gameObject.SetActive(false));

            var subsaves = pickedSave.Files
                .Select(p => new SubsaveOption(GetPrettyFileName(p.FileName), p))
                .ToList();

            subsaveButtons.InitList(subsaves);
        }

        private string GetPrettyFileName(string fileName)
        {
            return Regex.Match(fileName, @"(?:.+?\.)?(\w+)").Groups[1].Value;
        }

        private void PressedSubsaveButton(SubsaveOption slot)
        {
            SelectedSavePath?.Invoke(slot.Meta);
        }

        public readonly struct SaveSlotOption : IHasName
        {
            public readonly string Name;
            public readonly List<FileNameAndMetadata> Files;

            public SaveSlotOption(string name, List<FileNameAndMetadata> files)
            {
                Name = name;
                Files = files;
            }

            string IHasName.Name => Name;
        }

        public readonly struct SubsaveOption : IHasName
        {
            public readonly string Name;
            public readonly FileNameAndMetadata Meta;

            public SubsaveOption(string name, FileNameAndMetadata meta)
            {
                Name = name;
                Meta = meta;
            }

            string IHasName.Name => Name;
        }
    }
}
