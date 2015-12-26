using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class SecondPass
    {
        List<Instruction> instructions;
        Dictionary<string, string> symTab;
        Dictionary<string, string> opTab;
        Dictionary<string, Literal> litTab;

        public SecondPass(List<Instruction> instructions, Dictionary<string,string> symTab , Dictionary<string, Literal> litTab)
        {
            this.instructions = instructions;
            this.symTab = symTab;
            this.litTab = litTab;

            opTab = new Dictionary<string, string>();
            opTab.Add("ADD", "18");
            opTab.Add("AND","40");
            opTab.Add("COMP", "28");
            opTab.Add("DIV", "24");
            opTab.Add("J", "3C");
            opTab.Add("JEQ", "30");
            opTab.Add("JGT", "34");
            opTab.Add("JLT", "38");
            opTab.Add("JSUB", "48");
            opTab.Add("LDA", "00");
            opTab.Add("LDCH", "50");
            opTab.Add("LDL", "08");
            opTab.Add("LDX", "04");
            opTab.Add("MUL", "20");
            opTab.Add("OR", "44");
            opTab.Add("RD", "D8");
            opTab.Add("RSUB", "4C");
            opTab.Add("STA", "0C");
            opTab.Add("STCH", "54");
            opTab.Add("STL", "14");
            opTab.Add("STX", "10");
            opTab.Add("SUB", "1C");
            opTab.Add("TD", "E0");
            opTab.Add("TIX", "2C");
            opTab.Add("WD", "DC");
        }

        public void calculateObjCode()
        {
            foreach(Instruction inst in instructions)
            {
                if(opTab.ContainsKey(inst.Operation.ToUpper()))
                {
                    string opcode = opTab[inst.Operation.ToUpper()];
                    string address = "";
                    if (inst.Operand.EndsWith(",x"))
                    {
                        address = symTab[inst.Operand.Substring(0,inst.Operand.Length - 2)];
                        address = (int.Parse(address, System.Globalization.NumberStyles.HexNumber) + 32768).ToString("X");
                    }
                    else if(inst.Operand.StartsWith("="))
                    {
                        address = litTab[inst.Operand.Substring(1)].Address;
                    }
                    else
                    {
                        address = symTab[inst.Operand];
                    }
                    inst.ObjectCode = opcode + address;
                    if(inst.Operation.Equals("RSUB",StringComparison.InvariantCultureIgnoreCase))
                    {
                        inst.ObjectCode = "4C0000";
                    }
                }
                else if(inst.Operation.Equals("WORD",StringComparison.InvariantCultureIgnoreCase))
                {
                    try {
                        inst.ObjectCode = int.Parse(inst.Operand).ToString("X6");
                    }
                    catch(Exception e)
                    {

                    }
                }
                else if(inst.Operation.Equals("BYTE",StringComparison.InvariantCultureIgnoreCase))
                {
                    if(inst.Operand.StartsWith("x"))
                    {
                        inst.ObjectCode = int.Parse(inst.Operand.Substring(2, inst.Operand.Length - 3),System.Globalization.NumberStyles.HexNumber).ToString("X6");
                    }
                    else
                    {
                        try {
                            string buffer = inst.Operand.Substring(2, inst.Operand.Length - 3);
                        for(int i = 0; i < buffer.Length; i++)
                        {
                            inst.ObjectCode = inst.ObjectCode + string.Format("{0:X}",Convert.ToInt32(buffer[i]));
                        }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                else if(inst.Label.Equals("*"))
                {

                }
                else if(inst.Operation.Equals("EQU",StringComparison.InvariantCultureIgnoreCase))
                {

                }
                else if(!inst.Operation.Equals("START", StringComparison.InvariantCultureIgnoreCase) && !inst.Operation.Equals("END", StringComparison.InvariantCultureIgnoreCase) && !inst.Operation.Equals("RESW", StringComparison.InvariantCultureIgnoreCase) && !inst.Operation.Equals("RESB", StringComparison.InvariantCultureIgnoreCase) && !inst.Operation.Equals("LTORG",StringComparison.InvariantCultureIgnoreCase) && !inst.Operation.Equals("ORG",StringComparison.InvariantCultureIgnoreCase) && !inst.Operation.Equals("EQU",StringComparison.InvariantCultureIgnoreCase))
                {
                    inst.addError("unrecOpCode");
                }
            }
        }

        public void writeListFile()
        {
            if(!File.Exists(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\LISFILE"))
            {
                File.Create(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\LISFILE");
            }
            StreamWriter listWriter = new StreamWriter(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\LISFILE");
                foreach (Instruction inst in instructions)
            {
                string line = string.Format("{0,-4} {1,-6} {2,-8} {3,-6} {4}", inst.Address, inst.ObjectCode, inst.Label, inst.Operation, inst.Operand);
                listWriter.WriteLine(line);
                inst.CalcInstErrors();
                if(inst.containError())
                {
                    for( int n = 0; n < inst.Errors.Count; n++)
                    {
                        listWriter.WriteLine(inst.Errors[n]);
                    }
                }
            }
            listWriter.Close();
        }

        public void writeObjectFile()
        {
            if (!File.Exists(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\OBJFILE"))
            {
                File.Create(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\OBJFILE");
            }
            StreamWriter listWriter = new StreamWriter(@"E:\Programming\Software\C#\Assembler\Assembler\Assembler\OBJFILE");

            foreach(Instruction inst in instructions)
            {
                if (inst.containError())
                    return;
            }

            string name = instructions[0].Label;
            listWriter.WriteLine(string.Format("H{0,-6}",name) + int.Parse(instructions[0].Operand,System.Globalization.NumberStyles.HexNumber).ToString("X6") + (int.Parse(instructions[instructions.Count - 1].Address, System.Globalization.NumberStyles.HexNumber) - int.Parse(instructions[0].Operand, System.Globalization.NumberStyles.HexNumber)).ToString("X6"));
            int lines = 0;
            List<int> sizes = new List<int>();
            int size = 0;
            for (int i = 1; i <  instructions.Count; i++)
            {
                if ((((size%10 == 0) && size != 0) || instructions[i].ObjectCode.Equals("")))
                {
                    lines++;
                    sizes.Add(size);
                    if (!instructions[i].ObjectCode.Equals(""))
                        size = 1;
                    else
                        size = 0;
                    continue;
                }
                size += 1;
            }

            int l = 1;
            string line = "";

            for(int j = 0; j < lines; j++)
            {
                if (l > instructions.Count - 1)
                    break;
                if (sizes[j] == 0)
                {
                    l++;
                    continue;
                }
                for(int k = 0; k < sizes[j]; k++)
                {
                    if (l > instructions.Count - 1)
                        break;
                    while (instructions[l].ObjectCode.Equals(""))
                        l++;
                    if (k == 0)
                    {
                        line = string.Format("T{0,-6}{1,-2}", int.Parse(instructions[l].Address, System.Globalization.NumberStyles.HexNumber).ToString("X6"), (sizes[j] * 3).ToString("X2"));
                    }
                    line += string.Format("{0,-6}",instructions[l].ObjectCode);
                    l++;
                }
                listWriter.WriteLine(line);
            }

            listWriter.WriteLine("E" + int.Parse(instructions[0].Operand, System.Globalization.NumberStyles.HexNumber).ToString("X6"));

            listWriter.Close();
        }


    }
}
