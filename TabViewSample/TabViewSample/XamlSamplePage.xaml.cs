using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TabViewSample
{
    public partial class XamlSamplePage : ContentPage
    {
        public XamlSamplePage()
        {
            InitializeComponent();
            //tab.HeaderSelectionTabTextColor = Color.Blue;
        }

        async void GoBackClicked(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
