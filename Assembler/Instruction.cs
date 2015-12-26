using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Instruction
    {
        private string text;
        private string label = "";
        private string operation = "";
        private string operand = "";
        private string address;
        private string objectCode = "";
        private List<string> lErrors = new List<string>();
        private Dictionary<string, string> errors;

        public Instruction(string text)
        {
            this.text = text;
        }

        public Instruction(string text, Dictionary<string,string> errors)
        {
            this.text = text;
            this.errors = errors;
        }

        public Instruction(string label, string operation, string objectCode,string address)
        {
            this.text = "";
            this.label = label;
            this.operation = operation;
            this.objectCode = objectCode;
            this.address = address;
        }

        public string Label
        {
            get
            {
                if(label.Equals(""))
                {
                    if (text.Equals("") || text.Contains("\t"))
                        return "";
                    return text.Substring(0, 8).TrimEnd();
                }
                return label;
            }
        }

        public string Operation
        {
            get
            {
                if(operation.Equals(""))
                {
                    if (text.Equals("") || text.Contains("\t"))
                        return "";
                    return text.Substring(9, 6).TrimEnd();
                }
                return operation;
            }
        }

        public string Operand
        {
            get
            {
                if(operand.Equals(""))
                {
                    if (text.Equals("") || text.Contains("\t"))
                        return "";
                    return text.Substring(17, Math.Min(text.Length - 17, 18));
                }
                return operand;
            }
        }

        public string Comment
        {
            get
            {
                if(text.Length > 35)
                {
                    return text.Substring(35,text.Length-35);
                }
                return "";
            }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string ObjectCode
        {
            get
            {
                if (objectCode == null)
                    return "";
                return objectCode;
            }
            set { objectCode = value; }
        }

        public void CalcInstErrors()
        {
            if (this.Label.Contains(" "))
            {
                lErrors.Add(errors["labelFormat"]);
            }
            if (this.Operation.Equals(""))
            {
                lErrors.Add(errors["missOperation"]);
            }
            if (this.Operation.Contains(" "))
            {
                lErrors.Add(errors["operationFormat"]);
            }
            if (this.Operation.Equals("START", StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.Operand.Equals(""))
                {
                    lErrors.Add(errors["missStartOperand"]);
                }
                try
                {
                    int.Parse(this.Operand, NumberStyles.HexNumber);
                }
                catch (Exception e)
                {
                    lErrors.Add(errors["startOperand"]);
                }
            }
            if (this.Operation.Equals("BYTE",StringComparison.InvariantCultureIgnoreCase))
            {

            }
            if(this.Operation.Equals("RESW",StringComparison.InvariantCultureIgnoreCase))
            {
                if(this.Operand.Equals(""))
                {
                    lErrors.Add(errors["missReswOperand"]);
                }
                try
                {
                    int.Parse(this.Operand, NumberStyles.HexNumber);
                }
                catch (Exception e)
                {
                    lErrors.Add(errors["reswOperand"]);
                }

            }
            if (this.Operation.Equals("RESB", StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.Operand.Equals(""))
                {
                    lErrors.Add(errors["missResbOperand"]);
                }
                try
                {
                    int.Parse(this.Operand, NumberStyles.HexNumber);
                }
                catch (Exception e)
                {
                    lErrors.Add(errors["resbOperand"]);
                }
            }
            if(this.Operand.Equals("") && !this.label.Equals("*"))
            {
                lErrors.Add(errors["missOperand"]);
            }
        }

        public bool containError()
        {
            if (lErrors.Count == 0)
                return false;
            return true;
        }

        public void addError(string error)
        {
            lErrors.Add(errors[error]);
        }

        public List<string> Errors
        {
            get
            {
                return lErrors;
            }
        }

    }
}
