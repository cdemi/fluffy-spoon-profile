using demofluffyspoon.contracts;
using demofluffyspoon.contracts.Models;
using System;

namespace fluffyspoon.profile.States
{
    [Serializable]
    public class UserProfileState
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }

        public string Email { get; set; }
        
        public bool IsActive { get; set; }
        
        public UserProfileState Apply(UserRegisteredEvent @event)
        {
            Name = @event.Name;
            Surname = @event.Surname;
            Email = @event.Email;

            return this;
        }
        
        public UserProfileState Apply(UserVerificationEvent @event)
        {
            if (@event.Status == UserVerificationStatusEnum.Verified)
            {
                IsActive = true;
            }

            return this;
        }
    }
}