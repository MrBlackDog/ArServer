using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    class SatteliteList : List<Satellite>
    {
        public SatteliteList(String catalog)
        {
            var SattelitesInfo = FtpManager.GetEpfemerids(catalog);
            var HeaderInfo = FtpManager.GetHeader(catalog);
            foreach (var satteliteInfo in SattelitesInfo)
            {
                this.Add(new Satellite(satteliteInfo, HeaderInfo[0]));
            }
        }
    }
}
