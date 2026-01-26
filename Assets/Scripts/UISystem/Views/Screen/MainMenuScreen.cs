using Cysharp.Threading.Tasks;
using Isekai.Managers;
using Isekai.UI.ViewModels.Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Isekai.UI.Views.Screens
{
    public class MainMenuScreen : Screen<MainMenuViewModel>
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Version;
        
        public override void OnEnterScreen()
        {
            BindingDescription bindingDescription = new BindingDescription("text", "ViewModel.Title", BindingMode.OneWay);
            bindingDescription.TargetType = typeof(TextMeshProUGUI);

            Binding binding = new Binding(ViewModel, Title, bindingDescription);
            ViewModel.Title = "Backpack Battle";
        }

        public override void OnExitScreen()
        {
            
        }
        public void OnNewGameClicked()
        {
            LevelManager.Instance.TransitionToScene("GameScene").Forget();
        }
        public void OnSettingsClicked()
        {
            SettingsViewModel viewmodel = new SettingsViewModel();
            ScreenManager.Instance.TransitionTo(EScreenType.SettingsScreen, ELayerType.DefaultLayer, viewmodel).Forget();
        }

    }

}
