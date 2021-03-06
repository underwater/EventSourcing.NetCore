﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS.Tests.TestsInfrasructure
{
    class ServiceLocator
    {
        private readonly Dictionary<Type, List<object>> Services = new Dictionary<Type, List<object>>();

        public void Register(Type type, params object[] implementations)
            => Services.Add(type, implementations.ToList());

        public List<object> Get(Type type) { return Services[type]; }

        public void RegisterCommandHandler<TCommand, TCommandHandler>(TCommandHandler commandHandler) 
            where TCommand : IRequest
            where TCommandHandler : IRequestHandler<TCommand>
        {
            Register(typeof(IRequestHandler<TCommand>), commandHandler, commandHandler);
            //Registration needed internally by MediatR
            Register(typeof(IPipelineBehavior<TCommand, Unit>), new object[] { });
            Register(typeof(IAsyncRequestHandler<TCommand>), new IAsyncRequestHandler<TCommand>[] { });
        }

        public void RegisterQueryHandler<TQuery, TResponse>(IAsyncRequestHandler<TQuery, TResponse> queryHandler)
            where TQuery : IRequest<TResponse>
        {
            Register(typeof(IAsyncRequestHandler<TQuery, TResponse>), queryHandler);
            //Registration needed internally by MediatR
            Register(typeof(IPipelineBehavior<TQuery, TResponse>), new object[] { });
            Register(typeof(IRequestHandler<TQuery, TResponse>), new IRequestHandler<TQuery, TResponse>[] { });
        }

        public IMediator GetMediator()
        {
            return new Mediator(
                    type => Get(type).FirstOrDefault(),
                    type => Get(type));
        }
    }
}
