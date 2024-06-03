﻿using AwesomeDevEvent.API.Entities;
using AwesomeDevEvent.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvent.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        public DevEventsController(DevEventsDbContext context)
        {
            _context = context;    
        }

        // api/dev-events GET
        [HttpGet]
        public IActionResult GetAll() 
        {
            var devEvents = _context.DevEvents.Where(d => !d.isDeleted).ToList();

            return Ok(devEvents);
        }

        // api/dev-events 12349481 GET
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id) 
        {
            var devEvent = _context.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d=> d.Id == id);

            if(devEvent == null)
            {
                return NotFound();
            }

            return Ok(devEvent);
        }

        // api/dev-events / POST
        [HttpPost]
        public IActionResult Post(DevEvent devEvent) 
        {
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new {  id = devEvent.Id }, devEvent);
        }

        //api/dev-events/12312421 PUT
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEvent input) 
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();
            return NoContent();
        }

        // api/dev-events 12349481 DELETE 
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id) 
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();

            _context.SaveChanges();
            return NoContent();
        }

        // api/dev-events/2722B2a6-26d6-43c98-7eCC03d9509e3646e/speakers
        [HttpPost("{id}/speakers")]
        public IActionResult PostSpeakers(Guid id, DevEventSpeaker speaker)
        {
            speaker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
            {
                return NotFound();
            }

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
