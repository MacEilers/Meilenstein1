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
            public int Throws { get; set; } = 1;
            public int Schritte  { get; set; } = 0;
            public FieldNode? Position { get; set; } 

            public Player(string name,FieldNode start)
            {
                this.Name = name;
                Position = start;

            }
        
        }

        public bool gleichesFeld(Player[] spieler)
        {
            return (spieler[0].Position == spieler[1].Position);
        }


        public void Leitern(Player[] spieler, int spielzug)
        {
            
            if (spieler[spielzug % 2].Position.Ladder) 
            {
                spieler[spielzug % 2].Position=Ziehen(spieler[spielzug % 2].Position,3) ;
                Console.WriteLine($"{spieler[spielzug % 2].Name} ist ein über eine Leiter 3 Felder weiter gegeangen ");
                spieler[spielzug % 2].Schritte += 3;
                Leitern(spieler, spielzug);
                
            }
        }
        public void Schlangen(Player[] spieler, int spielzug)
        {
            
             if  (spieler[spielzug % 2].Position.Snake)
            {
                spieler[spielzug % 2].Position=ZurueckZiehen(spieler[spielzug % 2].Position,3);
                spieler[spielzug % 2].Schritte -= 3;
                Console.WriteLine($"{spieler[spielzug % 2].Name} ist ein über eine Schlange 3 Felder zurück gegeangen ");
                Schlangen(spieler, spielzug);

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
                spieler[spielzug % 2].Schritte += wurf;

                Console.WriteLine($"{spieler[spielzug % 2].Name} hat eine {wurf} gewürfeld");
                if (wurf == 1)
                {
                    Append(5);
                }
                else if (wurf == 6)
                {
                    InsertBevor(spieler[0].Position,5);
                }
                
                
                spieler[spielzug % 2].Position=Ziehen(spieler[spielzug % 2].Position,wurf) ;

                
                if (spieler[spielzug % 2].Position == last)// Nach dem Würfeln am Ende 
                {
                    Console.WriteLine($"{spieler[spielzug % 2].Name} hat nach {spieler[spielzug % 2].Throws} Würfen mit {spieler[spielzug % 2].Schritte} Schritten Gewonnen ");
                    return;
                }

                Schlangen(spieler, spielzug);// Bewegt sich rekusiv über Schlangen zurück .
                Leitern(spieler, spielzug);// Falls am ende auf einer Leiter landet Geht wieder leitern hoch 
                
                
                if (gleichesFeld(spieler))// Wenn gleiches Fled Gehe ein zurück
                {
                    spieler[spielzug % 2].Position=ZurueckZiehen(spieler[spielzug % 2].Position,1) ;
                    Console.WriteLine($"{spieler[spielzug % 2].Name} hat nach {spieler[spielzug % 2].Throws} Würfen mit {spieler[spielzug % 2].Schritte} Schritten Gewonnen ");
                   spieler[spielzug % 2].Schritte -= 1;
                   
                }
                
                if (spieler[spielzug % 2].Position == last)// Wenn Er durch Leiter aufs Letzte feld gekommen ist 
                {
                    return;
                }


                spieler[spielzug % 2].Throws += 1;

                spielzug++;
            }
            
            
            
        }

        

        public FieldNode Ziehen(FieldNode f,int Anzahl)
        {
            if (f != last) // Wenn am Ende Bleibt er einfach stehen (3 vor ende 5 Gewürfelt -> Gewonnen )??
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
