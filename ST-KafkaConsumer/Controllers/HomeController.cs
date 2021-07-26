using Microsoft.AspNetCore.Mvc;
//using ST_KafkaConsumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST_KafkaConsumer.Controllers
{
    public class HomeController : Controller
    {
        public KafkaContext db;
        public HomeController(KafkaContext context)
        {
            db = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Temperature temperature)
        {
            db.Temperatures.Add(temperature);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
