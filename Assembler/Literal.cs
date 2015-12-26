using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Literal
    {
        private string name;
        private string value;
        private string address;

        public Literal(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get
            {
                if (name.StartsWith("x"))
                {
                    value = int.Parse(name.Substring(2, name.Length - 3), System.Globalization.NumberStyles.HexNumber).ToString("X6");
                }
                else if (name.StartsWith("c"))
                {
                    try
                    {
                        string buffer = name.Substring(2, name.Length - 3);
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            value = value + string.Format("{0:X}", Convert.ToInt32(buffer[i]));
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    value = int.Parse(name).ToString("X6");
                }
                return value;
            }
        }

        public int Length
        {
            get
            {
                if (name.StartsWith("c"))
                    return name.Length - 3;
                else if (name.StartsWith("x"))
                {
                    if(((name.Length - 3) % 2) != 0)
                        return ((name.Length - 3) + 1) / 2;
                    return (name.Length - 3) / 2;
                }
                return 3;
            }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
    }
}
