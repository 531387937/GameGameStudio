using GameServer.net;
using System;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            NetManager.StartLoop(8888);

            //Room room = new Room();
            //room.TestBattle();
        }
    }
}
