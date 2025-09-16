using System;
using System.Collections.Generic;

namespace ScatterBrainersV2
{
    // Program Class contains Main special static method - Application entry point
    class Program
    {
        // .NET looks for method "MAIN" to start execution of app, ends on finish
        // static methods belong to class, not an instance; can return nothing or exit code
        static void Main(string[] args) // args is string array of arguments passed in CLI
        {
            Game game = new Game();
            game.Start();
            game.Play();
            game.End();
        }
    }
}