using AutoMapper;
using CakeShop.Core;
using CakeShop.Core.Models;
using CakeShop.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CakeShop
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) // ругистрирует сервисы 
        {
            services.AddMvc();
            services.AddScoped<ICakeRepository, CakeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IShoppingCartService>(sp => ShoppingCartService.GetCart(sp));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<CakeShopDbContext>(ctx =>
            {
                ctx.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddAutoMapper();

            services.AddMemoryCache();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<CakeShopDbContext>();

            //services.AddSession();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/UnAuthorized";
            });

        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env) // обьязательный .
                                                                                //задает как приложение будет обрабатывать запросы
                                                                              //  IHostingEnvironment: позволяет взаимодействовать со средой, в которой запускается приложение
        {
            //добавление компонентов middleware для обработки запрос
            //Метод Configure выполняется один раз при создании объекта класса Startup
            //, и компоненты middleware создаются один раз и живут в течение всего жизненного цикла приложения

            // если приложение в процессе разработки
            if (env.IsDevelopment())
            {
                // то выводим информацию об ошибке, при наличии ошибки
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseStatusCodePages();
            //app.UseSession();
            // установка обработчика статических файлов
            app.UseStaticFiles();

            app.UseAuthentication();
            //устанавливает компоненты MVC для обработки запроса и, в частности, настраивает систему маршрутизации в приложении.
            app.UseMvc(routes =>
            {

                //routes.MapRoute(
                //    name: "categoryFilter",
                //    template: "Cakes/{action}/{category?}",
                //    defaults: new { Controller = "Cake", action = "List" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
