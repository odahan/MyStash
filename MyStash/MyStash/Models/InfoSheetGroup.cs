using System.Collections.Generic;

namespace MyStash.Models
{
    public class InfoSheetGroup : List<InfoSheet>
    {
        public string GroupName { get; private set; }
        public string GroupShortName { get; private set; }

        public InfoSheetGroup(string name, string shortName)
        {
            GroupName = name;
            GroupShortName = shortName;
        }
    }
}