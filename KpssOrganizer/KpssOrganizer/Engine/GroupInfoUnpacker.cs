using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpssOrganizer.Engine
{
    class GroupInfoUnpacker
    {
        public Dictionary<string, string> BoldedDates = new Dictionary<string, string>();
        public List<string> Users = new List<string>();
        public List<string> Events = new List<string>();

        public GroupInfoUnpacker(string info)
        {
            string[] data = info.Split('\n');

            if (data[0].Length > 0)
                Users = data[0].Split('&').ToList();

            if (data[1].Length > 0)
            {
                foreach (var date in data[1].Split('&'))
                {
                    string[] temp = date.Split('^');
                    BoldedDates.Add(temp[0], temp[1]);
                }
            }

            if (data[2].Length > 0)
              Events = data[2].Split('&').ToList();
        }
    }
}
