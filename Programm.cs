namespace ConsoleApp3;
using System;
public class Programm
{
    public static void Main(string[] args)
    {
        int Spielfeldgroese = 100;
        GameField? GameField = new GameField(Spielfeldgroese);
        
        GameField.Spielen( "Spieler 1", "Spieler 2");
    }
    
    class GameField
    {
        // Internal class:
        
        internal class FieldNode
        {
            Random rnd = new Random();
            public bool Snake { get; }
            public bool Ladder { get; }
            public FieldNode Next { get; set; }
            public FieldNode Previous { get; set; }

            public FieldNode( FieldNode previous, FieldNode next)
            {
                
                    int g =  rnd.Next(1, 7);
                    Snake = (1 == g);
                    Ladder = (2 == g);
                    Next = next;
                    Previous = previous;
            }
        }
        internal class Player{
            
            public string Name;
            public int Throws { get; set; } = 0;
            public FieldNode? Position { get; set; } 

            public Player(string name,FieldNode start)
            {
                this.Name = name;
                Position = start;

            }
        
        }

        public void Spielen(string n1, string n2)
        {
            Random rnd = new Random();
            int spielzug = 0;

            Player[] spieler ={new Player(n1,first),new Player(n2,first)};
            
            while (spieler[0].Position != last||spieler[1].Position != last)
            {
                int wurf = rnd.Next(1, 7);

                if (wurf == 1)
                {
                    Append(5);
                }
                
                
                
                spieler[spielzug % 2].Position=Ziehen(spieler[spielzug % 2].Position,wurf) ;

                if (spieler[spielzug % 2].Position == last)
                {
                    return;
                }

                if (spieler[spielzug % 2].Position == last)// Nach dem Würfeln am Ende 
                {
                    return;
                }


                while (spieler[spielzug % 2].Position.Ladder || spieler[spielzug % 2].Position.Snake)
                {
                    if (spieler[spielzug % 2].Position.Ladder)
                    {
                        spieler[spielzug % 2].Position=Ziehen(spieler[spielzug % 2].Position,3) ;
                    }
                    else
                    {
                        spieler[spielzug % 2].Position=ZurueckZiehen(spieler[spielzug % 2].Position,3) ;

                    }
                    if (spieler[spielzug % 2].Position == last)// Nach dem Würfeln am Ende 
                    {
                        return;
                    }
                    
                }
                









                spielzug++;
            }
            
            
            
        }

        public FieldNode Ziehen(FieldNode f,int Anzahl)
        {
            if (f != last)
            {
                if (Anzahl > 1)
                {
                    return Ziehen(f.Next, Anzahl - 1);
                }
                else
                {
                    return (f.Next);
                }
                
                
            }
            else
            {
                return last;
            }
            
        }
        public FieldNode ZurueckZiehen(FieldNode f,int Anzahl)
        {
            if (f != first)
            {
                if (Anzahl > 1)
                {
                    return Ziehen(f.Previous, Anzahl - 1);
                }
                else
                {
                    return (f.Previous);
                }
                
                
            }
            else
            {
                return first;
            }
            
        }

        public GameField(int Spielfeldgroese)
        {
            Append(Spielfeldgroese);
        }
        // Data fields:

        FieldNode? first = null;
        FieldNode? last = null;

        // Read-only properties:

        public FieldNode? First
        {
            get { return first; }
        }

        public FieldNode? Last
        {
            get { return last; }
        }

        // Methods:
        public void Prepend() //  Ändern Prepend Bevor 
        {
            FieldNode newElement = new FieldNode(null, first);

            if (first == null)
            {
                last = newElement;
            }
            else
            {
                first.Previous = newElement;
            }

            first = newElement;
        }

       
        public void Append(int Anzahl)
        {
            for (int i = 0; i < Anzahl; i++)
            {
                FieldNode newElement = new FieldNode( last, null);

                if (last == null)
                {
                    first = newElement;
                }
                else
                {
                    last.Next = newElement;
                }

                last = newElement;
            }
           
        }
        

        

        

        

        public FieldNode InsertBevor(FieldNode previous, int Anzahl)
        {
            FieldNode newElement = new FieldNode( previous.Previous, previous);

            if (previous.Previous == null)
            {
                first = newElement;
            }
            else
            {
                previous.Previous.Next = newElement;
            }

            previous.Previous = newElement;

            if (Anzahl > 1)
            {
                return (InsertBevor(newElement, Anzahl - 1));
            }
            else
            {
                return newElement;
            }
            
        }

        
        

      

       

       
    }
}
