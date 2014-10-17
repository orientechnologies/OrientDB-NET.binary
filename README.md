OrientDB-NET.binary is C#/.NET driver for [OrientDB](http://www.orientdb.org/) document/graph database which implements network binary protocol.

Check out [wiki docs](https://github.com/yojimbo87/OrientDB-NET.binary/wiki) to learn more.

This fork contains the following improvements 

* Better handling of ordered edges
* Support for LoadRecord and CreateRecord operations - faster than performing the same action via SQL commands
* Improved mapping code for generic types to/from ODocuments - much faster, avoids repeated reflection
* Support for fetch plans using LoadRecord to pull back a whole tree of objects in one request
* Initial support for transactional create/update/delete - should be much faster than multiple individual SQL commands
* Better support for derived types - .ToUnique<TBase> will construct a TDerived if the database record is of type TDerived 
* Initial support for the OrientDB 2.0 binary protocol
* Automatic schema creation from public properties of C# types (Database.Create.Class<T>().CreateProperties().Run())

Fetching a large block of records from the DB via Database.Load.ORID(orid).FetchPlan(plan) and converting them to typed objects about 6 times faster than original code using SQL commands and old mapping.

Storing a large block of records into the DB via a transaction is about 10 times faster than doing it via individual SQL create statements.

How To Use
----------

This code is still under active development, and is not yet production quality in all areas. There are currently no official binary downloads 
since it is very likely that you will need to read, debug through and possibly change the driver code in order to get your project working. The 
best way to use this package is to check out this code as a git submodule and add the Orient.Client and Orient.Tests projects to your solution.

The unit tests should run cleanly and will start and stop OrientDB on the local machine themselves. You will likely need to change the values of
the orientDBDir and jreDir variables in the AssemlySetup class to point correctly to your local OrientDB install and JRE/JDK for this to work 
properly.