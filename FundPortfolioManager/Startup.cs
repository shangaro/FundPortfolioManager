using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using FundPortfolioManager.Models;
using DocumentProcessor;
using FundPortfolioManager.Services;
using FundPortfolioManager.Data;
using FundPortfolioManager.Repository;
using Microsoft.EntityFrameworkCore;

namespace FundPortfolioManager
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
            // add sql support
            services.AddDbContext<SalesDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SalesSqlConnectionString"));
            });

            services.AddControllersWithViews();
            services.AddOptions();
            services.Configure<AwsBucketConfig>(Configuration.GetSection("AWSBucket"));
            services.Configure<PDFReactorConfig>(Configuration.GetSection("PDFReactor"));
            services.AddAWSService<IAmazonS3>(Configuration.GetAWSOptions());
            services.AddSingleton<IBucketRepository, BucketRepository>();
            services.AddScoped<IPdfProcessor, PdfProcessor>();

            // sales database seeder
            services.AddTransient<SalesDbSeeder>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,SalesDbSeeder salesDbSeeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //seed sales database
            salesDbSeeder.SeedDataAsync(app.ApplicationServices).Wait();
        }
    }
}
