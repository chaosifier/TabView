using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Xam.Plugin.TabView
{
    public interface ITabViewControlTabItem
    {
        string TabViewControlTabItemTitle { get; set; }
        ImageSource TabViewControlTabItemIconSource { get; set; }

        /// <summary>
        /// called when the view receives focus
        /// </summary>
        void TabViewControlTabItemFocus();
    }
}
