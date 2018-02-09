using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xam.Plugin.TabView.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var tabView = new TabViewControl(new List<TabViewControl.TabItem>()
            {
                new TabViewControl.TabItem
                {
                    Content = new Label{Text = "Tab 1", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Red },
                    HeaderText = "Tab 1"
                },
                new TabViewControl.TabItem
                {
                    Content = new Label{Text = "Tab 2", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Green },
                    HeaderText = "Tab 2"
                },
                new TabViewControl.TabItem
                {
                    Content = new Label{Text = "Tab 3", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Blue },
                    HeaderText = "Tab 3"
                }
            });

            theSl.Children.Add(tabView);
        }
    }
}
