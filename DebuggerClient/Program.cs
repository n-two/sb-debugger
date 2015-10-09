using System;
using System.Collections;
using SmartBotAPI;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace DebuggerClient
{
    internal class Program
    {
        private static void SetUpSeed(ref Board _board)
        {
            
            AddToHand(ref _board, "GVG_095");
            AddToHand(ref _board, "GVG_096");
            AddToHand(ref _board, "GVG_092");

            AddToOwnBoard(ref _board, "AT_079", 6, 6);

            AddToOppBoard(ref _board, "EX1_534", 6, 5);


        }
        private static void Main(string[] args)
        {
            bool ENEMY = false; // isFriend is the bool, so enemy must be false.
            bool JUSTICAR = true;
            SetupEventHandlers();
            PipeClient.Connect();
            
            // make a empty board from base seed;
            Board _board = new Board();
            _board.IsOwnTurn = true; // can't sim if it's not your turn


            // define constants, modify as see fit
            _board.EnemyClass = Card.CClass.HUNTER;
            _board.FriendClass = Card.CClass.DRUID;
            _board.ManaAvailable = 10;
            _board.SecretEnemy = true;
            _board.SecretEnemyCount = 2; // TODO for profiles: if it's 5 secrects on a pally we kinda know what they are.


            SetUpHero(ref _board, _board.FriendClass);
            SetUpHero(ref _board, _board.EnemyClass, ENEMY, JUSTICAR);
            _board.HeroFriend.CurrentHealth = 30;
            _board.HeroFriend.CurrentArmor = 0;
            _board.HeroEnemy.CurrentHealth = 30;
            _board.HeroEnemy.CurrentArmor = 0;
            SetUpWeapons(ref _board, "EX1_536", 3, 2, true);
            SetUpSeed(ref _board);


            Console.WriteLine("Pipe connected : " + PipeClient.IsConnected());

            string SeedStr = _board.ToSeed();
            string ProfileStr = "Default"; // NOTE: make sure your profiles don't have weird characters in their names
            bool AutoConcede = false;


            SendSeedRequest(SeedStr, ProfileStr, AutoConcede);

            Console.ReadLine();

            PipeClient.Disconnect();
        }

        private static void SetUpHero(ref Board b, Card.CClass hc, bool isFriend = true, bool j = false) // j = justicartrueheart
        {
            Card c = new Card(), a = new Card();
            switch (hc)
            {
                case Card.CClass.DRUID:
                    c = Card.Create("HERO_06", isFriend, b.GetNextId());
                    if (j)
                    {
                        a = Card.Create("AT_132_DRUID", isFriend, b.GetNextId());
                        
                    }
                    else
                    {
                        a = Card.Create("CS2_017", isFriend, b.GetNextId());
                    }
                    break;
                case Card.CClass.HUNTER:
                    c = Card.Create("HERO_05", isFriend, b.GetNextId());
                    if (j)
                    {
                        a = Card.Create("CS2_102", isFriend, b.GetNextId());
                    }
                    else
                    {
                        a = Card.Create("CS2_102", isFriend, b.GetNextId());
                    }
                    break;
                case Card.CClass.MAGE:
                     c = Card.Create("HERO_08", isFriend, b.GetNextId());
                     if (j)
                     {
                         a = Card.Create("AT_132_MAGE", isFriend, b.GetNextId());
                     }
                     else
                     {
                         a = Card.Create("CS2_034", isFriend, b.GetNextId());
                     }
                     break;
                case Card.CClass.PALADIN:
                    c = Card.Create("HERO_04", isFriend, b.GetNextId());
                    if (j)
                    {

                    }
                    else
                    {

                    }
                     break;
                case Card.CClass.PRIEST:
                    c = Card.Create("HERO_09", isFriend, b.GetNextId());
                    if (j)
                    {

                    } else
                    {

                    }
                     break;
                case Card.CClass.ROGUE:
                    c = Card.Create("HERO_03", isFriend, b.GetNextId());
                    if (j)
                    {

                    } else
                    {

                    }
                     break;
                case Card.CClass.SHAMAN:
                    c = Card.Create("HERO_02", isFriend, b.GetNextId());
                    if (j)
                    {

                    } else
                    {
                        a = Card.Create("CS2_049", isFriend, b.GetNextId());
                    }
                     break;
                case Card.CClass.WARLOCK:
                    c = Card.Create("HERO_07", isFriend, b.GetNextId());
                    if (j)
                    {

                    }
                    else
                    {

                    }
                     break;
                case Card.CClass.WARRIOR:
                    c = Card.Create("HERO_01", isFriend, b.GetNextId());
                    if (j)
                    {

                    } else
                    {

                    }
                     break;
            }
            if (!isFriend)
            {
                b.HeroEnemy = c;
                b.EnemyAbility = a;
            }
            else
            {
                b.HeroFriend = c;
                b.Ability = a;
            }
        }

        private static void AddSecrect(ref Board b, string id)
        {
            b.Secret.Add(CardTemplate.StringToCard(id));
        } 

        private static void SetUpWeapons(ref Board b, string id, int pow, int dur, bool enemy = false)
        {
            if (enemy)
            {
                Card c = Card.Create(id, false, b.GetNextId());
                c.CurrentAtk = pow;
                c.CurrentDurability = dur;
                b.WeaponEnemy = c;
            }
            else 
            {
                Card c = Card.Create(id, true, b.GetNextId());
                c.CurrentAtk = pow;
                c.CurrentDurability = dur;
                b.WeaponFriend = c;
            }
        }

        private static void AddToHand(ref Board b, string id)
        {
            Card c = new Card();
            c = Card.Create(id, true, b.GetNextId());
            b.Hand.Add(c);

        }

        private static void AddToOwnBoard(ref Board b, string id, int pow, int dur)
        {
            Card c = new Card();
            c.InitInstance(CardTemplate.LoadFromId(id), true, b.GetNextId());
            c.CurrentAtk = pow;
            c.CurrentDurability = dur;
            b.MinionFriend.Add(c);
        }

        private static void AddToOppBoard(ref Board b, string id, int pow, int dur) 
        {
            Card c = new Card();
            c.InitInstance(CardTemplate.LoadFromId(id), false, b.GetNextId());
            c.CurrentAtk = pow;
            c.CurrentDurability = dur;
            b.MinionEnemy.Add(c);     
        }

        private static void SetupEventHandlers()
        {
            CommandHandler.SetupEvents();
            CommandHandler.OnCommandReceived += CommandHandlerOnOnCommandReceived;
        }

        private static void CommandHandlerOnOnCommandReceived(CommandHandler.CommandType command, string[] args)
        {
            switch (command)
            {
                case CommandHandler.CommandType.ActionList:
                    Console.WriteLine("Actions list : " + string.Join(Environment.NewLine, args));
                    break;

                case CommandHandler.CommandType.BoardAfterActions:
                    Console.WriteLine("BoardAfterActions : " + string.Join(Environment.NewLine, args));
                    break;

                case CommandHandler.CommandType.BoardBeforeActions:
                    Console.WriteLine("BoardBeforeActions : " + string.Join(Environment.NewLine, args));
                    break;

                case CommandHandler.CommandType.Log:
                    Console.WriteLine("Log : " + string.Join(Environment.NewLine, args));
                    break;
            }
        }

        private static void SendSeedRequest(string seed, string profile, bool aoespells)
        {
            CommandHandler.SendCommand(CommandHandler.CommandType.CalculationRequest,
                new[] {seed, profile, aoespells.ToString()});
        }
    }
}