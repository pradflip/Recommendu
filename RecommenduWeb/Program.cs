using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Services;
using Microsoft.Extensions.ML;
using static RecommenduWeb.AnaliseDescricao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<LocalidadeService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ComentarioService>();
builder.Services.AddScoped<EnvioEmailService>();
builder.Services.AddScoped<AdminService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>()
    .FromFile("AnaliseDescricao.mlnet");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// PostagensController
app.MapControllerRoute(name: "postagens",
                        pattern: "/postagens/{userName}",
                        defaults: new { controller = "Postagens", action = "Index" });
app.MapControllerRoute(name: "postagem-completa",
                        pattern: "/postagens/detalhes/{id}/{cat}",
                        defaults: new { controller = "Postagens", action = "Details" });
app.MapControllerRoute(name: "nova-postagem",
                        pattern: "/postagens/minhas-postagens/nova",
                        defaults: new { controller = "Postagens", action = "Create" });
app.MapControllerRoute(name: "editar-postagem",
                        pattern: "/postagens/minhas-postagens/editar/{id}/{cat}",
                        defaults: new { controller = "Postagens", action = "Edit" });
app.MapControllerRoute(name: "excluir-postagem",
                        pattern: "/postagens/minhas-postagens/excluir/{id}/{cat}",
                        defaults: new { controller = "Postagens", action = "Delete" });
app.MapControllerRoute(name: "curtir-postagem",
                        pattern: "/postagens/curtir/{postId}/{cat}/{userId}/{acao}/{Count}",
                        defaults: new { controller = "Postagens", action = "Curtir" });
app.MapControllerRoute(name: "reportar-postagem",
                        pattern: "/postagens/reportar/{postId}/{cat}/{Count}",
                        defaults: new { controller = "Postagens", action = "Reportar" });
app.MapControllerRoute(name: "produtos",
                        pattern: "/encontrar/produtos",
                        defaults: new { controller = "Postagens", action = "Produtos" });
app.MapControllerRoute(name: "servicos",
                        pattern: "/encontrar/servicos",
                        defaults: new { controller = "Postagens", action = "Servicos" });

// UsuariosController
app.MapControllerRoute(name: "usuarios",
                        pattern: "/encontrar/usuarios",
                        defaults: new { controller = "Usuarios", action = "Usuarios" });
app.MapControllerRoute(name: "perfil-usuarios",
                        pattern: "usuarios/atualizar-foto",
                        defaults: new { controller = "Usuarios", action = "AtualizarFoto" });
app.MapControllerRoute(name: "perfil-usuarios",
                        pattern: "usuarios/deletar-foto",
                        defaults: new { controller = "Usuarios", action = "DeletarFoto" });
app.MapControllerRoute(name: "perfil-usuarios",
                        pattern: "usuarios/{userName}",
                        defaults: new { controller = "Usuarios", action = "Index" });

// ComentariosController
app.MapControllerRoute(name: "enviar-comentario",
                        pattern: "/comentarios/enviar/{postId}/{cat}/{userId}/{Count}/{comentario}",
                        defaults: new { controller = "Comentarios", action = "EnviarComentario" });
app.MapControllerRoute(name: "excluir-comentario",
                        pattern: "/comentarios/excluir/{comentId}/{Count}",
                        defaults: new { controller = "Comentarios", action = "ExcluirComentario" });

// HomeController
app.MapControllerRoute(name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
