# Caliburn.Micro.Extras

[Caliburn.Micro](http://caliburnmicro.codeplex.com/) is a powerful framework for building WPF, Silverlight, Windows Phone and Windows 8 Store apps.
These additions make it easier to to get the most out of the framework.

## Install 
The extras are available through NuGet:

**Install-Package** [Caliburn.Micro.Extras](https://www.nuget.org/packages/Caliburn.Micro.Extras/)

## Content
* `DebugLogger` to see Caliburn.Micro logging output in Visual Studio
* `ActionCommand` to use Caliburn.Micro _Actions_ with `ICommand`
* [Win8] `IWindowManager` for displaying normal dialogs all around the screen
* `EventAggregatorExtensions` to publish messages on different Threads

### Services
* `IMessageService` to show a message box from the ViewModel
* `IOpenFileService` wraps an OpenFileDialog 
* `ISaveFileService` wraps a SaveFileDialog

### IResult Implementations
* `CancelResult` always returns WasCancelled=true
* `DelegateResult` wraps an arbitrary Action or Func<TResult>
* `MessengerResult` wraps a MessageBox
* `OpenFileResult` wraps an OpenFileDialog with fluent configuration
* `SaveFileResult` wraps a SaveFileDialog with fluent configuration

### IResult Extensions
* `Rescue<TException>()` decorates the result with an error handler which is executed when an error occurs
* `WhenChancelled()` decorates the result with an handler which is executed when the result was cancelled
* `OverrideCancel()` decorates the result and overrides WasCancelled=false

### Weak Events
powered by [Weakly](https://github.com/tibel/Weakly)

### Conventions
* Module level bootstrappers (inspired by [Splitting Application to Multiple Assemblies when using Caliburn.Micro](http://mikaelkoskinen.net/post/windows-phone-caliburn-micro-split-app-multiple-assemblies.aspx))
* `ContentHost` control (inspired by [Fast switching between ViewModels in Caliburn.Micro](http://www.baud.cz/blog/fast-switching-between-viewmodels-in-caliburn.micro)
