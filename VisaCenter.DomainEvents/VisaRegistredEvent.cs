using System;
using System.Collections.Generic;
using System.Text;
using VisaCenter.Interfaces.Handlers;
using VisaCenter.Repository.Models;

namespace VisaCenter.DomainEvents
{
    public class VisaRegistredEvent : IDomainEvent
    {
        public Visa Visa { get; set; }
    }
}
