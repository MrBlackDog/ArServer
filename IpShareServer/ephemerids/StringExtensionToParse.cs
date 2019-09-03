using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
     static class StringExtensionToParse
    {
        public static List<string> GetSatteliteStringMass(this string input, int n, int Step)
        {
            var RowCollection = input.Split('\n').Where(i => !string.IsNullOrEmpty(i)).ToList();
            var ResultCollection = RowCollection.Where((item, index) => RowCollection.Count - index <= n);
            return GroupBuSattelite(ResultCollection, Step);
        }

        public static List<string> GetHeaderStringMass(this string input, int n, int Step)
        {
            var RowCollection = input.Split('\n').Where(i => !string.IsNullOrEmpty(i)).ToList();
            var ResultCollection = RowCollection.Where((item, index) => index <= n);
            return GroupBuSattelite(ResultCollection, Step);
        }

        private static List<string> GroupBuSattelite(IEnumerable<string> input,int Step)
        {
            var ResultList = new List<String>();
            for (var i=0; i < input.Count(); i=i+ Step)
            {
                ResultList.Add(String.Join("", input.Where((item, index) => index >= i & index < i + Step).ToArray()));
            }
            return ResultList;
        }
    }
}
