# jcs-es
## Event-Sourced Job Costing Demo

### Setup

1. Install the latest .Net5.0 SDK [here][net5].
2. If you don't have Visual Studio 2019 installed, open your favorite shell and navigate to the repository root, otherwise just open the *jcs-es.sln* file.
3. Build the solution, using `dotnet build` command if you're in a shell; the solution should build just fine, although a `dotnet restore` may also be necessary.
4. Install the latest MongoDb Community Server edition [here][mongo].
5. Add the `mongod` and `mongo` executables to your PATH
6. Create or choose some directory to contain the database files
7. In a new shell, submit a `mongod --dbpath <data_directory>` command to start the mongodb server, where `<data_directory>` is the relative or absolute path to your chose directory for the mongodb database files
8. In a new shell, submit a `mongo` command; you should see some output followed by the caret prompt `>`. Submitting a `show dbs` command should reveal that your shell is now connected to the mongodb server

### Testing

1. Run all the tests, using `dotnet test` if you're in the shell; all tests should pass except for one, which shows something like

   ```shell
   Expected: 0
   Actual:   125
   ```

   The `Actual` value is the number of milliseconds it took to rehydrate an aggregate from 10001 events.

2. If you like you can issue the following commands through mongodb

   ```shell
   use jcs
   db.events.find().pretty()
   db.events.count()
   db.events.drop()
   ```

   

   [net5]: https://dotnet.microsoft.com/download/dotnet/5.0
   [mongo]: https://www.mongodb.com/try/download/community

   

   