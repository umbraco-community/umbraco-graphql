// include Fake lib
#r "paket:
source nuget/dotnetcore
source https://api.nuget.org/v3/index.json
nuget FSharp.Core ~> 4.1
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem //"
#load "./.fake/build.fsx/intellisense.fsx"

open System
open System.IO
open Fake.Core
open Fake.DotNet
open Fake.IO

// Properties
let currentDirectory = Directory.GetCurrentDirectory()
let solutionFile = Directory.findFirstMatchingFile "*.sln" currentDirectory
let artifactsDir = Path.getFullName "./artifacts/"

let buildVersion = if not BuildServer.isLocalBuild then Some BuildServer.buildVersion else None

// Targets
Target.create "Clean" (fun _ ->
  Shell.cleanDirs [artifactsDir]
)

Target.create "Build" (fun _ ->
  DotNet.build (fun c ->
    { c with
        Configuration = DotNet.BuildConfiguration.Release
        OutputPath = Some artifactsDir
    }) solutionFile
)

Target.create "Package" (fun _ ->
  DotNet.pack (fun c ->
    { c with
        Configuration = DotNet.BuildConfiguration.Release
        OutputPath = Some artifactsDir
        VersionSuffix = buildVersion
    }) solutionFile
)

Target.create "Default" ignore

open Fake.Core.TargetOperators

"Clean"
  ==> "Build"
  ==> "Default"

"Clean"
  ==> "Package"

// start build
Target.runOrDefault "Default"
