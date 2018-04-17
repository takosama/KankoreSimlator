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
                Attack(buttle.MyFleet[i], Target);
            }

            void Attack(Ship Attacker, Ship Defender)
            {
                double Pow = 0;
                if (Attacker.Type == "正規空母" ||
                 Attacker.Type == "軽空母" ||
                 Attacker.Name.IndexOf("速吸") != -1)
                {
                    Pow = Attacker.GetPow + 4;
                }
                else
                {
                    Pow = Attacker.GetPow + Attacker.GetTorp + +(int)(1.3 * Attacker.GetBom) + 1;
                    Pow = (int)(1.5 * Pow) + 55;
                }
                double baseHitRate =
                           Cond.GetHitCorrection(Attacker.Cond) *
       Formation.HitCorrectionShell[(int)buttle.MyFormation] *
       (64 + Math.Sqrt(1.5 * Attacker.GetLuk) + 2 * Math.Sqrt(Attacker.LV) + Attacker.GetHit);

                double AvoidRate = 0;
                double BaseAvoidRate = (int)(Formation.AvoidCorrectionShell[(int)buttle.EnFormation] * (Defender.GetAvoid + Math.Sqrt(2 * Defender.GetAvoid)));
                if (BaseAvoidRate < 40)
                    AvoidRate = baseHitRate;
                else if (BaseAvoidRate < 65)
                    AvoidRate = (int)(40 + 3 * Math.Sqrt(baseHitRate - 40));
                else
                    AvoidRate = (int)(55 + 2 * Math.Sqrt(baseHitRate - 65));


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

                Pow *= WarFormRate;
                if (Pow > 150) Pow = 150 + Math.Sqrt(Pow - 150);
                int DMG = (int)(Pow * (CL == 2 ? 1.5 : 1) - (.7 * Defender.GetDef + .6 * MyRandom.random.Next(0, Defender.GetDef - 1)));

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
            System.IO.StreamReader sr = new System.IO.StreamReader(@"C:\Users\1718077\Desktop\test.txt");
            var str = sr.ReadToEnd();

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(str);

            var ship1 = json.f1.s1;
     //       GetShip(ship1);
        }

   //     public Ship GetShip(S input)
   //     {
   //
   //     }
    }


    public class Rootobject
    {
        public F1 f1 { get; set; }
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

    }


    class Ship
    {
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



        public int Avoid;
        public int GetAvoid => Avoid + this.items.Select(x => x.Avoid).Sum();


        public string Name;
        public string Type;
        public Item[] items;
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
