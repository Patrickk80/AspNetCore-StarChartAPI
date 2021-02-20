using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            Models.CelestialObject obj = _context.CelestialObjects.Find(id);
            if (obj == null) return NotFound();

            var sattelites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            obj.Satellites = sattelites;
            return Ok(obj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);
            if (!celestialObjects.Any()) return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects.ToList());        
        }
        
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id } , celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null) return NotFound();
            
            obj.Name = celestialObject.Name;
            obj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            obj.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null) return NotFound();

            obj.Name = name;

            _context.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var obj = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);
            if (!obj.Any()) return NotFound();
            
            _context.RemoveRange(obj);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
