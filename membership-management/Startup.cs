using membership_management.Contracts;
using membership_management.Data;
using membership_management.Mappings;
using membership_management.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace membership_management
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(
                    Configuration.GetConnectionString("DefaultConnection")));

            // added repositories to the container
            services.AddScoped<IMembershipTypeRepository, MembershipTypeRepository>();
            services.AddScoped<IMembershipRequestRepository, MembershipRequestRepository>();
            services.AddScoped<IMembershipAllocationRepository, MembershipAllocationRepository>();
            services.AddAutoMapper(typeof(Maps));
            // services.AddDefaultIdentity<Employee>(options => options.SignIn.RequireConfirmedAccount = true)
            services.AddDefaultIdentity<Employee>()
                .AddRoles<IdentityRole>() // roles will be initiated
                .AddEntityFrameworkStores<ApplicationDbContext>();
                
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            SeedData.Seed(userManager, roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
