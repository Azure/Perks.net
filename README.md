# Azure/Perks

This repository contains common reusable libraries and tools that has been accumulating in some open source projects.  

Instead of having one project publish things tangentially to it's core mission, we're migrating that code here.

### Libraries 

#### Microsoft.Perks
Simple dotnet classes:
    - `Microsoft.Perks.CompareExtensions`
    - `Microsoft.Perks.Comparer`
    - `Microsoft.Perks.EqualityComparer`
    - `Microsoft.Perks.DisposableExtensions`
    - `Microsoft.Perks.OnDispose`
    - `Microsoft.Perks.StringExtensions`

#### Microsoft.Perks.Async
Async helper classes:
    - `Microsoft.Perks.Async.Collections.BlockingCollection`
    - `Microsoft.Perks.Async.Linq.LinqExtensions`
    - `Microsoft.Perks.Async.Observable.BlockingCollectionObserver`
    - `Microsoft.Perks.Async.Observable.EnumerableObserver`
    - `Microsoft.Perks.Async.Observable.ObservableAwaiter`
    - `Microsoft.Perks.Async.Observable.ObservableExtensions`
    - `Microsoft.Perks.Async.Observable.WaitableBoolean`
    - `Microsoft.Perks.Async.Task.TaskExtensions`
    - `Microsoft.Perks.Async.Task.TasksCollection`

#### Microsoft.Perks.Collections
Extensions to Collection Classes:
    - `Microsoft.Perks.Collections.List`

#### Microsoft.Perks.Console
Classes to make EXEs based on the same attributes as PowerShell cmdlets.
    
#### Microsoft.Perks.DependencyInjection
The Least Offensive Dependency Injection System 
    - see (documentation)[./docs/Dependency-Injection/Readme.md]
    
#### Microsoft.Perks.Linq

Classes for a better LINQ experience in .NET 

    - `Microsoft.Perks.Linq.DisposeAsYouGoEnumerable`
    - `Microsoft.Perks.Linq.LinqExtensions`
    - `Microsoft.Perks.Linq.MutableEnumerable`
    - `Microsoft.Perks.Linq.ReEnumerable`

### Tools 

#### Microsoft.Perks.CodeGen

A simple razor-based code generation tool code that transforms `.cshtml` files into `.cs` files. 
Shipped as a nuget package that can be installed in a dotnet-cli project:


``` xml
  <ItemGroup>
    <DotnetCliToolReference Include="Microsoft.Perks.CodeGen" Version="1.0.0" />
  </ItemGroup>
```



#### 
# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
