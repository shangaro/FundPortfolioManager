// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Messaging.RabbitMQ.Internal
{
    // REVIEW: This class seems mostly overhead, except that we can't really register a singleton
    // for two interface types (IHostedService _and_ IRabbitMQMessenger) without pre-creating
    // the instance.
    public class RabbitMQHostedService : IHostedService
    {
        private IRabbitMQMessenger _messenger;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQHostedService(IServiceProvider serviceProvider)
        {
            // Since creation == initialization/connection, we delay retrieving the service
            // until we're asked to start.
            _serviceProvider = serviceProvider;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _messenger = _serviceProvider.GetRequiredService<IRabbitMQMessenger>();
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _messenger?.Dispose();
            await Task.CompletedTask;
        }
    }
}
