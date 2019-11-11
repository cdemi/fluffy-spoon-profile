using demofluffyspoon.contracts;
using demofluffyspoon.contracts.Grains;
using demofluffyspoon.contracts.Models;
using fluffyspoon.profile.States;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace fluffyspoon.profile.Grains
{
    [ImplicitStreamSubscription(nameof(UserRegisteredEvent))]
    [ImplicitStreamSubscription(nameof(UserVerifiedEvent))]
    public class ProfileGrain : Grain<UserProfileState>, IProfileGrain, IAsyncObserver<UserRegisteredEvent>,
        IAsyncObserver<UserVerifiedEvent>
    {
        private readonly ILogger<ProfileGrain> _logger;

        public ProfileGrain(ILogger<ProfileGrain> logger)
        {
            _logger = logger;
        }
        
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.StreamProviderName);

            var userRegisteredStream =
                streamProvider.GetStream<UserRegisteredEvent>(this.GetPrimaryKey(), nameof(UserRegisteredEvent));
            await userRegisteredStream.SubscribeAsync(this);

            var userVerifiedStream =
                streamProvider.GetStream<UserVerifiedEvent>(this.GetPrimaryKey(), nameof(UserVerifiedEvent));
            await userVerifiedStream.SubscribeAsync(this);

            await base.OnActivateAsync();
        }

        public Task OnNextAsync(UserRegisteredEvent item, StreamSequenceToken token = null)
        {
            _logger.LogInformation("----------RECEIVED MESSAGE----------");
            State.Name = item.Name;
            State.Surname = item.Surname;
            State.Email = item.Email;

            return Task.CompletedTask;
            
            //RaiseEvent(item);

            //return ConfirmEvents();

        }

        public Task OnNextAsync(UserVerifiedEvent item, StreamSequenceToken token = null)
        {
            State.IsActive = true;

            return Task.CompletedTask;
            
            //RaiseEvent(item);

            //return ConfirmEvents();
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