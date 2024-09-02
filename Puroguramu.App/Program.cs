#pragma warning disable CA1506
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Puroguramu.App.Middlewares;
using Puroguramu.Domains;
using Puroguramu.Infrastructures.Data;
using Puroguramu.Infrastructures.Roslyn;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILogger, Logger<object>>();
builder.Services.AddScoped<ReverseProxyLinksMiddleware>();
builder.Services.AddScoped<IExercisesRepository, ExercisesRepository>();
builder.Services.AddScoped<IAssessExercise, RoslynAssessor>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
builder.Services.AddScoped<ILessonsRepository, LessonsRepository>();

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("PuroguramuDatabase");

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<PuroguramuDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<PuroguramuDbContext>(options =>
            options.UseSqlite(connectionString));
}

builder.Services
    .AddDefaultIdentity<IdentitySchoolMember>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PuroguramuDbContext>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions());
app.UseReverseProxyLinks();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
#pragma warning restore CA1506
