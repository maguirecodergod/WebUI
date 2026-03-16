// Server
services.AddLHAGrpcServer(o => o.EnableReflection = true);
app.MapGrpcService<MyService>();
app.MapLHAGrpcInfrastructure();

// Client
services.AddLHAGrpcClientDefaults(o => o.DefaultDeadline = TimeSpan.FromSeconds(30));
services.AddLHAGrpcClient<UserService.UserServiceClient>("https://user-service:5001")
        .AddStandardResilienceHandler();