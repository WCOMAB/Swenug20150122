var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var isPullRequest       = AppVeyor.Environment.PullRequest.IsPullRequest;
var version             = "0.0.0.1";
var semVersion          = isLocalBuild ? version : (version + string.Concat("-build-", AppVeyor.Environment.Build.Number));
var assemblyId          = "SweNug20150122";
var binDir              = string.Format("./src/{0}/bin/{1}", assemblyId, configuration);
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

Task("Publish-MyGet")
    .IsDependentOn("Create-NuGet-Package")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest) 
    .Does(() =>
{
    // Resolve the API key.
    var apiKey = EnvironmentVariable("MYGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    var source = EnvironmentVariable("MYGET_SOURCE");
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve MyGet source.");
    }

    // Get the path to the package.
    var package = nugetRoot + assemblyId + semVersion + ".nupkg";

    // Push the package.
    NuGetPush(package, new NuGetPushSettings {
        Source = source,
        ApiKey = apiKey
    }); 
});
