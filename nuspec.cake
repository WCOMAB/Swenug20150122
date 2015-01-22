var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var isPullRequest       = AppVeyor.Environment.PullRequest.IsPullRequest;
var version             = "0.0.0.1";
var semVersion          = isLocalBuild ? version : (version + string.Concat("-build-", AppVeyor.Environment.Build.Number));
var assemblyId          = "SweNug20150222";
var binDir              = "C:/temp/Swenug20150122/src/Swenug20150222/bin";
var nugetRoot           = "./nuget/";

var nuGetPackSettings   = new NuGetPackSettings { 
    Id                  = assemblyId,
    Version             = semVersion,
    BasePath 		= binDir, 
    OutputDirectory     = nugetRoot
};

Task("Create-NuGet-Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (!Directory.Exists(nugetRoot))
    {
	CreateDirectory(nugetRoot);
    }
    NuGetPack(
        string.Format("./nuspec/{0}.nuspec", assemblyId),
        nuGetPackSettings
    );
});