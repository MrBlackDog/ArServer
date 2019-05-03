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
        public volatile static Dictionary<string, Vector3> ListIp = new Dictionary<string, Vector3>();

        [HttpGet]
        public IActionResult SaveMyCoords(String Name, float x, float y, float z)
        {
            if (ListIp.ContainsKey(Name))
                ListIp[Name] = new Vector3(x, y, z);
            else
                ListIp.Add(Name, new Vector3(x, y, z));
            return Ok();
        }

        [HttpGet]
        public IActionResult GetFriendsCoords(String Name)
        {
            if (ListIp.ContainsKey(Name))
                return Content($"{ListIp[Name].X};{ListIp[Name].Y};{ListIp[Name].Z}");
            return BadRequest();
        }
    }
}