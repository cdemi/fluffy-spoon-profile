using System;
using DemoFluffySpoon.Contracts.Models;

namespace DemoFluffySpoon.Profile.States
{
    [Serializable]
    public class UserProfileState
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }

        public string Email { get; set; }
        
        public bool IsActive { get; set; }
        
        public void Apply(UserRegisteredEvent @event)
        {
            Name = @event.Name;
            Surname = @event.Surname;
            Email = @event.Email;
        }
        
        public void Apply(UserVerificationEvent @event)
        {
            if (@event.Status == UserVerificationStatusEnum.Verified)
            {
                IsActive = true;
            }
        }
    }
}