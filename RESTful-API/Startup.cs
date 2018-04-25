using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Entities;
using RESTfulAPI.Services;
using RESTfulAPI.Helpers;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace RESTfulAPI
{
    public class Startup
    {

        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            });

            var connectionString = Startup.Configuration["connectionStrings:libraryDBConncectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<ILibraryRepository, LibraryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler(appBuilder => {
                    appBuilder.Run(async context => {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected fault happened. Try again later.");
                    });
                });
            }

            libraryContext.EnsureSeedDataForContext();

            InitializeAutoMapper();
            
            app.UseMvc();

        }

        private void InitializeAutoMapper() {
            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<Entities.Author, Models.AuthorDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{ src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
                cfg.CreateMap<Entities.Book, Models.BookDto>();
            });
        }
    }
}
