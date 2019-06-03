using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IpShareServer;
using System.Numerics;
using Microsoft.AspNetCore.Mvc;

namespace IpShareServer.Controllers
{
    public class CoordShareController : Controller
    {
        public volatile static Dictionary<string, Vector3> Measurments = new Dictionary<string, Vector3>();
        public volatile static Dictionary<string, Vector3> Clock = new Dictionary<string, Vector3>();

        [HttpGet]
        public IActionResult getMeasurments(String Name, long x, long y, long z)
        {
            if (Measurments.ContainsKey(Name))
                Measurments[Name] = new Vector3(x, y, z);
            else
                Measurments.Add(Name, new Vector3(x, y, z));
            return Ok();
        }

        [HttpGet]
        public IActionResult getTime(String Name, float x, float y, float z)
        {
            if (Clock.ContainsKey(Name))
                Clock[Name] = new Vector3(x, y, z);
            else
                Clock.Add(Name, new Vector3(x, y, z));
            return Ok();
        }

        [HttpGet]
        public IActionResult GetFriendsCoords(String Name)
        {
            if (Measurments.ContainsKey(Name))
                return Content($"{Measurments[Name].X};{Measurments[Name].Y};{Measurments[Name].Z}");
            return BadRequest();
        }
    }
}