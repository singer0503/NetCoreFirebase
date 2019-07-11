using System;

namespace Gambling.Models
{
    public class PokeViewModel
    {
        public string person1 { get; set; }
        public string person2 { get; set; }
        public string person3 { get; set; }
        public string person4 { get; set; }

        public Person[] person;
        public string[] data;
    }

    //限定四種花色 梅花,方塊,愛心,黑桃
    public enum Suit { Clubs, Diamonds, Hearts, Spades }

    //限定牌面為 1~13
    public enum Value { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

    public class Poker
    {
        public Suit suit;
        public Value value;
        public Poker(Suit i, Value v)
        {
            suit = i;
            value = v;
        }
    }

    public class Person
    {
        public Person() { }
        public Poker[] perPoker;
        public int count;
    }
}