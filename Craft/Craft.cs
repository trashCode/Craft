using System;
using System.Collections.Generic;

public class Craft
{

    public static List<Craft> craftotheque = new List<Craft>();
    public string name;
    public int level;
    public int batchSize;
    public Dictionary<Craft, int> recipe;

    /**********************************
    Constructors             
    **********************************/
    
    //constructors
    public Craft()
    {
        this.name = "empty";
        this.batchSize = 0;
        this.recipe = new Dictionary<Craft, int>();
        //Craft.craftotheque.Add(this);
    }

    public Craft(string name,int batchSize)
    {

        foreach (Craft c in Craft.craftotheque)
        {
            if (c.name.ToLower() == name.ToLower())
            {
                throw new Exception("Craft name already taken");
            }
        }

        this.name = name;
        this.batchSize = batchSize;
        this.recipe = new Dictionary<Craft, int>();
        Craft.craftotheque.Add(this);
    }


    /**********************************
    Internal functions (all are public for now, might change soon)             
    **********************************/

    public Dictionary<Craft, int> getRecipe(int targetLevel)
    {
        Dictionary<Craft, int> result = new Dictionary<Craft, int>(this.recipe);
        return Craft.getRecipe(result, targetLevel);
    }

    public void add(Craft craft, int qt)
    {
        //todo : verifier que les compo et sous compo ne contient pas this => pour eviter les redondances cycliques
        if (this.recipe.ContainsKey(craft))
        {
            this.recipe[craft] += qt;
        } else
        {
            this.recipe.Add(craft, qt);
        }
        this.level = this.getLevel();
    }

    public void add(String craftName, int qt = 1)
    {
        if (exists(craftName))
        {
            this.add(get(craftName), qt);
        } else
        {
            throw new Exception(String.Format("Craft \"{0}\" does not exist", craftName));
        }
    }

    public Dictionary<Craft, int> multiplyRecipe(int factor)
    {
        Dictionary<Craft, int> rs = new Dictionary<Craft, int>();

        foreach (KeyValuePair<Craft, int> compo in this.recipe)
        {
            rs.Add(compo.Key, compo.Value * factor);
        }
        return rs;
    }

    public int getLevel()
    {

        if (this.recipe.Count < 1)
        {
            return 0;
        } else
        {

            int max = 0;

            //il faut iterer un dictionnaire
            foreach (KeyValuePair<Craft, int> compo in this.recipe)
            {
                max = System.Math.Max(max, compo.Key.getLevel());
            }

            return max + 1;
        }


    }

    public void showRecipe(int level)
    {
        Console.WriteLine("{0}\n{1}", this.name, new String('=', this.name.Length));
        Craft.showRecipe(this.getRecipe(level));
    }

    //obsolete.
    public void incorporate(Dictionary<Craft, int> subRecipe)
    {
        Console.WriteLine("OBSOLETE FUNCTION CALLED");

        foreach (KeyValuePair<Craft, int> compo in subRecipe)
        {

            if (this.recipe.ContainsKey(compo.Key))
            {
                this.recipe[compo.Key] += compo.Value;
            } else
            {
                this.recipe.Add(compo.Key, compo.Value);
            }
        }
    }

    /**********************************
    Static functions for console interface             
    **********************************/
    private static Boolean exists(string name){
        foreach (Craft c in Craft.craftotheque)
        {
            if (c.name.ToLower() == name.ToLower())
            {
                return true;
            }
        }

        return false;
    }

    public static Craft get(string name)
    {
        foreach (Craft c in Craft.craftotheque)
        {
            if (c.name.ToLower() == name.ToLower())
            {
                return c;
            }
        }
        throw new Exception("Craft not found");
    }

    public static void showAll(){
        foreach (Craft c in Craft.craftotheque)
        {
            Console.WriteLine(c.name);
        }
    }

