# Benchmarks Analyzer

BenchmarksAnalyzer lets you analyze your Apache Benchmarks and export the results.

### Usage

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

### Requirements

A .NET 4.5 runtime is required to run `BenchmarksAnalyzer.exe`.
Linux/OSX platforms can install Mono: http://www.go-mono.com/mono-downloads/download.html

Running an Apache Benchmark
===========================

Here's an example of using the ab utility to run a benchmark against one of the BenchmarkAnalyzer services itself:

    ab -k -n 1000 -c 10 "http://localhost:1337/testplans.json" > json_testplans_1000_10.txt
    ab -k -n 1000 -c 10 "http://localhost:1337/testplans.xml"  > xml_testplans_1000_10.txt

This benchmark uses the HTTP keepalive feature when performing 1000 requests at 10 requests at a time 
against the above url, saving the results of the benchmark in a file called json_1000_10.txt. 
More options are available in the Apache Benchmark docs: http://httpd.apache.org/docs/2.2/programs/ab.html

The `*_testplans_1000_10.txt` files can be saved in the same directory as `BenchmarksAnalyzer.exe` 
so the results are available next time the program is restarted.

> A Windows version of the `ab.exe` utility is bundled inside `BenchmarksAnalyzer.zip` download. 
> The ab utility is available on other platforms in the `/bin/ab` where Apache is installed.

Grouping Benchmarks
-------------------

If you want to group multiple benchmarks outputs together, save them in separate .zip files so they're 
grouped into separate test runs labelled with the **name** of the .zip file, next time the Program is restarted.

Marking Up Benchmark Charts
---------------------------

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

# About

The source code for Benchmarks Analyzer is available at: https://github.com/ServiceStack/ServiceStack.Gap/tree/master/src/BenchmarksAnalyzer
It's also available as a free online service at: https://httpbenchmarks.servicestack.net/
