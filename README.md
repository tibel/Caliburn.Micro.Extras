# Caliburn.Micro.Extras [Discontinued]

[Caliburn.Micro](http://caliburnmicro.com) is a powerful framework for building WPF, Silverlight, Windows Phone and Windows 8 Store apps.
These additions make it easier to to get the most out of the framework.

## Install 
The extras are available through NuGet:

**Install-Package** [Caliburn.Micro.Extras](https://www.nuget.org/packages/Caliburn.Micro.Extras/)

## Content
* `ActionCommand` to use Caliburn.Micro _Actions_ with `ICommand`
* [Win8] `IWindowManager` for displaying normal dialogs all around the screen

### Dialogs
* `IMessageService` to show a MessageBox
* `IOpenFileService` to show an OpenFileDialog 
* `ISaveFileService` to show an SaveFileDialog
* `MessengerResult` wraps IMessageService with fluent configuration
* `OpenFileResult` wraps IOpenFileService with fluent configuration
* `SaveFileResult` wraps ISaveFileService with fluent configuration

### Weak Events
powered by [Weakly](https://github.com/tibel/Weakly)

### Conventions
* Module level bootstrappers (inspired by [Splitting Application to Multiple Assemblies when using Caliburn.Micro](http://mikaelkoskinen.net/post/windows-phone-caliburn-micro-split-app-multiple-assemblies.aspx))
* `ContentHost` control (inspired by [Fast switching between ViewModels in Caliburn.Micro](http://www.baud.cz/blog/fast-switching-between-viewmodels-in-caliburn.micro)



## Integrated in CM 2.0
* `DebugLogger` to see Caliburn.Micro logging output in Visual Studio
* `EventAggregatorExtensions` to publish messages on different Threads

### IResult Implementations
* `CancelResult` always returns WasCancelled=true
* `DelegateResult` wraps an arbitrary Action or Func<TResult>

### IResult Extensions
* `Rescue<TException>()` decorates the result with an error handler which is executed when an error occurs
* `WhenChancelled()` decorates the result with an handler which is executed when the result was cancelled
* `OverrideCancel()` decorates the result and overrides WasCancelled=false
