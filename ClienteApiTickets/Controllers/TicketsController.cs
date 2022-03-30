using ClienteApiTickets.Filters;
using ClienteApiTickets.Models;
using ClienteApiTickets.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteApiTickets.Controllers
{
    public class TicketsController : Controller
    {
        private ServiceApiTickets service;

        public TicketsController(ServiceApiTickets service)
        {
            this.service = service;
        }


        public IActionResult Index()
        {
            return View();
        }
        [AuthorizeUsuarios]
        public async Task<IActionResult> ListTickets()
        {
            List<Ticket> tickets =
            await this.service.GetTicketsAsync();
            return View(tickets);
        }
        public IActionResult CreateTicket()
        {
            return View();
        }
        [AuthorizeUsuarios]

        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            await this.service.InsertTicketAsync
            (ticket.IdTicket,ticket.IdUsuario,ticket.Fecha,ticket.Importe,ticket.Producto,ticket.Fileanme,ticket.StoragePath);
            return RedirectToAction("ListTickets","Tickets");
        }
        [AuthorizeUsuarios]

        public IActionResult CreateUser()
        {
            return View();
        }
     

        [HttpPost]
        public async Task<IActionResult> CreateUser(Usuario usuario)
        {
            await this.service.InsertUsertAsync
            (usuario.IdUsuario,usuario.Nombre,usuario.Apellidos,usuario.Email,usuario.Username,usuario.Password);
            return RedirectToAction("Index", "Home");
        }
        [AuthorizeUsuarios]

        public IActionResult BuscarTicket()
        {
            return View();
        }        [AuthorizeUsuarios]

        [HttpPost]
        public async Task<IActionResult> BuscarTicket(int id)
        {
            Ticket ticket = await this.service.FindTicketAsync(id);
            return View(ticket);
        }


    }
}
