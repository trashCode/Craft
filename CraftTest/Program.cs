using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Craft x = new Craft("simpleCraft0");
            Craft y = new Craft("simpleCraft1");
            Craft z = new Craft("complexCraft");
            z.add(x,2);
            z.add(y,3);

            Console.WriteLine("complex Recipe : ");
            z.showRecipe(0);

            Console.WriteLine("\nknown Crafts :{0}",Craft.craftotheque.Count);

            foreach (Craft c in Craft.craftotheque)
            {
                Console.WriteLine(c.name);
            }

            
        }
    }
}
