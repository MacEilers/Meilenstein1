
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Visualizer-Klasse für grafische Darstellung
        internal class BoardVisualizer
        {
            private GameField gameField;
            private int totalFields;
            private Dictionary<FieldNode, int> fieldPositions = new Dictionary<FieldNode, int>();

            internal BoardVisualizer(GameField field, int total)
            {
                gameField = field;
                totalFields = total;
            }

            private void BuildFieldPositions()
            {
                fieldPositions.Clear();
                FieldNode current = gameField.first;
                    int position = 1;

                while (current != null) 
                {
                    if (!fieldPositions.ContainsKey(current))
                    {
                        fieldPositions[current] = position;
                    }
                        
                    current = current.Next;
                    position++;
                }
            }

            internal int GetFieldPosition(FieldNode? node)
            {
                if (node == null || !fieldPositions.TryGetValue(node, out int pos))
                    return -1;
                return pos;
            }

            internal void DisplayBoard(Player[] players)
            {
                BuildFieldPositions(); // Aktualisiere Positionen vor dem Anzeigen
                
                int maxFields = fieldPositions.Count > 0 ? fieldPositions.Values.Max() : 100;
                int maxRows = (maxFields + 9) / 10;
                
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                       OOP-Meilenstein 1                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

                // Display status bar
                int pos1 = GetFieldPosition(players[0].Position);
                int pos2 = GetFieldPosition(players[1].Position);
                
                string pos1Text = (pos1 == -1) ? "Tunnel" : pos1.ToString(); // Falls Spieler in Tunnel? Position = Tunnel
                string pos2Text = (pos2 == -1) ? "Tunnel" : pos2.ToString();

                Console.WriteLine($"  {players[0].Name}: Feld {pos1Text}/{maxFields} (Würfe: {players[0].Throws}, Schritte: {players[0].Schritte})");
                Console.WriteLine($"  {players[1].Name}: Feld {pos2Text}/{maxFields} (Würfe: {players[1].Throws}, Schritte: {players[1].Schritte})");
                Console.WriteLine("\n  Legende: 🐪 Spieler 1 │ 🐘 Spieler 2 │ 🐍 Schlange │🪜  Leiter │ ❄️  Einfrieren │ 🔁 Spieler-Tausch | 🕳️  Tunnel\n");

                // Dynamisches Spielfeld:
                for (int row = maxRows; row >= 1; row--)
                {
                    Console.Write("  ");
                    for (int col = 1; col <= 10; col++)
                    {
                        int fieldNum = CalculateFieldNumber(row, col);
                        if (fieldNum<= maxFields)
                            DisplayFieldCell(fieldNum, players);
                        else
                            Console.Write("[  ]");
                    }
                    int rangeStart  =(row - 1) * 10 + 1;
                    int rangeEnd;
                    if(row *10 < maxFields)
                    {
                        rangeEnd = row*10;
                    }
                    else
                    {
                        rangeEnd = maxFields;
                    }
                        
                    
                    Console.WriteLine($"  [{rangeStart:D2}-{rangeEnd:D2}]");
                }

                Console.WriteLine();
            }

            private int CalculateFieldNumber(int row, int col)
            {
               
                if (row % 2 == 0)
                {
                    return row * 10 - col + 1;
                }
                else
                {
                    return (row - 1) * 10 + col;
                }
            }

            private void DisplayFieldCell(int fieldNum, Player[] players)
            {
                FieldNode? node = GetNodeByPosition(fieldNum);
                
                if (node == null)
                {
                    Console.Write("[  ]");
                    return;
                }

                string cellContent = $"{fieldNum:D2}";
                string displayChar;

                // wenn zwei auf einem Feld sind
                if (players[0].Position  == node && players[1].Position == node)
                    displayChar = "⚔ ";
                else if (players[0].Position == node)
                    displayChar = "🐪";
                else if (players[1].Position == node)
                    displayChar = "🐘";
                else if (node.Snake)
                    displayChar = "🐍";
                else if (node.Ladder)
                    displayChar = "🪜 ";
                else if (node.Freeze)
                    displayChar = "❄️ ";
                else if (node.HasLoop)
                    displayChar = "🕳️ ";
                else if (node.SwapPlayers)
                    displayChar = "🔁";
                else
                    displayChar = "  ";

                if (displayChar != " ")
                    Console.Write($"[{displayChar}]");
                else
                    Console.Write($"[{cellContent}]");
            }

            private FieldNode? GetNodeByPosition(int position)
            {
                // Finde den Knoten durch direkte Iteration
                FieldNode? current = gameField.first;
                int pos = 1;

                while (current != null && pos <= position)
                {
                    if (pos == position)
                        return current;
                    current = current.Next;
                    pos++;
                }

                return null;
            }
        }
         
        
        internal class FieldNode
        {
           
            internal bool Snake { get; set; }
            
            internal bool SwapPlayers { set; get; }
            internal bool Ladder {get; set; }
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

                        // Es wird eine LinkedList Erstelt von f geht zeiger in die Loop 
                        // Loop selbst zeigt auf f.prev und f.next
                        


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

            Player[] spieler ={new Player(n1,first!),new Player(n2,first!)};
            
            // Visualizer initialisieren
            BoardVisualizer visualizer = new BoardVisualizer(this, 100);
            visualizer.DisplayBoard(spieler);
            Console.WriteLine("\nSpiel startet! Drücke Enter...");
            Console.ReadLine();
            
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
                    InsertBevor(spieler[spielzug].Position!,5);
                
                
                spieler[spielzug].Position=Ziehen(spieler[spielzug].Position!,spieler[spielzug ].Position!,wurf) ;

                
                if (spieler[spielzug].Position != last)// Nach dem Würfeln am Ende 
                {
                   
                    SchlangenUndLeitern(spieler, spielzug);
                     
                    
                }
                
                // Board nach Zug anzeigen
                visualizer.DisplayBoard(spieler);
                Console.WriteLine("\nDrücke Enter für nächsten Zug...");
                Console.ReadLine();
                
                if (gleichesFeld(spieler))// Wenn gleiches Fled MiniGame wer stehen bleiben kann und wer zurück ziehen muss 
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Auf dem Feld steht schon jemand! Kämpfe um dein Leben!!");
                
                    System.Console.WriteLine($"{n1}: Zum würfeln Enter drücken...");
                    Console.ReadLine();
                    System.Console.WriteLine($"{n1} würfelt...");
                    int sp1 = GetCryptographicRandom(1,7);
                    System.Console.WriteLine($"{n1} hat eine {sp1} gewürfelt");

                    System.Console.WriteLine($"{n2}: Zum würfeln Enter drücken...");
                    Console.ReadLine();
                    System.Console.WriteLine($"{n2} würfelt...");
                    int sp2 = GetCryptographicRandom(1,7);
                    System.Console.WriteLine($"{n2} hat eine {sp2} gewürfelt");
                    
                    //Verlierer muss um die Differenz zurück ziehen 
                    int Schritaenderung = 0;
                    if (sp1 < sp2)
                    {
                       System.Console.WriteLine($"Spieler 2 hat gewonnen! Spieler 1 wird um {(sp2-sp1)} Felder zurückgeworfen!");
                       spieler[0].Position = ZurueckZiehen(spieler[0].Position, (sp2-sp1),ref Schritaenderung);
                       spieler[0].Schritte -= Schritaenderung;

                    } else if(sp2 < sp1)
                        {
                            System.Console.WriteLine($"Spieler 1 hat gewonnen! Spieler 2 wird um {(sp1-sp2)} Felder zurückgeworfen!");
                            spieler[1].Position = ZurueckZiehen(spieler[1].Position, (sp1-sp2),ref Schritaenderung);
                            spieler[1].Schritte -=  Schritaenderung;

                            
                        } else
                        {
                            System.Console.WriteLine("Unentschieden!");
                           
                            spieler[spielzug ].Position=ZurueckZiehen(spieler[spielzug ].Position,1, ref Schritaenderung) ;
                            spieler[spielzug ].Schritte -= Schritaenderung;
                        }
                    
                    // Board nach Kampf anzeigen
                    visualizer.DisplayBoard(spieler);
                    Console.ReadLine();

                    
                   
                }
                
                if (spieler[spielzug].Position == last)// Wenn Er durch Leiter aufs Letzte feld gekommen ist 
                {
                    visualizer.DisplayBoard(spieler);
                    Console.WriteLine($"\n{'═'*70}");
                    Console.WriteLine($"{spieler[spielzug].Name} hat nach {spieler[spielzug].Throws} Würfen mit {spieler[spielzug].Schritte} Schritten gewonnen!");
                    Console.WriteLine($"{spieler[(1+spielzug) % 2].Name} hat nach {spieler[(1+spielzug) % 2].Throws} Würfen mit {spieler[(1+spielzug) % 2].Schritte} Schritten verloren!");
                    Console.WriteLine($"{'═'*70}\n");
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
                spielzug = spielzug == 0 ? 1 : 0; // Spieler wechsel 
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


       private void SchlangenUndLeitern(Player[] spieler, int spielzug) // Am Anfang und Ende könen L/S sein allerdings werden diese nie ausgeführt um eine Null Pointer zu vermeiden 
        {
            // Wenn S oder L ausgeführt wird, löscht sich die L/S
           // Rekusiver aufruf von dem neuen Feld aus 
            
            if (spieler[spielzug ].Position.Ladder)
            {
                spieler[spielzug].Position.Ladder = false;
                FieldNode helper = Ziehen(spieler[spielzug ].Position,spieler[spielzug ].Position,3) ;
                if (spieler[spielzug ].Position == helper)
                    return; 
                spieler[spielzug ].Position = helper;// Leiter geht über des ende und wird deswegen nicht gegangen aber sonst rekusiv wieder ausgefürt -> fix Abbruch wenn nach gehen auf dems elben feld 
                Console.WriteLine($"        {spieler[spielzug ].Name} ist ein über eine Leiter 3 Felder weiter gegangen ");
                spieler[spielzug ].Schritte += 3;
                
                SchlangenUndLeitern(spieler, spielzug);
                
            }
            else if  (spieler[spielzug].Position.Snake)
            {
                int Schritaenderung = 0;
                spieler[spielzug].Position.Snake = false;
                spieler[spielzug ].Position=ZurueckZiehen(spieler[spielzug ].Position,3,ref Schritaenderung);
                spieler[spielzug ].Schritte -= Schritaenderung;
                Console.WriteLine($"        {spieler[spielzug].Name} ist ein über eine Schlange 3 Felder zurück gegangen ");
                SchlangenUndLeitern(spieler, spielzug);

            }
        }

        
        
       
        
        

        private FieldNode Ziehen(FieldNode start ,FieldNode f,int Anzahl)
        {
            if(f.Next == null&& f!= last) 
                throw new ArgumentNullException("Null Pointer");
            
            
           // Implementierung ziehen in Loops - Wenn Ziehen begint und das feld auf dem ich mich befinde ein Loop hat 

           if (start == f && start.HasLoop) // Ziehen hat auf der Loop begnonnen 
           {
               Console.WriteLine("      Es wurde eine Loop betreten");
               return Ziehen(start,f.LoopFirst, Anzahl - 1);
               
               
               
               
           }

            
            
             if (f != last)  {
                
                if (Anzahl > 1)
                {
                    return Ziehen(start,f.Next, Anzahl - 1); // Rekusiver aufruf
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
        private FieldNode ZurueckZiehen(FieldNode f,int Anzahl, ref int TotalFieldsMoved)
        {

            
            
            if (f != first )
            {
                 TotalFieldsMoved++;// Um für die ausgabe sagen zu können wie weit ich gegangen bin
                if (Anzahl > 1)
                {
                   
                    return ZurueckZiehen(f.Previous, Anzahl - 1,ref TotalFieldsMoved);
                }
                else
                {
                    return (f.Previous);
                }
                
                
            }
            else // Zieht immer zurück falls, falls wieder am start angekomen wrid bleibt er dort stehen 
            {
                return first;
            }
            
        }

        
        // Data fields:

        internal FieldNode? first = null;
        internal FieldNode? last = null;

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
