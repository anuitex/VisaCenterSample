using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VisaCenter.AppHandlers;
using VisaCenter.DomainEvents;
using VisaCenter.Interfaces.Handlers;
using VisaCenter.Repository.Models;
using VisaCenter.Repository.Repositories;
using VisaCenter.Web.Models;

namespace VisaCenter.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBus _bus;
        private readonly IVisaRepository _visaRepository;

        public HomeController(IBus bus, IVisaRepository visaRepository)
        {
            _bus = bus;
            _visaRepository = visaRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Visa visa)
        {
            await _visaRepository.CreateAsync(visa);
            Task.Run(async () =>
            {
                await _bus.Raise(new VisaRegistredEvent { Visa = visa });
            });


            return RedirectToAction("Results", new { Id = visa.Id });
        }


        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            //var v = (await _visaRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            //return View("Results", v);

            var v = await _bus.Raise<VisaStatusCheckEvent, string>(new VisaStatusCheckEvent { Id = id });
            return View("Results", new Visa { VisaStatus = v });
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
