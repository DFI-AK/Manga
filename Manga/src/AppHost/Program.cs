var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");

var database = sql.AddDatabase("MangaDb");

builder.AddProject<Projects.Web>("web");

builder.Build().Run();
