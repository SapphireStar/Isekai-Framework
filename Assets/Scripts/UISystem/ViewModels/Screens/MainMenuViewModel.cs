using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Isekai.UI.ViewModels.Screens
{
    public class MainMenuViewModel:ViewModel
    {
        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                ChangePropertyAndNotify(ref title, value);
            }
        }
    }
}

