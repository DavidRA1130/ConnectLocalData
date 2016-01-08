using System;
using IronPython.Hosting;

public class Class1
{
	public Class1()
	{

        var engine = Python.CreateEngine();

        var theScript = @"import json
                        import urllib2
                        result = json.load(urllib2.urlopen(""http://ondemand.websol.barchart.com/getQuote.json?apikey=510a0e5d6b43d16805589ee06a0e8e8c&symbols=IBM,FB,AAPL""))

                     print result
          ";

        // execute the script
        var result = engine.Execute(theScript);

        return result;

    }
}
