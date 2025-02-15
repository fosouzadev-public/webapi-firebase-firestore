using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

AddFirebaseFirestore(builder.Services, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

void AddFirebaseFirestore(IServiceCollection services, IConfiguration configuration)
{
    FirestoreDbBuilder builder = new()
    {
        JsonCredentials = configuration["Firestore:Credentials"],
        ProjectId = configuration["Firestore:ProjectId"]
    };

    services.AddSingleton(provider => builder.Build());
}