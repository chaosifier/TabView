using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xam.Plugin.TabView;
using Xamarin.Forms;

namespace TabViewSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Next(object sender, EventArgs e)
        {
            theTabView.SelectNext();
        }

        private void Button_First(object sender, EventArgs e)
        {
            theTabView.SelectFirst();
        }

        private void Button_Previous(object sender, EventArgs e)
        {
            theTabView.SelectPrevious();
        }

        private void Button_Last(object sender, EventArgs e)
        {
            theTabView.SelectLast();
        }

        private void Button_AddTab(object sender, EventArgs e)
        {
            theTabView.AddTab(new TabItem("New", new Label { Text = "New", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Red }));
        }

        private void Button_RemoveLastTab(object sender, EventArgs e)
        {
            theTabView.RemoveTab(theTabView.ItemSource.Count - 1);
        }

        private void Button_ChangeTextColor(object sender, EventArgs e)
        {
            theTabView.HeaderTabTextColor = theTabView.HeaderTabTextColor == Color.LightGreen ? Color.White : Color.LightGreen;
        }

        private void Button_ChangeSelectionUnderlineColor(object sender, EventArgs e)
        {
            theTabView.HeaderSelectionUnderlineColor = theTabView.HeaderSelectionUnderlineColor == Color.Yellow ? Color.White : Color.Yellow;
        }

        private void Button_ChangeSelectionUnderlineThickness(object sender, EventArgs e)
        {
            theTabView.HeaderSelectionUnderlineThickness = theTabView.HeaderSelectionUnderlineThickness + 5;
        }

        private void Button_ChangeSelectionUnderlineWidth(object sender, EventArgs e)
        {
            theTabView.HeaderSelectionUnderlineWidth = theTabView.HeaderSelectionUnderlineWidth + 10;
        }

        private void Button_ChangeTabTextFontSize(object sender, EventArgs e)
        {
            theTabView.HeaderTabTextFontSize = theTabView.HeaderTabTextFontSize + 1;
        }

        private void Button_ChangeTabTextFontAttributes(object sender, EventArgs e)
        {
            theTabView.HeaderTabTextFontAttributes = theTabView.HeaderTabTextFontAttributes == FontAttributes.Bold ? FontAttributes.Italic : FontAttributes.Bold;
        }

        private void Button_ChangeTabTextFontFamily(object sender, EventArgs e)
        {
            theTabView.HeaderTabTextFontFamily = theTabView.HeaderTabTextFontFamily == "Droid Sans Mono" ? "Comic Sans MS" : "Droid Sans Mono";
        }

        private async void GoToXamlSample(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new XamlSamplePage());
        }
    }
}