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

        private static Random rnd = new Random();
         
        internal class FieldNode
        {
           
            internal bool Snake { set; get; }
            internal bool Ladder { set; get; }
            internal bool Freeze {get; set;}
            internal bool HasLoop { get; set; }
            internal bool LoopElement { get; set; }
            
            
            internal FieldNode Next { get; set; }
            internal FieldNode Previous { get; set; }
            internal FieldNode LoopFirst { get; set; } = null;
            internal FieldNode LoopLast { get; set; } = null;
            
            
            public FieldNode( FieldNode previous, FieldNode next, bool canHaveLoops = true)
            {

                    LoopElement = !canHaveLoops;
                    int g =  rnd.Next(1, 7);
                    Snake = (1 == g);
                    Ladder = (2 == g);
                    Freeze = (4 == g);
                

                    HasLoop = ((3 == g) && canHaveLoops); // in loops kann es keine Loops geben 
                    if (HasLoop)
                    {
                        int n =  rnd.Next(3, 10);
                        FieldNode loopFirst = null;
                        FieldNode loopLast = null;
                        
                        CreateLoop(out loopFirst, out loopLast, n);
                        LoopLast = loopLast;
                        LoopFirst = loopFirst;
                        
                        LoopLast.Next = next;
                        LoopFirst.Previous = previous;


                    }
                   
                        
                       
                    
                   
                    
                    Next = next;
                    Previous = previous;

                    void CreateLoop (out FieldNode LoopFirst, out FieldNode LoopLast, int size)

                    {
                        LoopFirst = null;
                        LoopLast  = null;

                        for (int i = 0; i < size; i++)

                        {

                            FieldNode node = new FieldNode(null, null, false);

                            if (LoopFirst == null)
                                LoopFirst = node;
                            else
                            {
                                LoopLast.Next = node;

                                node.Previous = LoopLast;

                            }

                            LoopLast = node;

                        }

                       

                    }
            }
        }
        
        internal class Player{
            
            internal string Name;
            internal int Throws { get; set; } = 1;
            internal int Schritte  { get; set; } = 0;
            internal FieldNode? Position { get; set; } 
            internal bool IsFrozen {get;set;} = false;
            

            public Player(string name,FieldNode start)
            {
                this.Name = name;
                Position = start;

            }
        
        }
        
        
        public GameField(int Spielfeldgroese, bool canHaveLoops = true)
        {
            Append(Spielfeldgroese, canHaveLoops);
            
        }
        
        
         internal void Spielen(string n1, string n2)
        {
            
            int spielzug = 0;

            Player[] spieler ={new Player(n1,first),new Player(n2,first)};
            
            while (spieler[0].Position != last && spieler[1].Position != last)
            {
                //falls Spieler gefreezed, überspringe diesen Spielzug
                if(spieler[spielzug%2].IsFrozen) {
                    Console.WriteLine($"{spieler[spielzug%2].Name} ist eingefroren");
                    spieler[spielzug%2].IsFrozen = false;
                    spielzug++;
                    continue;
                }
                else
                {
                    
                
                int wurf = rnd.Next(1, 7);
                spieler[spielzug % 2].Schritte += wurf;

                Console.WriteLine($"{spieler[spielzug % 2].Name} hat eine {wurf} gewürfeld");
                if (wurf == 1)
                    Append(5);
                if (wurf == 6)
                    InsertBevor(spieler[spielzug % 2].Position,5);
                
                
                spieler[spielzug % 2].Position=Ziehen(spieler[spielzug % 2].Position,spieler[spielzug % 2].Position,wurf) ;

                
                if (spieler[spielzug % 2].Position != last)// Nach dem Würfeln am Ende 
                {
                   
                    Schlangen(spieler, spielzug);// Bewegt sich rekusiv über Schlangen zurück .
                    Leitern(spieler, spielzug);// Falls am ende auf einer Leiter landet Geht wieder leitern hoch 
                    if (spieler[spielzug%2].Position.Freeze) spieler[spielzug%2].IsFrozen = true; //Freezd Spieler wenn auf Freeze Feld am Ende
                }
                
                
                if (gleichesFeld(spieler))// Wenn gleiches Fled Gehe ein zurück
                {
                    spieler[spielzug % 2].Position=ZurueckZiehen(spieler[spielzug % 2].Position,1) ;
                    spieler[spielzug % 2].Schritte -= 1;
                   
                }
                
                if (spieler[spielzug % 2].Position == last)// Wenn Er durch Leiter aufs Letzte feld gekommen ist 
                {
                    Console.WriteLine($"{spieler[spielzug % 2].Name} hat nach {spieler[spielzug % 2].Throws} Würfen mit {spieler[spielzug % 2].Schritte} Schritten Gewonnen ");
                    Console.WriteLine($"{spieler[(1+spielzug) % 2].Name} hat nach {spieler[(1+spielzug) % 2].Throws} Würfen mit {spieler[(1+spielzug) % 2].Schritte} Schritten Verloren  ");

                    return;
                }
                }


                spieler[spielzug % 2].Throws += 1;

                spielzug++;
            }
            
            
        }

        private bool gleichesFeld(Player[] spieler)
        {
            return (spieler[0].Position == spieler[1].Position);
        }


       private void Leitern(Player[] spieler, int spielzug)
        {
            
            if (spieler[spielzug % 2].Position.Ladder)
            {
                FieldNode helper = Ziehen(spieler[spielzug % 2].Position,spieler[spielzug % 2].Position,3) ;
                if (spieler[spielzug % 2].Position == helper)
                    return; 
                spieler[spielzug % 2].Position = helper;// Leiter geht über des ende und wird deswegen nicht gegangen aber sonst rekusiv wieder ausgefürt -> fix Abbruch wenn nach gehen auf dems elben feld 
                Console.WriteLine($"{spieler[spielzug % 2].Name} ist ein über eine Leiter 3 Felder weiter gegeangen ");
                spieler[spielzug % 2].Schritte += 3;
                Leitern(spieler, spielzug);
                
            }
        }

        
        private void Schlangen(Player[] spieler, int spielzug)
        {
            
             if  (spieler[spielzug % 2].Position.Snake)
             {
                 //spieler[spielzug % 2].Position.Snake = false;
                 // Neue schlange 
                spieler[spielzug % 2].Position=ZurueckZiehen(spieler[spielzug % 2].Position,3);
                spieler[spielzug % 2].Schritte -= 3;
                Console.WriteLine($"{spieler[spielzug % 2].Name} ist ein über eine Schlange 3 Felder zurück gegeangen ");
                Schlangen(spieler, spielzug);

            }
            
            

        }
       
        
        

        private FieldNode Ziehen(FieldNode start ,FieldNode f,int Anzahl)
        {


           // Implementierung ziehen in Loops

           if (start == f && start.HasLoop) // Ziehen hat auf der Loop begnonnen 
           {
               return Ziehen(start,f.LoopFirst, Anzahl - 1);
               
               
               
           }

            
            
             if (f != last)  {
                
                if (Anzahl > 1)
                {
                    return Ziehen(start,f.Next, Anzahl - 1);
                }
                else
                {
                    return (f.Next);
                }
                
                
            }
            else
            {
                return ((Anzahl>1)?start:last);
            }
            
        }
        private FieldNode ZurueckZiehen(FieldNode f,int Anzahl)
        {
            
            
            
            if (f != first )
            {
                
                if (Anzahl > 1)
                {
                    return ZurueckZiehen(f.Previous, Anzahl - 1);
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

        
        // Data fields:

        FieldNode? first = null;
        FieldNode? last = null;

        // Read-only properties:

        private FieldNode? First
        {
            get { return first; }
        }

        private FieldNode? Last
        {
            get { return last; }
        }



        
       
        private void Append(int Anzahl, bool canHaveLoops = true)
        {
            for (int i = 0; i < Anzahl; i++)
            {
                FieldNode newElement = new FieldNode( last, null, canHaveLoops);

                if (last == null)
                {
                    first = newElement;
                    if (first.HasLoop)
                    {
                        first.LoopLast.Next = newElement;
                        first.LoopFirst.Previous = newElement;
                    }
                }
                else
                {
                    last.Next = newElement;
                    if (last.HasLoop)
                    {
                        last.LoopLast.Next = newElement;
                    }
                }
                
                last = newElement;
            }
            
           
        }

       
        

        private FieldNode InsertBevor(FieldNode previous, int Anzahl)
        {
            FieldNode newElement = new FieldNode( previous.Previous, previous);

            if (previous.Previous == null)
            {
                first = newElement;
                if (first.HasLoop)
                {
                    first.LoopFirst.Previous = first;
                }
            }
            else
            {
                previous.Previous.Next = newElement;

                if (previous.Previous.HasLoop)
                {
                    previous.Previous.LoopLast.Next = newElement;
                    
                }
                    
            }

            previous.Previous = newElement;

            if (Anzahl > 1)
            {
                return (InsertBevor(newElement, Anzahl - 1));
                
            }
            
                
            return newElement;
                
                
            
            
        }

        
        

      

       

       
    }
}
