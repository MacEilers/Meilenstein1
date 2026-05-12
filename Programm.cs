namespace ConsoleApp3;
using System;
using System.Security.Cryptography;

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

        public static int GetCryptographicRandom(int min, int max)
        {
            return RandomNumberGenerator.GetInt32(min, max);

        }
         
        internal class FieldNode
        {
           
            internal bool Snake { get; }
            
            internal bool SwapPlayers { set; get; }
            internal bool Ladder {get; }
            internal bool Freeze {get;}
            internal bool HasLoop { get;  }
            internal bool LoopElement { get; set; }
            
            
            internal FieldNode Next { get; set; }
            internal FieldNode Previous { get; set; }
            internal FieldNode LoopFirst { get;  } 
            internal FieldNode LoopLast { get; } 
            
            
            
            public FieldNode( FieldNode previous, FieldNode next, bool canHaveLoops = true)
            {

                    LoopElement = !canHaveLoops;
                    int g =  GetCryptographicRandom(1, 10);
                    SwapPlayers=  (5==GetCryptographicRandom(1,30));
                    Snake = (1 == g);
                    Ladder = (2 == g);
                    Freeze = (4 == g);
                

                    HasLoop = ((3 == g) && canHaveLoops); // in loops kann es keine Loops geben 
                    if (HasLoop)
                    {
                        int n =  GetCryptographicRandom(3, 10);
                        FieldNode loopFirst ;
                        FieldNode loopLast ;
                        
                        CreateLoop(out loopFirst, out loopLast, n);
                        LoopLast = loopLast;
                        LoopFirst = loopFirst;
                        
                        LoopLast.Next = next;
                        LoopFirst.Previous = previous;


                    }
                   
                        
                       
                    
                   
                    
                    Next = next;
                    Previous = previous;

                    

                       

                    }
            internal void CreateLoop (out FieldNode LoopFirst, out FieldNode LoopLast, int size)

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
                if(spieler[spielzug].IsFrozen) {
                    Console.WriteLine($"{spieler[spielzug].Name} ist eingefroren");
                    spieler[spielzug].IsFrozen = false;
                    
                }
                else
                {
                    
                
                int wurf = GetCryptographicRandom(1, 7);
                spieler[spielzug].Schritte += wurf;

                Console.WriteLine($"{spieler[spielzug].Name} hat eine {wurf} gewürfelt");
                if (wurf == 1)
                    Append(5);
                if (wurf == 6)
                    InsertBevor(spieler[spielzug].Position,5);
                
                
                spieler[spielzug].Position=Ziehen(spieler[spielzug].Position,spieler[spielzug ].Position,wurf) ;

                
                if (spieler[spielzug].Position != last)// Nach dem Würfeln am Ende 
                {
                   
                    Schlangen(spieler, spielzug);// Bewegt sich rekusiv über Schlangen zurück .
                    Leitern(spieler, spielzug);// Falls am ende auf einer Leiter landet Geht wieder leitern hoch 
                    
                }
                
                
                if (gleichesFeld(spieler))// Wenn gleiches Fled Gehe ein zurück
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Auf dem Feld steht schon jemand! Kämpfe um dein Leben!!");
                
                    System.Console.WriteLine("Spieler 1: Zum würfeln Enter drücken...");
                    Console.ReadLine();
                    System.Console.WriteLine("Spieler 1 würfelt...");
                    int sp1 = GetCryptographicRandom(1,7);
                    System.Console.WriteLine($"Spieler 1 hat eine {sp1} gewürfelt");

                    System.Console.WriteLine("Spieler 2: Zum würfeln Enter drücken...");
                    Console.ReadLine();
                    System.Console.WriteLine("Spieler 2 würfelt...");
                    int sp2 = GetCryptographicRandom(1,7);
                    System.Console.WriteLine($"Spieler 2 hat eine {sp2} gewürfelt");
                    System.Console.WriteLine("");
                    
                    if (sp1 < sp2)
                    {
                       System.Console.WriteLine($"Spieler 2 hat gewonnen! Spieler 1 wird um {(sp2-sp1)} Felder zurückgeworfen!");
                       spieler[0].Position = ZurueckZiehen(spieler[0].Position, (sp2-sp1));
                       spieler[0].Schritte -= (sp2-sp1);

                    } else if(sp2 < sp1)
                        {
                            System.Console.WriteLine($"Spieler 1 hat gewonnen! Spieler 2 wird um {(sp1-sp2)} Felder zurückgeworfen!");
                            spieler[1].Position = ZurueckZiehen(spieler[1].Position, (sp1-sp2));
                            spieler[1].Schritte -= (sp1-sp2);

                        } else
                        {
                            System.Console.WriteLine("Unentschieden!");
                            spieler[spielzug ].Position=ZurueckZiehen(spieler[spielzug ].Position,1) ;
                            spieler[spielzug ].Schritte -= 1;
                        }


                    
                   
                }
                
                if (spieler[spielzug].Position == last)// Wenn Er durch Leiter aufs Letzte feld gekommen ist 
                {
                    Console.WriteLine($"{spieler[spielzug].Name} hat nach {spieler[spielzug].Throws} Würfen mit {spieler[spielzug].Schritte} Schritten gewonnen ");
                    Console.WriteLine($"{spieler[(1+spielzug) % 2].Name} hat nach {spieler[(1+spielzug) % 2].Throws} Würfen mit {spieler[(1+spielzug) % 2].Schritte} Schritten verloren  ");

                    return;
                }

                if (spieler[spielzug].Position.Freeze)
                {
                    spieler[spielzug].IsFrozen = true;
                }
                      


                if (spieler[spielzug].Position.SwapPlayers)
                {
                    spieler[spielzug].Position.SwapPlayers = false;// Swap Felder sind nur einmalig Nutzbar 
                    FieldNode h = spieler[spielzug].Position;
                    spieler[spielzug].Position = spieler[(spielzug + 1) % 2].Position;
                    spieler[(spielzug + 1) % 2].Position = h;
                    
                    
                }
                    
                
                }
                

                spieler[spielzug].Throws += 1;
                spielzug = spielzug == 0 ? 1 : 0;
              /*  if (spielzug == 0)
                {
                    spielzug = 1;
                }
                else
                    spielzug = 0;
        */

            }
            
            
        }

        private bool gleichesFeld(Player[] spieler)
        {
            return (spieler[0].Position == spieler[1].Position);
        }


       private void Leitern(Player[] spieler, int spielzug)
        {
            
            if (spieler[spielzug ].Position.Ladder)
            {
                FieldNode helper = Ziehen(spieler[spielzug ].Position,spieler[spielzug ].Position,3) ;
                if (spieler[spielzug ].Position == helper)
                    return; 
                spieler[spielzug ].Position = helper;// Leiter geht über des ende und wird deswegen nicht gegangen aber sonst rekusiv wieder ausgefürt -> fix Abbruch wenn nach gehen auf dems elben feld 
                Console.WriteLine($"{spieler[spielzug ].Name} ist ein über eine Leiter 3 Felder weiter gegangen ");
                spieler[spielzug ].Schritte += 3;
                Leitern(spieler, spielzug);
                
            }
        }

        
        private void Schlangen(Player[] spieler, int spielzug)
        {
            
             if  (spieler[spielzug].Position.Snake)
             {
                
                spieler[spielzug ].Position=ZurueckZiehen(spieler[spielzug ].Position,3);
                spieler[spielzug ].Schritte -= 3;
                Console.WriteLine($"{spieler[spielzug].Name} ist ein über eine Schlange 3 Felder zurück gegangen ");
                Schlangen(spieler, spielzug);

            }
            
            

        }
       
        
        

        private FieldNode Ziehen(FieldNode start ,FieldNode f,int Anzahl)
        {


           // Implementierung ziehen in Loops

           if (start == f && start.HasLoop) // Ziehen hat auf der Loop begnonnen 
           {
               Console.WriteLine("Es wurde eine Loop betreten");
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
