using System;

namespace CoolestLibrary
{
    public class Actor : IArtist
    {
        private readonly int _loveOfShakespeare;

        public Actor(int loveOfShakespeare)
        {
            _loveOfShakespeare = loveOfShakespeare;
        }
        public void MakeArt()
        {
            for (int i = 0; i < _loveOfShakespeare; i++)
            {
                Console.WriteLine("till Birnam wood do come to Dunsinane...");
            }
        }
    }
}
