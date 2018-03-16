using System;

namespace CoolestLibrary
{
    public class CampusTypeAttribute : Attribute
    {
        public CampusTypeAttribute(
            string schoolName,
            ArchitecturalStyle archStyle)
        {
            SchoolName = schoolName;
            ArchStyle = archStyle;
        }

        public string SchoolName { get; private set; }
        public ArchitecturalStyle ArchStyle { get; private set; }
    }
}
