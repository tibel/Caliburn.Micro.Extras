# Caliburn.Micro.Extras

[Caliburn.Micro](http://caliburnmicro.codeplex.com/) is a powerful framework for building WPF, Silverlight, Windows Phone and Windows 8 Store apps.
These additions make it easier to to get the most out of the framework.

## Install 
The extras are available through NuGet:

**Install-Package** [Caliburn.Micro.Extras](https://www.nuget.org/packages/Caliburn.Micro.Extras/)

## Content
* [Win8] `IWindowManager` for displaying normal dialogs all around the screen
* `DebugLogger` to see Caliburn.Micro logging output in Visual Studio
* `EventAggregatorExtensions` to publish messages on different Threads
* `IMessageService` to show a message box from the ViewModel
* `IOpenFileService` and `ISaveFileSerivce` to show select a file from the ViewModel
* `ResultExtensions` to get more out of Coroutines
* [WP71] and [WP8] Module level bootstrappers (inspired by [Splitting Application to Multiple Assemblies when using Caliburn.Micro](http://mikaelkoskinen.net/post/windows-phone-caliburn-micro-split-app-multiple-assemblies.aspx))
