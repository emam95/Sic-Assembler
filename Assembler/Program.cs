using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            FirstPass pass = new FirstPass();
            pass.begin();
            SecondPass pass2 = new SecondPass(pass.Instructions, pass.SymTab,pass.LitTab);
            pass2.calculateObjCode();
            pass2.writeListFile();
            pass2.writeObjectFile();
            Console.ReadKey();
        }

        public static Dictionary<string,string> fillError()
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            errors.Add("labelFormat", "illegal format in label field");
            errors.Add("missOperation", "missing operation code");
            errors.Add("operationFormat", "illegal format in operation field");
            errors.Add("missStartOperand", "missing or misplaced operand in start statement");
            errors.Add("startOperand", "illegal operand in start statement");
            errors.Add("byteOperand", "illegal operand in byte statement");
            errors.Add("byteXOdd", "odd length hex string in byte statement");
            errors.Add("missByteOperand", "missing or misplaced operand in byte statement");
            errors.Add("missReswOperand", "missing or misplaced operand in resw statement");
            errors.Add("reswOperand", "illegal operand in resw statement");
            errors.Add("missResbOperand", "missing or misplaced operand in resb statement");
            errors.Add("resbOperand", "illegal operand in resb statement");
            errors.Add("dupLabel", "duplicate label definition");
            errors.Add("manySymb", "too many symbols in source program");
            errors.Add("dupStart", "duplicate or misplaced start statement");
            errors.Add("missStart", "missing or misplaced start statement");
            errors.Add("wordOperand", "illegal operand in word statement");
            errors.Add("missWordOperand", "missing or misplaced operand in word statement");
            errors.Add("missEndOperand", "missing or misplaced operand in end statement");
            errors.Add("endOperand", "illegal operand in end statement");
            errors.Add("undefSymb", "undefined symbol in operand");
            errors.Add("afterEnd", "statement should not follow end statement");
            errors.Add("illegalOperand", "illegal operand field");
            errors.Add("unrecOpCode", "unrecognized operation code");
            errors.Add("missOperand", "missing or misplaced operand in instruction");
            errors.Add("wrongOrigin", "origin value is not identified");
            errors.Add("invalidExp", "invalid expression");
            return errors;
        }
    }
}
