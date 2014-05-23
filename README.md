ServiceStack.Gap
================

#### Creating cross-platform Native Desktop Single Page apps with ServiceStack.

In the same way that [Phone Gap](http://phonegap.com/) allows you to create mobile apps for different
platforms with web technologies, you can also create Native Desktop apps running of an embedded version of ServiceStack.

ServiceStack has a number of features that's particularly suited for these kind of apps:

 - It allows your services to be self-hosted using .NET's HTTP Listener
 - It supports pre-compiled Razor Views
 - It supports Embedded resources
 - It supports an embedded database in Sqlite and OrmLite
 - It can be ILMerged into a single .exe

Combined together this allows you to encapsulate your ServiceStack application into a single cross-platform **.exe** that can run on Windows or OSX.

To illustrate this we'll encapsulate the [Http Benchmarks](https://github.com/ServiceStack/HttpBenchmarks) project into a portable embedded Native Desktop app that can be run locally.

## Download Benchmarks Analyzer

This app is available for download in a number of different flavours:

> **[BenchmarksAnalyzer.zip](https://github.com/ServiceStack/ServiceStack.Gap/raw/master/deploy/BenchmarksAnalyzer.zip)** - Single .exe that opens the BenchmarksAnalyzer app in the users browser
[![Partial Console Screenshot](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/gap/partial-exe.png)](https://github.com/ServiceStack/ServiceStack.Gap/raw/master/deploy/BenchmarksAnalyzer.zip)

> **[BenchmarksAnalyzer.Mac.zip](https://github.com/ServiceStack/ServiceStack.Gap/raw/master/deploy/BenchmarksAnalyzer.Mac.zip)** - Self-hosted app running inside a OSX Cocoa App Web Browser
[![Partial OSX Screenshot](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/gap/partial-osx.png)](https://github.com/ServiceStack/ServiceStack.Gap/raw/master/deploy/BenchmarksAnalyzer.Mac.zip)

> **[BenchmarksAnalyzer.Windows.zip](https://github.com/ServiceStack/ServiceStack.Gap/raw/master/deploy/BenchmarksAnalyzer.Windows.zip)** - Self-hosted app running inside a Native WinForms app inside [CEF](https://code.google.com/p/chromiumembedded/)
[![Partial Windows Screenshot](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/gap/partial-win.png)](https://github.com/ServiceStack/ServiceStack.Gap/raw/master/deploy/BenchmarksAnalyzer.Windows.zip)

## About Benchmarks Analyzer

To use just copy `BenchmarksAnalyzer.exe` in the same directory where your Apache Benchmark output files 
are kept and when started it will automatically import all Benchmark outputs with a `.txt` extension
(or grouped inside `.zip` batches) and open your preferred web browser to view the results.

### Viewing the bundled Example

`BenchmarksAnalyzer.zip` also comes with example data so you can run it to see an example of it in action:

	\Database Performance in ASP.NET.zip - Example dataset containing Benchmark outputs in a single .zip
	\server.labels - Holds Custom Labels to markup different servers in charts
	\test.labels - Holds Custom Labels to markup different tests in charts

To view the built-in example just extract the .zip file and double-click on `BenchmarksAnalyzer.exe` 
to run the Program to view the above example dataset with the custom configuration.

### Grouping Benchmarks

If you want to group multiple benchmarks outputs together, save them in separate .zip files so they're 
grouped into separate test runs labelled with the **name** of the .zip file, next time the Program is restarted.

### Marking Up Benchmark Charts

To make the benchmarks easier to read you can replace the Server and Test labels inferred from the target 
url with your own custom labels. The easiest way to do this is to go to the Admin UI redirected from the
home page (http://localhost:1337/) then click the **edit labels** checkbox which will allow you to edit
the labels for the server and tests used. 

We can markup the labels for the example urls tested above, i.e:

- http://localhost:1337/testplans.json
- http://localhost:1337/testplans.xml

#### Server Labels

	localhost:1337 ServiceStack SelfHost

Using the Format: {hostname}:{port} {Label}

#### Test Labels

	/testplans.json JSON Response
	/testplans.xml XML Response

Using the Format: {/pathinfo} {Label}

Once saved you can view the graphs again to see charts now display the custom labels above.

Saving also writes out the above info in `server.labels` and `test.labels` text files which can be 
hand-edited outside the Admin UI and will be available next time the Program is restarted.

## Creating Benchmarks Analyzer

To create the portable version of Benchmarks Analyzer we simply 
[copied the resources from HttpBenchmarks](https://github.com/ServiceStack/ServiceStack.Gap/blob/master/src/BenchmarksAnalyzer/build/init-copy-resources.done) project 
into a new [BenchmarksAnalyzer.Resources](https://github.com/ServiceStack/ServiceStack.Gap/tree/master/src/BenchmarksAnalyzer/BenchmarksAnalyzer.Resources) 
project that contains all the websites html/cshtml/js/css/img/etc assets.

We can then compile all the Razor Views and embed all the resouces inside the single `BenchmarksAnalyzer.Resources.dll` 
by adding the ServiceStack.Gap NuGet package:

	> Install-Package ServiceStack.Gap

All this package does is change all the `*.cshtml` pages **BuildAction** to `Content` and change the web assets to 
`EmbeddedResource` so they all get compiled into the `BenchmarksAnalyzer.Resources.dll`. It also adds a MS Build task
to `BenchmarksAnalyzer.Resources.csproj` to get VS.NET to pre-compile the Razor views.

### Registering Embedded Resources and Compiled Razor Views to your AppHost

To get ServiceStack to make use of your compiled Razor Views you need to specify the Assembly ServiceStack 
should scan when you're registering the `RazorFormat`, i.e:

```csharp
Plugins.Add(new RazorFormat {
    LoadFromAssemblies = { typeof(Resources.BaseTypeMarker).Assembly },
});
```

To tell ServiceStack that it should serve resources embedded inside dlls we just need to add it the HostConfig:

```csharp
SetConfig(new HostConfig {
    EmbeddedResourceBaseTypes = { GetType(), typeof(Resources.BaseTypeMarker) },
});
```

> We need to specify base types instead of assemblies so their namespaces are preserved once they're ILMerged into a single .exe

## Optimizing Benchmarks Analyzer as a portable app

There are a few things in [httpbenchmarks.servicestack.net](https://httpbenchmarks.servicestack.net/) that doesn't 
make sense as a portable app, primarily it requires authentication and supports multiple OAuth providers. It also 
supports maintaining a history of test runs and a more complicated Admin UI to manage it.

As we want to make this app as easy as possible to use, we've removed the required authentication and have changed it to 
suit a "one-shot stateless execution" where it doesn't save any state in the Sqlite database between runs, instead
it just automatically imports all the Apache Benchmark Outputs in `.txt` or `.zip` files in the directory where it's run from.

Luckily this is simple to do where we're able to re-use the existing HTTP Upload service that processses HTTP File uploads
of multiple `.txt` and `.zip` files, where instead of passing a HTTP Request with the uploads we inject it with a 
`BasicRequest` context that contains File metadata sourced from the `FileSystemVirtualPathProvider`, e.g:

```csharp
using (var admin = Resolve<AdminServices>())
{
	//...
	var dir = new FileSystemVirtualPathProvider(this, Config.WebHostPhysicalPath);
	var files = dir.GetAllMatchingFiles("*.txt")
		.Concat(dir.GetAllMatchingFiles("*.zip"));

	admin.Request = new BasicRequest
	{
		Files = files.Map(x => new HttpFile {
			ContentLength = x.Length,
			ContentType = MimeTypes.GetMimeType(x.Name),
			FileName = x.Name,
			InputStream = x.OpenRead(),
		} as IHttpFile).ToArray()
	};

	if (admin.Request.Files.Length > 0)
	{
		admin.Post(new UploadTestResults
		{
			TestPlanId = 1,
			TestRunId = testRun.Id,
			CreateNewTestRuns = true,
		});
	}
}
```

After running the import we can determine which page to open based on whether any files were imported or not.

If some results were imported then we open up immediately to view the results, otherwise we open the Admin UI
so users can manually upload the Apache Benchmark outputs themselves, e.g:

```csharp
public string GetStartUrl()
{
    using (var db = Resolve<IDbConnectionFactory>().Open())
    {
        var testResult = db.Single<TestResult>(q => q.OrderBy(x => x.Id));
        var testPlan = testResult != null ? db.SingleById<TestPlan>(testResult.TestPlanId) : null;

        return testPlan != null
            ? BaseUrl.CombineWith("{0}?id={1}".Fmt(testPlan.Slug, testResult.TestRunId)) 
            : BaseUrl;
    }
}
```

### Console Application

The Console Application works like any other [Self Host](https://github.com/ServiceStack/ServiceStack/wiki/Self-hosting) app,
the only difference is because it needs to also run as a single ILMerged .exe we need to call 
[ExportMonoSqliteDll()](https://github.com/ServiceStack/ServiceStack.Gap/blob/master/src/BenchmarksAnalyzer/BenchmarksAnalyzer/AppHost.cs#L135
) to write the unmanaged `sqlite3.dll` out to a file so it can be found by Sqlite's ADO.NET provider.

Other than that we just show a friendly reminder when it can't find any files to import, e.g:

```csharp
static void Main(string[] args)
{
    AppUtils.ExportMonoSqliteDll();

    var appHost = new AppHost("http://localhost:1337/")
        .Start();

    "Listening on {0}".Print(appHost.BaseUrl);
    "Type Ctrl+C to quit..\n".Print();

    var startUrl = appHost.GetStartUrl();
    if (startUrl == appHost.BaseUrl)
    {
        "No .txt or .zip Apache Benchmark Results found, skipping initial import...".Print();
        "Use ab utility to generate Apache Benchmark .txt files to analyze.".Print();
    }

    Process.Start(startUrl);
    Thread.Sleep(Timeout.Infinite);
}
```