    public static void create(string name,int batchSize)
    {
        try
        {
            if (Craft.exists(name))
            {
                throw new Exception(String.Format("Craft \"{0}\" already defined", name));
            }
            Craft x = new Craft(name,batchSize);

        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot create craft : {0}", e.Message);
        }
    }

    public static Dictionary<Craft, int> multiplyRecipe(Dictionary<Craft, int> recipe, int factor)
    {
        Dictionary<Craft, int> rs = new Dictionary<Craft, int>();

        foreach (KeyValuePair<Craft, int> compo in recipe)
        {
            rs.Add(compo.Key, compo.Value * factor);
        }
        return rs;
    }
    
    public static void incorporate(Dictionary<Craft, int> recipe, Dictionary<Craft, int> subRecipe)
    {
        foreach (KeyValuePair<Craft, int> compo in subRecipe)
        {

            if (recipe.ContainsKey(compo.Key))
            {
                recipe[compo.Key] += compo.Value;
            }
            else
            {
                recipe.Add(compo.Key, compo.Value);
            }
        }
    }

    public static Dictionary<Craft, int> getRecipe(Dictionary<Craft,int> recipe, int targetLevel)
    {
        Dictionary<Craft, int> result = new Dictionary<Craft, int>();
        int currentLevel = getRecipeLevel(recipe);

        if (currentLevel == targetLevel)
        {
            return recipe;
        }
        //A changer : on doit calculer toutes les recettes par level : pour faire des regroupement par batch size.
        for (int level = currentLevel-1; level >= targetLevel; level--)
        {
            //etape 1 : simplifier la recette en 1.(et regrouper les compos)
            foreach (KeyValuePair<Craft, int> compo in recipe)
            {
                if (compo.Key.level > level){

                    Craft.incorporate(result, Craft.multiplyRecipe(compo.Key.recipe, compo.Value));
                }
                else
                {
                    result.Add(compo.Key, compo.Value);
                }
            }

            //etape 2 : ajuster les compos pour tenir compte des batchsize.
            //Attention : ne faire les regroupement que pour les compo du level actuel (les level plus bas doivent attendre pour etre regroupés)
            foreach (KeyValuePair<Craft, int> compo in recipe)
            {
                if (compo.Key.level == level && compo.Value%compo.Key.batchSize != 0 )
                {
                    recipe[compo.Key] += (compo.Key.batchSize - compo.Value%compo.Key.batchSize);
                    //autrement compté : recipe[compo.Key] = compo.Key.batchSize * ((compo.Value / compo.Key.batchsier) + 1) 
                }
            }
            Craft.showRecipe(recipe);
        }
        return recipe;
        
    }

    public static int getRecipeLevel(Dictionary<Craft, int> recipe)
    {
        int max = 0;
        
        foreach (KeyValuePair<Craft,int> compo in recipe)
        {
            max = Math.Max(max, compo.Key.level);
        }

        return max;
    }

    public static void showRecipe(Dictionary<Craft, int> recipe)
    {
        foreach (KeyValuePair<Craft, int> compo in recipe)
        {
            Console.WriteLine("{0} (x{1})", compo.Key.name, compo.Value);
        }
    }

    public static void initSample()
    {
        Craft.create("iron ore", 1);
        Craft.create("copper ore", 1);
        Craft.create("silicon ore", 1);
        Craft.create("cobalt ore", 1);


        
        Craft.create("iron ingot",10);
        (Craft.get("iron ingot")).add("iron ore", 5);

        Craft.create("copper ingot", 10);
        (Craft.get("copper ingot")).add("copper ore", 5);

        Craft.create("silicon ingot", 10);
        (Craft.get("silicon ingot")).add("silicon ore", 5);

        Craft.create("cobalt ingot", 10);
        (Craft.get("cobalt ingot")).add("cobalt ore", 5);

        Craft.create("metal plate", 10);
        (Craft.get("metal plate")).add("iron ingot", 5);
    }

}//end class Craft
