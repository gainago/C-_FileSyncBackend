using FileSyncBackend.Data;
using FileSyncBackend.Repositories;
using FileSyncBackend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ICommitRepository, CommitRepository>();
builder.Services.AddScoped<IBlobRepository, BlobRepository>();
builder.Services.AddScoped<ISyncSnapshotRepository, SyncSnapshotRepository>();
builder.Services.AddScoped<IConflictRepository, ConflictRepository>();

// Register services
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<ICommitService, CommitService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IConflictResolutionService, ConflictResolutionService>();

// Configure CORS for browser client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();