

using Mono.Unix;
using Mono.Unix.Native;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string socketDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
Directory.CreateDirectory(socketDirectoryPath);
string socketPath = Path.Combine(socketDirectoryPath, "mysocket");
Console.WriteLine(socketPath);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenUnixSocket(socketPath);
});


var app = builder.Build();

app.MapGet("/", () => "Hello world");
app.Lifetime.ApplicationStarted.Register(OnStarted);
app.Run();




void OnStarted()
{
    // this is for "dotnet-app:www-data" chown pattern
    var userId = Syscall.getuid();
    var groupId = Syscall.getpwnam("www-data");
    Syscall.chown(socketPath, userId, groupId.pw_gid);

    Console.WriteLine($"This is uid: {userId}");
    Console.WriteLine($"This is gid: {groupId.pw_gid}");

    //var user = UnixUserInfo.GetRealUser();
    //var group = UnixGroupInfo.GetLocalGroups().Single(x => x.GroupName == "www-data");
    //var fileEntry = UnixFileSystemInfo.GetFileSystemEntry(socketPath);
    //fileEntry.SetOwner(user, group);


}

