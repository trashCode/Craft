using System;
using System.Collections.Generic;

public class Craft
{

    public static List<Craft> craftotheque = new List<Craft>();
    public string name;
    public int level;
    public Dictionary<Craft, int> recipe;

    public Craft()
    {
        this.name = "empty";
        this.recipe = new Dictionary<Craft, int>();
        //Craft.craftotheque.Add(this);
    }

    public Craft(string name)
    {
        //todo Voir si  le craft existe deja dans la craftotheque
        foreach (Craft c in Craft.craftotheque)
        {
            if (c.name.ToLower() == name.ToLower())
            {
                throw new Exception("Craft name already taken");
            }
        }

        this.name = name;
        this.recipe = new Dictionary<Craft, int>();
        Craft.craftotheque.Add(this);
    }

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

    public static void create(string name)
    {
        try
        {
            if (Craft.exists(name))
            {
                throw new Exception(String.Format("Craft {0} already defined", name));
            }
            Craft x = new Craft(name);//not yet added to the craftotheque

        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot create craft : {0}", e.Message);
        }
    }

    public static void create(string name, string[] composants ){
        
        if (Craft.exists(name))
        {
            Console.WriteLine("Craft {0} already defined", name);
            return;//c'est sale ?
        }

        try {
            Craft x = new Craft();//not yet added to the craftotheque
            x.name = name;

            int y = 0;
            
            for (int i = 0; i < composants.Length-1; i+=2) {

                if (!Craft.exists(composants[i]))
                {
                    throw new Exception(String.Format("Cannot find craft component :{0}" , composants[i]));
                }
                

                if (!Int32.TryParse(composants[i+1],out y)) {
                    throw new Exception(String.Format("Cannot add craft component {0} : {1} is not a valid number",composants[i],composants[i+1] ));
                }
                x.add( Craft.get(composants[i]) , y);
            }

            Craft.craftotheque.Add(x);

        }catch(Exception e){
            Console.WriteLine("Cannot create craft : {0}", e.Message);
        }
    }






    public void add(Craft craft, int qt)
    {
        //todo : verifier que les compo et sous compo ne contient pas this => pour eviter les redondances cycliques
        if (this.recipe.ContainsKey(craft))
        {
            this.recipe[craft] += qt;
        }
        else
        {
            this.recipe.Add(craft, qt);
        }
        this.level = this.getLevel();
    }

    public int getLevel()
    {

        if (this.recipe.Count < 1)
        {
            return 0;
        }
        else
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

    public static Dictionary<Craft, int> multiplyRecipe(Dictionary<Craft, int> recipe, int factor)
    {
        Dictionary<Craft, int> rs = new Dictionary<Craft, int>();

        foreach (KeyValuePair<Craft, int> compo in recipe)
        {
            rs.Add(compo.Key, compo.Value * factor);
        }
        return rs;
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

    //obsolete.
    public void incorporate(Dictionary<Craft, int> subRecipe)
    {

        foreach (KeyValuePair<Craft, int> compo in subRecipe)
        {

            if (this.recipe.ContainsKey(compo.Key))
            {
                this.recipe[compo.Key] += compo.Value;
            }
            else
            {
                this.recipe.Add(compo.Key, compo.Value);
            }
        }
    }

    public Dictionary<Craft, int> getRecipe(int level)
    {
        Dictionary<Craft, int> leveledRecipe = new Dictionary<Craft, int>();

        foreach (KeyValuePair<Craft, int> compo in this.recipe)
        {

            if (compo.Key.getLevel() <= level)
            {

                if (leveledRecipe.ContainsKey(compo.Key))
                {
                    leveledRecipe[compo.Key] += compo.Value;
                }
                else
                {
                    leveledRecipe.Add(compo.Key, compo.Value);
                }


            }
            else
            {

                Craft.incorporate(leveledRecipe, Craft.multiplyRecipe(compo.Key.getRecipe(level), (compo.Value)));//this is where the magic happens
            }
        }

        return leveledRecipe;
    }


    public static void showRecipe(Dictionary<Craft, int> recipe)
    {
        foreach (KeyValuePair<Craft, int> compo in recipe)
        {
            Console.WriteLine("{0} (x{1})", compo.Key.name, compo.Value);
        }
    }

    public void showRecipe(int level)
    {
        Console.WriteLine("{0}\n{1}", this.name, new String('=' , this.name.Length) );
        Craft.showRecipe(this.getRecipe(level));
    }

    public static void initSampe()
    {
        Craft.create("iron lingot");
        Craft.create("iron lingot");
    }

}//end class Craft
