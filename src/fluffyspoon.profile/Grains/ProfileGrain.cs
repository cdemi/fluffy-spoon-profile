using demofluffyspoon.contracts;
using fluffyspoon.profile.contracts.Grains;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace fluffyspoon.profile.Grains
{
    public class ProfileGrain : Grain<UserProfileState>, IProfileGrain, IAsyncObserver<UserRegisteredEvent>, IAsyncObserver<UserVerifiedEvent>
    {
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.StreamProviderName);

            var userRegisteredStream = streamProvider.GetStream<UserRegisteredEvent>(this.GetPrimaryKey(), nameof(UserRegisteredEvent));
            await userRegisteredStream.SubscribeAsync(this);

            var userVerifiedStream = streamProvider.GetStream<UserVerifiedEvent>(this.GetPrimaryKey(), nameof(UserVerifiedEvent));
            await userVerifiedStream.SubscribeAsync(this);

            await base.OnActivateAsync();
        }

        public Task OnNextAsync(UserRegisteredEvent item, StreamSequenceToken token = null)
        {
            State.Name = item.Name;
            State.Surname = item.Email;
            State.Email = item.Email;

            return Task.CompletedTask;
        }

        public Task OnNextAsync(UserVerifiedEvent item, StreamSequenceToken token = null)
        {
            State.IsActive = true;

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