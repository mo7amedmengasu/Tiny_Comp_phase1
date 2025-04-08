using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
public enum Token_Class
{
    Int, Float, String, Read, Write, Main,
    Repeat, Until, If, Elseif, Else, Then, Return, Endl, End, Dot,
    semicolon, Comma, LParanthesis, RParanthesis,
    EqualOp, NotEqualOp, LessThanOp, GreaterThanOp, AndOp, OrOp,
    PlusOp, MinusOp, MultiplyOp, DivideOp, AssignOp, Idenifier, Number, Comment, LCurlyBraces, RCurlyBraces, constant, StringLiteral
}
namespace Tiny_Comp_phase1
{
    public class Token
    {
        public string lex ="";
        public Token_Class token_type;
    }
    class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Symbols = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);


            Symbols.Add(".", Token_Class.Dot);
            Symbols.Add(";", Token_Class.semicolon);
            Symbols.Add(",", Token_Class.Comma);
            Symbols.Add("(", Token_Class.LParanthesis);
            Symbols.Add(")", Token_Class.RParanthesis);
            Symbols.Add("{", Token_Class.LCurlyBraces);
            Symbols.Add("}", Token_Class.RCurlyBraces);


            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add(":=", Token_Class.AssignOp);
        }

        public void StartScanning(string SourceCode)
        {
         
            Tokens.Clear();
            Errors.Error_List.Clear();
            int last_index = -1;

            for (int i = 0; i < SourceCode.Length; i++)
            {
                char current_char = SourceCode[i];
                string current_lex = current_char.ToString();
                int next_index = i + 1;

                if (current_char == ' ' || current_char == '\n' || current_char == '\t' || current_char == '\r')
                {
                    continue;
                }

                if (char.IsLetterOrDigit(current_char))
                {
                    while (next_index < SourceCode.Length && SourceCode[next_index] != ' ')
                    {
                        current_lex += SourceCode[next_index];
                        next_index++;
                    }
                }
                else if (current_char == '/')
                {
                    if (next_index < SourceCode.Length && SourceCode[next_index] == '*')
                    {
                        current_lex += SourceCode[next_index];
                        next_index++;
                        while (next_index < SourceCode.Length - 1 && !(SourceCode[next_index] == '*' && SourceCode[next_index + 1] == '/'))
                        {
                            current_lex += SourceCode[next_index];
                            next_index++;
                        }
                        if (next_index < SourceCode.Length - 1)
                        {
                            current_lex += "*/";
                            next_index += 2;
                        }
                    }
                }
                else if (current_char == '"')
                {
                    while (next_index < SourceCode.Length && SourceCode[next_index] != '"')
                    {
                        current_lex += SourceCode[next_index];
                        next_index++;
                    }
                    if (next_index < SourceCode.Length)
                    {
                        current_lex += '"';
                        next_index++;
                    }
                }
                else
                {
                    if (next_index < SourceCode.Length)
                    {
                        char next_char = SourceCode[next_index];
                        if ((current_char == '&' && next_char == '&') ||
                            (current_char == '|' && next_char == '|') ||
                            (current_char == '<' && next_char == '>') ||
                            (current_char == ':' && next_char == '='))
                        {
                            current_lex += next_char;
                            next_index++;
                        }
                    }

   
                }

             
                  FindTokenClass(current_lex);
                  i = next_index - 1;
                  last_index = next_index;
                
            }

   

            Tiny_Comp_phase1.TokenStream = Tokens;
        }


        void FindTokenClass(string Lex)
        {
            if(Lex == null)
            {
                return;
            }
            Token token = new Token();
            if (ReservedWords.ContainsKey(Lex))
            {
                token.lex = Lex;
                token.token_type = ReservedWords[Lex];
                Tokens.Add(token);
            }
            else if (Operators.ContainsKey(Lex))
            {
                token.lex = Lex;
                token.token_type = Operators[Lex];
                Tokens.Add(token);
            }
            else if (Symbols.ContainsKey(Lex))
            {
                token.lex = Lex;
                token.token_type = Symbols[Lex];
                Tokens.Add(token);
            }
            else if (IsComment(Lex))
            {
                token.lex = Lex;
                token.token_type = Token_Class.Comment;
                Tokens.Add(token);
            }
            else if (isConstant(Lex))
            {
                token.lex = Lex;
                token.token_type = Token_Class.constant;
                Tokens.Add(token);
            }
            else if (isStringLiteral(Lex))
            {
                token.lex = Lex;
                token.token_type = Token_Class.StringLiteral;
                Tokens.Add(token);
            }
            else if (IsIdentifier(Lex))
            {
                token.lex = Lex;
                token.token_type = Token_Class.Idenifier;
                Tokens.Add(token);
            }
            else
            {
                Errors.Error_List.Add("Invalid Token: " + Lex);
            }

        }
        public bool IsIdentifier(string lex)
        {
            Regex reg = new Regex(@"^([a-zA-Z])([0-9a-zA-Z])*$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }

        bool isConstant(string lex)
        {
            bool isDecimal = false;
            int i = 0;

            // Check if the lexeme is a valid integer or float
            while (i < lex.Length && isDigit(lex[i]))
            {
                i++;
            }

            if (i < lex.Length && lex[i] == '.')
            {
                isDecimal = true;
                i++;
            }

            while (i < lex.Length && isDigit(lex[i]))
            {
                i++;
            }

            // If we have reached the end of the lexeme, it is a valid constant
            return i == lex.Length && (isDecimal || lex.Length > 0);
        }
        bool isStringLiteral(string lex)
        {
            bool isValid;
            int len = lex.Length;
            if ((lex[0] == '"' && lex[len - 1] == '"')) {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
                return isValid;
        }

        public bool IsComment(string lex)
        {
            return (lex.Length >= 4 && lex.StartsWith("/*") && lex.EndsWith("*/"));
        }


        public bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

    }
}
