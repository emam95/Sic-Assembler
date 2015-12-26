using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    class FirstPass
    {
        private Dictionary<String, String> symTab = new Dictionary<string, string>();
        private string startingAddress = "";
        private List<Instruction> program = new List<Instruction>();
        List<string> comments = new List<string>();
        private Dictionary<string, Literal> litTab = new Dictionary<string, Literal>();
        private List<Literal> tempLit = new List<Literal>();
        private string oldLOOCTR;

        public void begin()
        {
            int instRead = 0;
            bool first = true;
            int address = 0;
            string LOOCTR = "";
            StreamReader fileReader = new StreamReader(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\SRCFILE");
            string line = fileReader.ReadLine();
            while (line != null)
            {
                if (line == null)
                    break;
                //Console.WriteLine(line);
                if (line.StartsWith("."))
                {
                    //handle comment
                    comments.Add(instRead + line);
                    instRead++;
                    line = fileReader.ReadLine();
                    continue;
                }
                Instruction inst = new Instruction(line,Program.fillError());

                if (first)
                {
                    if (inst.Operation.Equals("START", StringComparison.InvariantCultureIgnoreCase))
                    {
                        startingAddress = inst.Operand;
                        oldLOOCTR = startingAddress;
                        address = int.Parse(startingAddress, System.Globalization.NumberStyles.HexNumber);
                        LOOCTR = address.ToString("X");
                        inst.Address = LOOCTR;
                    }
                    else
                    {
                        inst.addError("missStart");
                        startingAddress = "0";
                        oldLOOCTR = startingAddress;
                        address = int.Parse(startingAddress, System.Globalization.NumberStyles.HexNumber);
                        LOOCTR = address.ToString("X");
                        inst.Address = LOOCTR;
                    }
                    first = false;
                }
                else
                {
                    if(inst.Operation.Equals("ORG",StringComparison.CurrentCultureIgnoreCase))
                    {
                        if(inst.Operand.Trim().Equals(""))
                        {
                            LOOCTR = oldLOOCTR;
                            address = int.Parse(LOOCTR, System.Globalization.NumberStyles.HexNumber);
                            Instructions.Add(inst);
                            instRead++;
                            line = fileReader.ReadLine();
                            continue;
                        }
                        else
                        {
                            string expVal = evaluateExp(inst.Operand , LOOCTR);
                            string newAdd = "";
                            if(expVal.Equals("e"))
                            {
                                inst.addError("wrongOrigin");
                                newAdd = LOOCTR;
                            }
                            else
                                newAdd = expVal;
                            address += 3;
                            oldLOOCTR = address.ToString("X");
                            LOOCTR = newAdd;
                            address = int.Parse(LOOCTR, System.Globalization.NumberStyles.HexNumber);
                            Instructions.Add(inst);
                            instRead++;
                            line = fileReader.ReadLine();
                            continue;
                        }    
                    }
                    if (inst.Operand.StartsWith("="))
                    {
                        Literal lit = new Literal(inst.Operand.Substring(1));
                        if(!LitTab.ContainsKey(lit.Name))
                        {
                            litTab.Add(lit.Name, lit);
                            tempLit.Add(lit);
                        }
                    }
                    if (inst.Label.Equals(""))
                    {
                        if(inst.Operation.Equals("LTORG",StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            Instructions.Add(inst);
                            foreach(Literal l in tempLit)
                            {
                                l.Address = LOOCTR;
                                LitTab[l.Name].Address = LOOCTR;
                                Instructions.Add(new Instruction("*",l.Name,l.Value,l.Address));
                                address += l.Length;
                                LOOCTR = address.ToString("X");
                                instRead++;
                            }
                            tempLit.Clear();
                            line = fileReader.ReadLine();
                            continue;
                        }
                        else if (inst.Operation.Equals("END", StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            Instructions.Add(inst);
                            foreach (Literal l in tempLit)
                            {
                                l.Address = LOOCTR;
                                LitTab[l.Name].Address = LOOCTR;
                                Instructions.Add(new Instruction("*", l.Name, l.Value, l.Address));
                                address += l.Length;
                                LOOCTR = address.ToString("X");
                                instRead++;
                            }
                            return;
                        }
                        inst.Address = LOOCTR;
                        address += 3;
                        //address = int.Parse(LOOCTR, System.Globalization.NumberStyles.HexNumber);
                        LOOCTR = address.ToString("X");
                    }
                    else
                    {
                        if(SymTab.ContainsKey(inst.Label))
                        {
                            inst.addError("dupLabel");
                            program.Add(inst);
                            instRead++;
                            line = fileReader.ReadLine();
                            continue;
                        }
                        if (inst.Operation.Equals("RESW",StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            symTab.Add(inst.Label, LOOCTR);
                            try {
                                address += 3 * int.Parse(inst.Operand);
                                LOOCTR = address.ToString("X");
                            }catch(Exception e)
                            {

                            }
                        }
                        else if(inst.Operation.Equals("RESB", StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            symTab.Add(inst.Label, LOOCTR);
                            try {
                                address += int.Parse(inst.Operand);
                                LOOCTR = address.ToString("X");
                            }catch(Exception e)
                            {

                            }
                        }
                        else if (inst.Operation.Equals("BYTE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            symTab.Add(inst.Label, LOOCTR);
                            if(inst.Operand.StartsWith("x"))
                            {
                                address += ((inst.Operand.Length - 3)/2);
                                LOOCTR = address.ToString("X");
                            }
                            else
                            {
                                address += inst.Operand.Length - 3;
                                LOOCTR = address.ToString("X");
                            }
                        }
                        else if (inst.Operation.Equals("END", StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            symTab.Add(inst.Label, LOOCTR);
                            Instructions.Add(inst);
                            foreach (Literal l in tempLit)
                            {
                                l.Address = LOOCTR;
                                LitTab[l.Name].Address = LOOCTR;
                                Instructions.Add(new Instruction("*", l.Name, l.Value, l.Address));
                                address += l.Length;
                                LOOCTR = address.ToString("X");
                                instRead++;
                            }
                            return;
                        }
                        else if (inst.Operation.Equals("LTORG", StringComparison.InvariantCultureIgnoreCase))
                        {
                            inst.Address = LOOCTR;
                            symTab.Add(inst.Label, LOOCTR);
                            Instructions.Add(inst);
                            foreach (Literal l in tempLit)
                            {
                                l.Address = LOOCTR;
                                LitTab[l.Name].Address = LOOCTR;
                                Instructions.Add(new Instruction("*", l.Name, l.Value, l.Address));
                                address += l.Length;
                                LOOCTR = address.ToString("X");
                                instRead++;
                            }
                            tempLit.Clear();
                            line = fileReader.ReadLine();
                            continue;
                        }
                        else if(inst.Operation.Equals("EQU",StringComparison.InvariantCultureIgnoreCase))
                        {
                            if(!evaluateExp(inst.Operand).Equals("e"))
                            {
                                symTab.Add(inst.Label, evaluateExp(inst.Operand,LOOCTR));
                                program.Add(inst);
                                instRead++;
                                line = fileReader.ReadLine();
                                continue;
                            }
                            else
                            {
                                inst.addError("invalidExp");
                                program.Add(inst);
                                instRead++;
                                line = fileReader.ReadLine();
                                continue;
                            }
                        }
                        else
                        {
                            inst.Address = LOOCTR;
                            symTab.Add(inst.Label, LOOCTR);
                            address += 3;
                            //address = int.Parse(LOOCTR, System.Globalization.NumberStyles.HexNumber);
                            LOOCTR = address.ToString("X");
                        }
                    }
                }
                program.Add(inst);
                //Console.WriteLine(line);
                instRead++;
                line = fileReader.ReadLine();
            }
            fileReader.Close();
        }
         
        public List<Instruction> Instructions
        {
            get { return program; }
        }

        public Dictionary<string, string> SymTab
        {
            get { return symTab; }
        }

        public Dictionary<string, Literal> LitTab
        {
            get { return litTab; }
        }

        private string evaluateExp(string exp, string looctr = "")
        {
            if (exp.Trim().Equals("*"))
                return looctr;
            string value = "0";
            int x = 0;
            int nSymbols = 0;
            char operation = '+';
            bool first = true;
            string pattern = "([+-])";
            string[] subStrings = Regex.Split(exp, pattern);
            foreach (string match in subStrings)
            {
                if(match.Trim().Equals("+"))
                {
                    operation = '+';
                    continue;
                }
                if(match.Trim().Equals("-"))
                {
                    operation = '-';
                    continue;
                }
                try
                {
                    x = int.Parse(match.Trim(), System.Globalization.NumberStyles.HexNumber);
                }
                catch (Exception e)
                {
                    if (symTab.ContainsKey(match.Trim()))
                    {
                        x = int.Parse(symTab[match.Trim()], System.Globalization.NumberStyles.HexNumber);
                        if (operation == '+')
                            nSymbols++;
                        else
                            nSymbols--;
                    }
                    else
                    {
                        return "e";
                    }
                }
                if (first)
                {
                    value = x.ToString("X4");
                    first = false;
                }
                else
                    value = calculate(operation, int.Parse(value, System.Globalization.NumberStyles.HexNumber), x).ToString("X4");
            }
            if (nSymbols > 1)
                return "e";
            if (int.Parse(value, System.Globalization.NumberStyles.HexNumber) < 0)
                return "e";
            return value;
        }

        private int calculate(char c, int x , int y)
        {
            if (c == '+')
                return (x + y);
            if (c == '-')
                return (x - y);
            return -1;
        }

    }
}
