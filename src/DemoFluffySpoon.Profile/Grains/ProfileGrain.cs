using System;
using System.Threading.Tasks;
using DemoFluffySpoon.Contracts;
using DemoFluffySpoon.Contracts.Grains;
using DemoFluffySpoon.Contracts.Models;
using DemoFluffySpoon.Profile.States;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Streams;

namespace DemoFluffySpoon.Profile.Grains
{
    [ImplicitStreamSubscription(nameof(UserRegisteredEvent))]
    [ImplicitStreamSubscription(nameof(UserVerificationEvent))]
    public class ProfileGrain : JournaledGrain<UserProfileState>, IProfileGrain, IAsyncObserver<UserRegisteredEvent>,
        IAsyncObserver<UserVerificationEvent>
    {
        private readonly ILogger<ProfileGrain> _logger;

        public ProfileGrain(ILogger<ProfileGrain> logger)
        {
            _logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.StreamProviderName);

            // Consumer
            var userRegisteredStream =
                streamProvider.GetStream<UserRegisteredEvent>(this.GetPrimaryKey(), nameof(UserRegisteredEvent));
            await userRegisteredStream.SubscribeAsync(this);

            // Consumer
            var userVerificationStream =
                streamProvider.GetStream<UserVerificationEvent>(this.GetPrimaryKey(), nameof(UserVerificationEvent));
            await userVerificationStream.SubscribeAsync(this);

            await base.OnActivateAsync();
        }

        public Task OnNextAsync(UserRegisteredEvent item, StreamSequenceToken token = null)
        {
            RaiseEvent(item);

            return Task.CompletedTask;
        }

        public Task OnNextAsync(UserVerificationEvent item, StreamSequenceToken token = null)
        {
            RaiseEvent(item);

            _logger.LogInformation("{name} <{email}> is now active", TentativeState.Name, TentativeState.Email);

            return Task.CompletedTask;
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}