using System;

namespace CoolestLibrary
{
    [DulcetTones]
    [CampusType("Marymount", ArchitecturalStyle.Gothic)]
    public class Conservatory
    {
        public Func<int, IArtist> ArtistFactory { get; }
        public Func<string, ITeacher> TeacherFactory { get; }

        public Conservatory(
            Func<string, ITeacher> canTeachFactory,
            Func<int, IArtist> canActFactory)
        {
            ArtistFactory = canActFactory;
            TeacherFactory = canTeachFactory;
        }

        public void DoYourThang()
        {
            TeacherFactory("Viola").Teach();

            ArtistFactory(3).MakeArt();
        }
    }
}
