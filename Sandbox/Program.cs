// See https://aka.ms/new-console-template for more information
using Sandbox;

using (Window window = new Window(800, 600, "MyGameEngine"))
{
    //Run takes a double, which is how many frames per second it should strive to reach.
    //You can leave that out and it'll just update as fast as the hardware will allow it.
    window.RenderFrequency = 60;
    window.Run();
}