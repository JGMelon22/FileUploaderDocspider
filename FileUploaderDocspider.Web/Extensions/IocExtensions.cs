using FileUploaderDocspider.Application.Commands;
using FileUploaderDocspider.Application.Commands.Handlers;
using FileUploaderDocspider.Application.Queries;
using FileUploaderDocspider.Application.Queries.Handlers;
using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using FileUploaderDocspider.Infrastructure.Interfaces.Services;
using FileUploaderDocspider.Infrastructure.Repositories;
using FileUploaderDocspider.Infrastructure.Services;

//using FileUploaderDocspider.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.SimpleMediator;
using System.Collections.Generic;

namespace FileUploaderDocspider.Web.Extensions
{
    public static class IocExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<IMediator, Mediator>();

            services.AddTransient<IRequestHandler<GetDocumentByIdQuery, Result<DocumentViewModel>>, GetDocumentByIdQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllDocumentsQuery, Result<IEnumerable<DocumentViewModel>>>, GetAllDocumentsQueryHandler>();
            services.AddTransient<IRequestHandler<CreateDocumentCommand, Result<DocumentViewModel>>, CreateDocumentCommandHandler>();
            services.AddTransient<IRequestHandler<UpdateDocumentCommand, Result<DocumentViewModel>>, UpdateDocumentCommandHandler>();
            services.AddTransient<IRequestHandler<DeleteDocumentCommand, Result<bool>>, DeleteDocumentCommandHandler>();
            services.AddTransient<IRequestHandler<DownloadDocumentQuery, Result<DownloadDocumentResult>>, DownloadDocumentQueryHandler>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDocumentService, DocumentService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            return services;
        }
    }
}
