using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            z.showRecipe(0);
        }
    }
}
