# TabView
TabView control for Xamarin.Forms.<br />
Requires <a href="https://github.com/alexrainman/CarouselView/">CarouselView</a> plugin.

Also available on NuGet :  https://github.com/chaosifier/TabView [![NuGet](https://img.shields.io/badge/NUGET-1.0.0-green.svg)](https://github.com/chaosifier/TabView)

Make sure to initialize the CarouselView plugin first before using TabView.
To do so, in your iOS and Android projects call:

```
Xamarin.Forms.Init();
CarouselViewRenderer.Init();
```

And in your UWP project call:

```
List<Assembly> assembliesToInclude = new List<Assembly>();
assembliesToInclude.Add(typeof(CarouselViewRenderer).GetTypeInfo().Assembly);
Xamarin.Forms.Forms.Init(e, assembliesToInclude);
```

Bindable properties:
- HeaderBackgroundColor
- HeaderTabTextColor
- ContentHeight
- HeaderSelectionUnderlineColor
- HeaderSelectionUnderlineThickness
- HeaderSelectionUnderlineWidth
- HeaderTabTextFontSize
- HeaderTabTextFontFamily
- HeaderTabTextFontAttributes


Events:
- OnPositionChanging
- OnPositionChanged


Functions: 
- SetPosition
- SelectNext
- SelectPrevious
- SelectLast
- AddTab
- RemoveTab


Screenshots:

<img src="https://media.giphy.com/media/l4pSYIQqxenNghNcY/giphy.gif" align="middle" width="300" alt="TabView preview on UWP"/>
<img src="https://media.giphy.com/media/3ohs4wHWKIzKlX9R9S/giphy.gif" align="middle" width="300" alt="TabView preview on Android"/>
