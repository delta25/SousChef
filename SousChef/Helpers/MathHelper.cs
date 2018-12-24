using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SousChef.Helpers
{
    public static class MathHelper
    {
        public static double ExecuteRPNFormulaWithVariables(string formula, KeyValuePair<string, double>[] variables)
        {
            return ExecuteRPN(ConvertFormulaToStack(PopulateFormulaVariables(formula, variables)));
        }

        private static string PopulateFormulaVariables(string formula, KeyValuePair<string, double>[] variables)
        {
            foreach(var variable in variables)
            {
                string stringval = variable.Value.ToString();
                formula = formula.Replace(variable.Key, stringval);
            }
            return formula;
        }

        private static Stack<string> ConvertFormulaToStack(string formula)
        {
            char[] sp = new char[] { ' ', '\t' };
            return new Stack<string>(formula.Split(sp, StringSplitOptions.RemoveEmptyEntries));
        }

        private static double ExecuteRPN(Stack<string> tks)
        {
            string tk = tks.Pop();
            double x, y;
            if (!double.TryParse(tk, out x))
            {
                y = ExecuteRPN(tks); x = ExecuteRPN(tks);
                if (tk == "+") x += y;
                else if (tk == "-") x -= y;
                else if (tk == "*") x *= y;
                else if (tk == "/") x /= y;
                else throw new Exception();
            }
            return x;
        }
    }
}
