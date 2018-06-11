// Copyright (c) Enable International Ltd. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Enable.Azure.WebJobs.Mqtt.Messaging;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Enable.Azure.WebJobs.Mqtt
{
    public class MqttListener : IListener
    {
        private readonly MqttConfiguration _config;
        private readonly string _topicName;
        private readonly ITriggeredFunctionExecutor _triggerExecutor;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private MqttClient _receiver;
        private bool _started;
        private bool _disposed;

        public MqttListener(
            MqttConfiguration config,
            string topicName,
            ITriggeredFunctionExecutor triggerExecutor)
        {
            _config = config;
            _topicName = topicName;
            _triggerExecutor = triggerExecutor;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_started)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            _receiver = _config.MessagingProvider.GetMessageClient(_topicName);

            await _receiver.RegisterMessageHandler(ProcessMessageAsync);

            _started = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (!_started)
            {
                throw new InvalidOperationException(
                    "The listener has not yet been started or has already been stopped.");
            }

            _cancellationTokenSource.Cancel();

            await _receiver.CloseAsync();

            _receiver = null;
            _started = false;
        }

        public void Cancel()
        {
            ThrowIfDisposed();

            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();

                    if (_receiver != null)
                    {
                        _receiver.CloseAsync().Wait();
                        _receiver = null;
                    }
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private async Task ProcessMessageAsync(MqttMessage message, CancellationToken cancellationToken)
        {
            var input = new TriggeredFunctionData
            {
                TriggerValue = message
            };

            var result = await _triggerExecutor.TryExecuteAsync(input, cancellationToken);

            if (!result.Succeeded)
            {
                cancellationToken.ThrowIfCancellationRequested();

                throw result.Exception;
            }
        }
    }
}
