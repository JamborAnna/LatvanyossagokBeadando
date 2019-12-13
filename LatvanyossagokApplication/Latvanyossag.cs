using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatvanyossagokApplication
{
    class Latvanyossag
    {
        int id;
        string nev;
        string leiras;
        int ar;
        int varos_id;

        public Latvanyossag(int id, string nev, string leiras, int ar, int varos_id)
        {
            this.id = id;
            this.nev = nev;
            this.leiras = leiras;
            this.ar = ar;
            this.varos_id = varos_id;
        }

        public int Id { get => id; set => id = value; }
        public string Nev { get => nev; set => nev = value; }
        public string Leiras { get => leiras; set => leiras = value; }
        public int Ar { get => ar; set => ar = value; }
        public int Varos_id { get => varos_id; set => varos_id = value; }

        public override string ToString()
        {
            return nev;
        }

    }
}
