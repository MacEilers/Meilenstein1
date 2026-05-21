
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
}
