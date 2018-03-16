using System;

namespace CoolestLibrary
{
    public class Professor : ITeacher
    {
        public Professor(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Teach()
        {
            Console.WriteLine("learn, yo");
        }
    }
}
