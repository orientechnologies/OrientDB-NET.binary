Alpha Released of New Driver
----------------------------------------------
https://github.com/orientechnologies/OrientDB.Net.Core

The .Net Driver has been completely overhauled and rewritten to follow modern standards in database driver patterns.  Please  test and add any issue to the new projects as we work to retire this implementation.

----------------------------------------------
OrientDB-NET.binary is C#/.NET driver for [OrientDB](http://www.orientdb.org/) document/graph database which implements network binary protocol. For more information look at the [Documentation](http://orientdb.com/docs/master/NET.html).

Now Contains
---------------------------------------------

* Written in DNX Release 1 for DNX and .Net 4.5.1 support
* Better handling of ordered edges
* Support for LoadRecord and CreateRecord operations - faster than performing the same action via SQL commands
* Improved mapping code for generic types to/from ODocuments - much faster, avoids repeated reflection
* Support for fetch plans using LoadRecord to pull back a whole tree of objects in one request
* Initial support for transactional create/update/delete - should be much faster than multiple individual SQL commands
* Better support for derived types - .ToUnique<TBase> will construct a TDerived if the database record is of type TDerived 
* Initial support for the OrientDB 2.2 binary protocol
* Automatic schema creation from public properties of C# types (Database.Create.Class<T>().CreateProperties().Run())

Fetching a large block of records from the DB via Database.Load.ORID(orid).FetchPlan(plan) and converting them to typed objects about 6 times faster than original code using SQL commands and old mapping.

Storing a large block of records into the DB via a transaction is about 10 times faster than doing it via individual SQL create statements.

How To Use
----------

This code is still under active development.

For the latest build you can in the driver from [NuGet](https://www.nuget.org/packages/OrientDB-Net.binary.Innov8tive/)

The unit tests should run cleanly and will start and stop OrientDB on the local machine themselves. You will likely need to change the values in the appsettings.json to point correctly to your local OrientDB install.

For more information look at the [Documentation](http://orientdb.com/docs/master/NET.html).
