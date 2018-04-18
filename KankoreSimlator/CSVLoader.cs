using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace KankoreSimlator
{
    class CSVLoader
    {
        enum Data
        {
            ID = 0,
            Type = 3,
            Name = 4,
            HP = 16-1,
            Pow = 19-1,
            TP = 21-1,
            Def = 25-1,
            Avoid = 29-1,
            Luk = 32-1,
        }
        string[][] tmp;
        public CSVLoader()
        {
            StreamReader sr = new StreamReader(@"C:\Users\1718077\Desktop\ship.csv", Encoding.UTF8);
            var str = sr.ReadLine();
            List<string> list = new List<string>();
            while (true)
            {
                str = sr.ReadLine();
                if (str == null) break;
                list.Add(str);
            }

            var tmp = list.Select(x => x.Split(',').ToArray()).ToArray();
            this.tmp = tmp;
            sr.Close();
        }
        public Ship ConvertShip(S input, int No)
        {

            var rtn = new Ship();
            if (int.Parse(input.id) > 1500)
            {
                rtn.ID = int.Parse(input.id);
                rtn.Avoid = int.Parse(input.ev);
                rtn.Cond = 49;
                rtn.Def = int.Parse(input.ar);
                rtn.Luk = int.Parse(input.luk);
                rtn.LV = int.Parse(input.lvl);
                rtn.MaxHP = int.Parse(input.hp);
                rtn.Name = tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Name] + tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Name + 1];
                rtn.No = No;
                rtn.NowHP = int.Parse(input.hp);
                rtn.Pow = int.Parse(input.aa);
                rtn.Torp = int.Parse(input.tp);
                rtn.Type = tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Type];
            }
            else
            {
                rtn.ID = int.Parse(input.id);
                rtn.Avoid =int.Parse( tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Avoid]) ;
                rtn.Cond = 49;
                rtn.Def = int.Parse(tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Def]);
                rtn.Luk = int.Parse(tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Luk]); ;
                rtn.LV = int.Parse(input.lv);
                rtn.MaxHP = int.Parse(tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.HP]) ;
                rtn.Name = tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Name] + tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Name + 1];
                rtn.No = No;
                rtn.NowHP = int.Parse(tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.HP]); ;
                rtn.Pow = int.Parse(tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Pow]); ;
                rtn.Torp = int.Parse(tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.TP]);
                rtn.Type = tmp.First(x => x[(int)Data.ID] == input.id)[(int)Data.Type];

            }
            return rtn;
        }

        //    public Item ConvertItem(I input)
        //    {
        //        var rtn = new Item();
        //        rtn.
        //    }
    }
}

