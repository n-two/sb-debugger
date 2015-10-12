using System;
using System.Collections;
using System.Collections.Generic;
using SmartBotAPI;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace DebuggerClient
{
    internal class Program
    {
        static int _globalValue;

        static Dictionary<string, string> _cardnames = new Dictionary<string, string>();
        public static Dictionary<string, string> CardNames
        {
            get { return _cardnames; }
            set { _cardnames = value; }
        }
        private static void MakeCardNames()
        {
            Dictionary<string, string> _dic = new Dictionary<string, string>();
            foreach (Card.Cards _card in Enum.GetValues(typeof(Card.Cards)))
            {
                CardTemplate temp = CardTemplate.LoadFromId(_card);
                if (temp == null) continue;
                if (CardNames.ContainsKey(temp.Name)) continue; 
                CardNames.Add(temp.Name, _card.ToString());
            }
        }

        public static int GlobalCounter
        {
            get
            {
                return ++_globalValue;
            }
        }


        private static void SetUpSeed(ref Board _board)
        {
            
            AddToHand(ref _board, "Mysterious Challenger");
            AddToHand(ref _board, "Quartermaster");
            AddToHand(ref _board, "Shielded Minibot");

            AddToOwnBoard(ref _board, "Knife Juggler", 6, 6);

            AddToOppBoard(ref _board, "Piloted Shredder", 6, 5);

            AddSecrect(ref _board, "Competitive Spirit");

            SetUpWeapons(ref _board, "Coghammer", 3, 2, true);
            


        }
        private static void Main(string[] args)
        {
            bool ENEMY = false; // isFriend is the bool, so enemy must be false.
            bool JUSTICAR = true;
            MakeCardNames();
            SetupEventHandlers();
            PipeClient.Connect();

            // make a empty board from base seed;
            Board _board = Board.FromSeed("10~5~37~0~0~20~19~2~True~7~True~8~EX1_536*0*0*3*3*0*2*37*2*0*0*2*0*False*False*False*False*False*False*False*False*False*False*False*False*False*0*False~0~HERO_06*0*0*0*0*0*30*37*30*0*0*30*0*False*False*False*False*False*False*False*False*False*False*False*False*False*0*False~HERO_06*0*0*0*0*0*30*37*30*0*0*30*0*False*False*False*False*False*False*False*False*False*False*False*False*False*0*False~CS2_017*0*0*0*2*0*0*37*0*0*0*0*0*False*False*False*False*False*False*False*False*False*False*False*False*False*0*False~AT_132_DRUID*0*0*0*2*0*0*37*0*0*0*0*0*False*False*False*False*False*False*False*False*False*False*False*False*False*0*False~0~0~0~0~0~0~0~0~True=True=False~0~0~0~False~AT_029,EX1_011,CS2_233,EX1_124,EX1_581,EX1_131,EX1_162,GVG_023,GVG_006,GVG_103,EX1_129,EX1_556,EX1_412,EX1_134,AT_100,GVG_091,AT_114,GVG_078,CS2_179,EX1_023,EX1_043,BRM_008,AT_090,AT_028,CS2_155,CS2_222,AT_103~");
            // Board _board = new Board();
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
            SetUpSeed(ref _board);


            Card test = Card.Create(CardTemplate.StringToCard("CS2_179"), true, GlobalCounter);

            Console.WriteLine(test.Template.Name);


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
                    c = Card.Create("HERO_06", isFriend, GlobalCounter);
                    if (j)
                    {
                        a = Card.Create("AT_132_DRUID", isFriend, GlobalCounter);
                        
                    }
                    else
                    {
                        a = Card.Create("CS2_017", isFriend, GlobalCounter);
                    }
                    break;
                case Card.CClass.HUNTER:
                    c = Card.Create("HERO_05", isFriend, GlobalCounter);
                    if (j)
                    {
                        a = Card.Create("CS2_102", isFriend, GlobalCounter);
                    }
                    else
                    {
                        a = Card.Create("CS2_102", isFriend, GlobalCounter);
                    }
                    break;
                case Card.CClass.MAGE:
                     c = Card.Create("HERO_08", isFriend, GlobalCounter);
                     if (j)
                     {
                         a = Card.Create("AT_132_MAGE", isFriend, GlobalCounter);
                     }
                     else
                     {
                         a = Card.Create("CS2_034", isFriend, GlobalCounter);
                     }
                     break;
                case Card.CClass.PALADIN:
                    c = Card.Create("HERO_04", isFriend, GlobalCounter);
                    if (j)
                    {

                    }
                    else
                    {

                    }
                     break;
                case Card.CClass.PRIEST:
                    c = Card.Create("HERO_09", isFriend, GlobalCounter);
                    if (j)
                    {

                    } else
                    {

                    }
                     break;
                case Card.CClass.ROGUE:
                    c = Card.Create("HERO_03", isFriend, GlobalCounter);
                    if (j)
                    {

                    } else
                    {

                    }
                     break;
                case Card.CClass.SHAMAN:
                    c = Card.Create("HERO_02", isFriend, GlobalCounter);
                    if (j)
                    {

                    } else
                    {
                        a = Card.Create("CS2_049", isFriend, GlobalCounter);
                    }
                     break;
                case Card.CClass.WARLOCK:
                    c = Card.Create("HERO_07", isFriend, GlobalCounter);
                    if (j)
                    {

                    }
                    else
                    {

                    }
                     break;
                case Card.CClass.WARRIOR:
                    c = Card.Create("HERO_01", isFriend, GlobalCounter);
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

        private static void AddSecrect(ref Board b, string name)
        {
            string id = GetCardIdByName(name);
            b.Secret.Add(CardTemplate.StringToCard(id));
        } 

        private static void SetUpWeapons(ref Board b, string name, int pow, int dur, bool enemy = false)
        {
            string id = GetCardIdByName(name);
            if (enemy)
            {
                Card c = Card.Create(id, false, GlobalCounter);
                c.CurrentAtk = pow;
                c.CurrentDurability = dur;
                b.WeaponEnemy = c;
            }
            else 
            {
                Card c = Card.Create(id, true, GlobalCounter);
                c.CurrentAtk = pow;
                c.CurrentDurability = dur;
                b.WeaponFriend = c;
            }
        }

        private static void AddToHand(ref Board b, string name)
        {
            Card c = new Card();
            string id = GetCardIdByName(name);
            c = Card.Create(id, true, GlobalCounter);
            b.Hand.Add(c);

        }

        private static void AddToOwnBoard(ref Board b, string name, int pow, int dur)
        {
            Card c = new Card();
            string id = GetCardIdByName(name);
            c = Card.Create(id, true, GlobalCounter);
            c.CurrentAtk = pow;
            c.CurrentDurability = dur;
            b.MinionFriend.Add(c);
        }

        private static void AddToOppBoard(ref Board b, string name, int pow, int dur) 
        {
            Card c = new Card();
            string id = GetCardIdByName(name);
            c = Card.Create(id, false, GlobalCounter);
            c.CurrentAtk = pow;
            c.CurrentDurability = dur;
            b.MinionEnemy.Add(c);     
        }

        private static string GetCardIdByName(string name)
        {
            string id;
            if(CardNames.TryGetValue(name, out id))
            {
                return id;
            } else
            {
                Console.WriteLine("No Card with such name found, you probably misspelled it.");
                return null; // throw err
            }
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
               //     Console.WriteLine("Log : " + string.Join(Environment.NewLine, args));
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