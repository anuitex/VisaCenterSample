using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisaCenter.Core;
using VisaCenter.DomainEvents;
using VisaCenter.Interfaces.Handlers;
using VisaCenter.Repository.Repositories;

namespace VisaCenter.AppHandlers
{
    public class VisaCheckHandler : IEventHandler<VisaRegistredEvent, object>
    {
        private readonly IVisaRepository _visaRepository;

        public VisaCheckHandler(IVisaRepository visaRepository)
        {
            _visaRepository = visaRepository;
        }
        public async Task<object> HandleAsync(VisaRegistredEvent ev, IBus bus)
        {
            //work on visa
            try
            {
                ev.Visa.VisaStatus = "In proggress";
                await _visaRepository.UpdateAsync(ev.Visa);

                await Core.Core.ValidateInitialData();

                ev.Visa.VisaStatus = "Validated";
                await _visaRepository.UpdateAsync(ev.Visa);

                await Core.Core.CheckPersonDataFromPolice();

                ev.Visa.VisaStatus = "Approved by police";
                await _visaRepository.UpdateAsync(ev.Visa);

                await Core.Core.CheckPersonFromLocalGoverment();

                ev.Visa.VisaStatus = "Approved by Goverment";
                await _visaRepository.UpdateAsync(ev.Visa);

                await Core.Core.GeneratePdf();

                ev.Visa.VisaStatus = "Prepared pdf version";
                await _visaRepository.UpdateAsync(ev.Visa);

                await Core.Core.CheckPhotoIsNotPhotoshopEdited();

                ev.Visa.VisaStatus = "Photo validated";
                await _visaRepository.UpdateAsync(ev.Visa);

                await Core.Core.CalculateCreditRating();

                ev.Visa.VisaStatus = "Congratulations you visa approved. You will receive it via mail.";
                await _visaRepository.UpdateAsync(ev.Visa);
                return null;
            }
            catch(Exception ex)
            {
                ev.Visa.VisaStatus = "Visa approval failed, please contact us to get more information.";
                return null; 
            }

           
        }
    }
}
