using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KankoreSimlator
{
    class MyRandom
    {
        readonly public static Random random = new Random();
    }
    class Program
    {
        static void Main(string[] args)
        {
            Buttle b = new Buttle();
            b.Load();
        }
    }

    //戦艦のみとする　空母も戦艦式で計算
    class ShellSupport
    {
        double[] WarFormPowerCorrection = new double[] { 1.0, .8, .7, .6 };
        public ShellSupport(Buttle buttle)
        {
            for (int i = 0; i < buttle.MyFleet.Length; i++)
            {
                int TargetNo = MyRandom.random.Next(0, 5);
                var Target = buttle.EnFleet[TargetNo];
                double WarFormRate = 0;
                int r = MyRandom.random.Next(0, 99);
                if (WarForm.Rate[0] <= r)
                    WarFormRate = WarForm.PowCorrection[0];
                else if (WarForm.Rate[1] <= r)
                    WarFormRate = WarForm.PowCorrection[1];

                else if (WarForm.Rate[2] <= r)
                    WarFormRate = WarForm.PowCorrection[2];
                else
                    WarFormRate = WarForm.PowCorrection[3];
                Attack(buttle.MyFleet[i], Target,WarFormRate);

            }

            void Attack(Ship Attacker, Ship Defender,double WarFormRate)
            {
                double Pow = 0;
                if (Attacker.Type == "正規空母" ||
                 Attacker.Type == "軽空母" ||
                 Attacker.Name.IndexOf("速吸") != -1)
                {
                    Pow = Attacker.GetPow + Attacker.GetTorp + +(int)(1.3 * Attacker.GetBom) + 1;
                    Pow = (int)(1.5 * Pow) + 55;
                }
                else
                {
                    Pow = Attacker.GetPow + 4;

                }
                double baseHitRate =
                           Cond.GetHitCorrection(Attacker.Cond) *
       Formation.HitCorrectionShell[(int)buttle.MyFormation] *
       (64 + Math.Sqrt(1.5 * Attacker.GetLuk) + 2 * Math.Sqrt(Attacker.LV) + Attacker.GetHit);

                double AvoidRate = 0;
                double BaseAvoidRate = (int)(Formation.AvoidCorrectionShell[(int)buttle.EnFormation] * (Defender.GetAvoid + Math.Sqrt(2 * Defender.Luk)));
                if (BaseAvoidRate < 40)
                    AvoidRate = BaseAvoidRate;
                else if (BaseAvoidRate < 65)
                    AvoidRate = (int)(40 + 3 * Math.Sqrt(BaseAvoidRate - 40));
                else
                    AvoidRate = (int)(55 + 2 * Math.Sqrt(BaseAvoidRate - 65));


                double HitRate = Cond.GetAvoidCorrection(Defender.Cond) * (baseHitRate - AvoidRate);
                if (HitRate < 10) HitRate = 10;
                if (HitRate > 97) HitRate = 97;

                double A = MyRandom.random.Next(0, 99);
                double B = Math.Sqrt(HitRate) * 1.0;

                int CL = 0;
                if (A <= B)
                    CL = 2;
                else if (A > HitRate)
                    CL = 0;
                else
                    CL = 1;


            
                Pow *= WarFormRate;
                if (Pow > 150) Pow = 150 + Math.Sqrt(Pow - 150);
                double P = Pow * (CL == 2 ? 1.5 : 1);
                var def = (.7 * Defender.GetDef + .6 * MyRandom.random.Next(0, Defender.GetDef - 1));
                int DMG = (int)(P -def);
                if(Defender.NowHP>=0)
                if (DMG<=0)
                {
                    int r0 = MyRandom.random.Next(0, Defender.NowHP);
                    DMG = (int)(.06 * Defender.NowHP +  .08*r0 );

                }

                if (CL != 0)
                    Defender.NowHP -= DMG;
            }
        }
    }
    //xxx
    class Cond
    {
        static public double GetHitCorrection(int Cond)
        {
            if (Cond > 49)
                return 1.2;
            else
                return 1;
        }
        static public double GetAvoidCorrection(int Cond)
        {
            if (Cond > 49)
                return .7;
            else
                return 1;
        }
    }

    class Buttle
    {
        public Formation.Type MyFormation;
        public Formation.Type EnFormation;
        public Ship[] MyFleet;
        public Ship[] EnFleet;

        public void Load()
        {
            int max = 0;
            int sum = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(@"C:\Users\1718077\Desktop\test.txt");
            System.IO.StreamReader sr2 = new System.IO.StreamReader(@"C:\Users\1718077\Desktop\test2.txt");
            var str = sr.ReadToEnd();
            var str2 = sr2.ReadToEnd();

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(str);
            var json2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(str2);

            CSVLoader csv = new CSVLoader();
            List<Ship> ships = new List<Ship>();
            ships.Add(csv.ConvertShip(json.f2.s1, 0));
            ships.Add(csv.ConvertShip(json.f2.s2, 1));
            ships.Add(csv.ConvertShip(json.f2.s3, 2));
            ships.Add(csv.ConvertShip(json.f2.s4, 3));
            ships.Add(csv.ConvertShip(json.f2.s5, 4));
            ships.Add(csv.ConvertShip(json.f2.s6, 5));



            CSVLoader csv2 = new CSVLoader();
            List<Ship> ships2 = new List<Ship>();
            ships2.Add(csv2.ConvertShip(json2.f1.s1, 0));
            ships2.Add(csv2.ConvertShip(json2.f1.s2, 1));
            ships2.Add(csv2.ConvertShip(json2.f1.s3, 2));
            ships2.Add(csv2.ConvertShip(json2.f1.s4, 3));
            ships2.Add(csv2.ConvertShip(json2.f1.s5, 4));
            ships2.Add(csv2.ConvertShip(json2.f1.s6, 5));

            for (int j = 1; j < 100000000; j++)
            {


                Buttle b = new Buttle();
                b.MyFleet = ships.ToArray();
                b.EnFleet = ships2.ToArray();
                for (int i = 0; i < 6; i++)
                {
                    b.EnFleet[i].NowHP = b.EnFleet[i].MaxHP;
                    b.MyFleet[i].NowHP = b.MyFleet[i].MaxHP;
                }


                ShellSupport s = new ShellSupport(b);

                int cnt = 0;
                for (int i = 0; i < b.EnFleet.Length; i++)
                {
                    Console.Write((b.EnFleet[i].NowHP < 0 ? "d " : "k ") +b.EnFleet[i].Name+" " + b.EnFleet[i].MaxHP + "->" + b.EnFleet[i].NowHP + "         ");
                    if (b.EnFleet[i].NowHP < 0) cnt++;
                }
                if (cnt > max) max = cnt;
                sum += cnt;
                Console.WriteLine(cnt + " ave=" + 1.0 * sum / j + " max=" + max);

            }
        }
    }
 
    public class Rootobject
    {
        public F1 f1 { get; set; }
        public F1 f2 { get; set; }
        public F1 f3 { get; set; }
        public F1 f4 { get; set; }
        public F1 f5 { get; set; }
        public F1 f6 { get; set; }
    }

    public class F1
    {
        public string form { get; set; }
        public S s1 { get; set; }
         public S s2 { get; set; }
         public S s3 { get; set; }
         public S s4 { get; set; }
         public S s5 { get; set; }
         public S s6 { get; set; }
    }

    public class S
    {
        public string id { get; set; }
        public string lvl { get; set; }
        public string lv { get; set; }
        public string hp { get; set; }
        public string fp { get; set; }
        public string tp { get; set; }
        public string aa { get; set; }
        public string ar { get; set; }
        public string ev { get; set; }
        public string asw { get; set; }
        public string los { get; set; }
        public string luk { get; set; }
        public string rng { get; set; }
        public string spd { get; set; }
        public string tacc { get; set; }
        public Items items { get; set; }
        public string morale { get; set; }
        public string fuel { get; set; }
        public string ammo { get; set; }
    }

    public class Items
    {
        public I  i1 { get; set; }
        public I  i2 { get; set; }
        public I  i3 { get; set; }
        public I  i4 { get; set; }
        public I  i5 { get; set; }
    }

    public class I
    {
        public string id { get; set; }
        public string num { get; set; }
    }
    

    class Kabai
    {

    }

    class Item
    {
        public int Pow;
        public int Torp;
        public int Def;
        public int Avoid;
        public int Luk;
        public int Hit;
        public int Bom;
        public string Name;
        public int ID;
    }


    class Ship
    {
       public Ship()
        {
            for (int i = 0; i < 4; i++)
                this.items[i] = new Item();
        }
        public int Pow;
        public int GetPow => Pow + this.items.Select(x => x.Pow).Sum();
        public int Cond;
        public int No;
        public int MaxHP;
        public int NowHP;

        public int Def;
        public int GetDef => Def + this.items.Select(x => x.Def).Sum();

        public int LV;
        public int Luk;
        public int GetLuk => Luk + this.items.Select(x => x.Luk).Sum();

        public int Torp;
        public int GetTorp => Torp + this.items.Select(x => x.Torp).Sum();

        public int GetBom => this.items.Select(x => x.Bom).Sum();
        public int GetHit => this.items.Select(x => x.Hit).Sum();

        public int ID;

        public int Avoid;
        public int GetAvoid => Avoid + this.items.Select(x => x.Avoid).Sum();


        public string Name;
        public string Type;
        public Item[] items = new Item[4];
    }

    class Formation
    {
        public enum Type
        {
            Vectical,
            Multiple,
            Cercle,
            Diagnal,
            side
        }
        public static readonly double[] PowCorrectionShell = new double[] { 1, .8, .7, .6, .6 };
        public static readonly double[] HitCorrectionShell = new double[] { 1, 1.2, 1, 1, 1 };
        public static readonly double[] AvoidCorrectionShell = new double[] { 1, 1, 1.1, 1.2, 1.3 };

    }
    class WarForm
    {
        enum Type
        {
            TAdvance,
            Same,
            Anti,
            TDisAdvance,
        }

        public static readonly double[] PowCorrection = new double[] { 1.2, 1, .8, .6 };
        public static readonly int[] Rate = new int[] { 15, 60, 90, 100 };
        public static readonly int[] RateSaiun = new int[] { 15, 60, 100, 100 };
    }

}
