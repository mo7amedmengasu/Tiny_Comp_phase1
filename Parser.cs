using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiny_Comp_phase1
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        private int TokenIndex = 0;
        List<Token> TokenStream = new List<Token>();
        public Node root = new Node("Default Root");
        private Boolean MainFunctionExecuted = false;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Function_List_Opt());

            if (!MainFunctionExecuted)
                Errors.Error_List.Add("The code misses the main function !!!");

            return root;
        }

        private Node Return_Statement()
        {
            Node node = new Node("Return_Statement");
            node.Children.Add(Match(Token_Class.Return));
            node.Children.Add(Expression());
            node.Children.Add(Match(Token_Class.semicolon));
            return node;
        }

        private Node Function_Call()
        {
            Node node = new Node("Function_Call");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(Argument_List_Opt());
            return node;
        }

        private Node Argument_List()
        {
            Node node = new Node("Argument_List");
            node.Children.Add(Expression());
            node.Children.Add(Arg_Tail());
            return node;
        }

        private Node? Arg_Tail()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
                Node node = new Node("Arg_Tail");
                node.Children.Add(Match(Token_Class.Comma));
                node.Children.Add(Expression());
                node.Children.Add(Arg_Tail());
                return node;
            }
            return null;
        }

        private Node Argument_List_Opt()
        {
            Node node = new Node("Argument_List_Opt");
            node.Children.Add(Match(Token_Class.LParanthesis));
            if (IsvalidToken(Token_Class.Idenifier) || IsvalidToken(Token_Class.constant) ||
                IsvalidToken(Token_Class.StringLiteral) || IsvalidToken(Token_Class.LParanthesis))
            {
                node.Children.Add(Argument_List());
            }
            node.Children.Add(Match(Token_Class.RParanthesis));
            return node;
        }

        private Node Condition()
        {
            Node node = new Node("Condition");
            if (IsvalidToken(Token_Class.MinusOp))
            {
                node.Children.Add(Match(Token_Class.MinusOp)); // Match the negative sign
            }
            node.Children.Add(Expression());
            node.Children.Add(Condition_Operator());
            if (IsvalidToken(Token_Class.MinusOp))
            {
                node.Children.Add(Match(Token_Class.MinusOp)); // Match the negative sign
            }
            node.Children.Add(Expression());
            return node;
        }

        private Node Condition_Operator()
        {
            Node node = new Node("Condition_Operator");
            if (IsvalidToken(Token_Class.EqualOp))
                node.Children.Add(Match(Token_Class.EqualOp));
            else if (IsvalidToken(Token_Class.GreaterThanOp))
                node.Children.Add(Match(Token_Class.GreaterThanOp));
            else if (IsvalidToken(Token_Class.LessThanOp))
                node.Children.Add(Match(Token_Class.LessThanOp));
            else
                node.Children.Add(Match(Token_Class.NotEqualOp));
            return node;
        }
        private Node Declaration_Statement()
        {
            Node node = new Node("Declaration_Statement");
            node.Children.Add(Datatype());
            node.Children.Add(Declaration_Identifiers());
            node.Children.Add(Match(Token_Class.semicolon));
            return node;
        }


        private Node Declaration_Identifiers()
        {
            Node node = new Node("Declaration_Identifiers");
            node.Children.Add(Decl_Item());
            node.Children.Add(Decl_Tail());
            return node;
        }


        private Node? Decl_Tail()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
                Node node = new Node("Decl_Tail");
                node.Children.Add(Match(Token_Class.Comma));
                node.Children.Add(Decl_Item());
                node.Children.Add(Decl_Tail());
                return node;
            }
            return null;
        }
        private Node Decl_Item()
        {
            Node node = new Node("Decl_Item");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(Decl_Assign_Opt());
            return node;
        }

        private Node? Decl_Assign_Opt()
        {
            if (IsvalidToken(Token_Class.AssignOp))
            {
                Node node = new Node("Decl_Assign_Opt");
                node.Children.Add(Match(Token_Class.AssignOp));
                node.Children.Add(Expression());
                return node;
            }
            return null;
        }
        private Node Assignment_Statement()
        {
            Node node = new Node("Assignment_Statement");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(Match(Token_Class.AssignOp));
            node.Children.Add(Expression());
            node.Children.Add(Match(Token_Class.semicolon));
            return node;
        }

        private Node AddOp()
        {
            Node node = new Node("AddOp");
            if (IsvalidToken(Token_Class.PlusOp))
                node.Children.Add(Match(Token_Class.PlusOp));
            else
                node.Children.Add(Match(Token_Class.MinusOp));
            return node;
        }
        private Node MultOp()
        {
            Node node = new Node("MultOp");
            if (IsvalidToken(Token_Class.MultiplyOp))
                node.Children.Add(Match(Token_Class.MultiplyOp));
            else
                node.Children.Add(Match(Token_Class.DivideOp));
            return node;
        }

        private Node Datatype()
        {
            Node node = new Node("Datatype");
            if (IsvalidToken(Token_Class.Int))
                node.Children.Add(Match(Token_Class.Int));
            else if (IsvalidToken(Token_Class.Float))
                node.Children.Add(Match(Token_Class.Float));
            else if (IsvalidToken(Token_Class.String))
                node.Children.Add(Match(Token_Class.String));
            return node;
        }

        private Node Parameter()
        {
            Node node = new Node("Parameter");
            node.Children.Add(Datatype());
            node.Children.Add(Match(Token_Class.Idenifier));
            return node;
        }

        private Node Param_Tail()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
                Node node = new Node("Param_Tail");
                node.Children.Add(Match(Token_Class.Comma));
                node.Children.Add(Parameter());
                node.Children.Add(Param_Tail());
                return node;
            }
            return null;
        }

        private Node Parameter_List()
        {
            Node node = new Node("Parameter_List");
            node.Children.Add(Parameter());
            node.Children.Add(Param_Tail());
            return node;
        }

        private Node Parameter_List_Opt()
        {
            Node node = new Node("Parameter_List_Opt");
            if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                node.Children.Add(Parameter_List());
            }
            return node;
        }

        private Node Main_Function()
        {
            Node node = new Node("Main_Function");
            if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                node.Children.Add(Datatype());
            }
            node.Children.Add(Match(Token_Class.Main));
            node.Children.Add(Match(Token_Class.LParanthesis));
            node.Children.Add(Match(Token_Class.RParanthesis));
            node.Children.Add(Function_Body());
            MainFunctionExecuted = true;
            return node;
        }

        private Node Function_Declaration()
        {
            Node node = new Node("Function_Declaration");
            node.Children.Add(Datatype());
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(Match(Token_Class.LParanthesis));
            node.Children.Add(Parameter_List_Opt());
            node.Children.Add(Match(Token_Class.RParanthesis));
            return node;
        }

        private Node Boolean_Operator()
        {
            Node node = new Node("Boolean_Operator");
            if (IsvalidToken(Token_Class.AndOp))
                node.Children.Add(Match(Token_Class.AndOp));
            else
                node.Children.Add(Match(Token_Class.OrOp));
            return node;
        }

        private Node Cond_Tail()
        {
            if (IsvalidToken(Token_Class.AndOp) || IsvalidToken(Token_Class.OrOp))
            {
                Node node = new Node("Cond_Tail");
                node.Children.Add(Boolean_Operator());
                node.Children.Add(Condition());
                node.Children.Add(Cond_Tail());
                return node;
            }
            return null;
        }

        private Node Condition_Statement()
        {
            Node node = new Node("Condition_Statement");
            node.Children.Add(Condition());
            node.Children.Add(Cond_Tail());
            return node;
        }

        private Node Optional_Else_Ifs()
        {
            if (IsvalidToken(Token_Class.Elseif))
            {
                Node node = new Node("Optional_Else_Ifs");
                node.Children.Add(Match(Token_Class.Elseif));
                node.Children.Add(Condition_Statement());
                node.Children.Add(Match(Token_Class.Then));
                node.Children.Add(Statement_List());
                node.Children.Add(Optional_Else_Ifs());
                return node;
            }
            return null;
        }

        private Node Optional_Else()
        {
            if (IsvalidToken(Token_Class.Else))
            {
                Node node = new Node("Optional_Else");
                node.Children.Add(Match(Token_Class.Else));
                node.Children.Add(Statement_List());
                return node;
            }
            return null;
        }

        private Node If_Statement()
        {
            Node node = new Node("If_Statement");
            node.Children.Add(Match(Token_Class.If)); // Match 'if'
            node.Children.Add(Condition()); // Parse the condition directly without parentheses
            node.Children.Add(Match(Token_Class.Then)); // Match 'then'
            node.Children.Add(Statement_List()); // Parse the statements inside the 'if' block
            node.Children.Add(Optional_Else_Ifs()); // Parse optional 'elseif' blocks
            node.Children.Add(Optional_Else()); // Parse optional 'else' block
            node.Children.Add(Match(Token_Class.End)); // Match 'end'
            return node;
        }

        private Node Repeat_Statement()
        {
            Node node = new Node("Repeat_Statement");
            node.Children.Add(Match(Token_Class.Repeat)); // Match 'repeat'
            node.Children.Add(Statement_List()); // Parse the statements inside the 'repeat' block
            node.Children.Add(Match(Token_Class.Until)); // Match 'until'
            node.Children.Add(Condition()); // Parse the condition directly without parentheses
            return node;
        }

        private Node Read_Statement()
        {
            Node node = new Node("Read_Statement");
            node.Children.Add(Match(Token_Class.Read));
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(Match(Token_Class.semicolon));
            return node;
        }

        private Node Write_Statement()
        {
            Node node = new Node("Write_Statement");
            node.Children.Add(Match(Token_Class.Write));
            node.Children.Add(Match(Token_Class.LParanthesis));
            if (IsvalidToken(Token_Class.Endl))
            {
                node.Children.Add(Match(Token_Class.Endl));
            }
            else
            {
                node.Children.Add(Expression());
            }
            node.Children.Add(Match(Token_Class.RParanthesis));
            node.Children.Add(Match(Token_Class.semicolon));
            return node;
        }

        private Node Statement()
        {
            Node node = new Node("Statement");
            if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                node.Children.Add(Declaration_Statement());
            }
            else if (IsvalidToken(Token_Class.Idenifier))
            {
                if (LookAhead(1) == Token_Class.AssignOp)
                {
                    node.Children.Add(Assignment_Statement());
                }
                else
                {
                    node.Children.Add(Function_Call());
                    node.Children.Add(Match(Token_Class.semicolon));
                }
            }
            else if (IsvalidToken(Token_Class.Read))
            {
                node.Children.Add(Read_Statement());
            }
            else if (IsvalidToken(Token_Class.Write))
            {
                node.Children.Add(Write_Statement());
            }
            else if (IsvalidToken(Token_Class.If))
            {
                node.Children.Add(If_Statement());
            }
            else if (IsvalidToken(Token_Class.Repeat))
            {
                node.Children.Add(Repeat_Statement());
            }
            return node;
        }

        private Node Statement_List()
        {
            if (IsvalidToken(Token_Class.Idenifier) || IsvalidToken(Token_Class.String)
              || IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float)
             || IsvalidToken(Token_Class.If) || IsvalidToken(Token_Class.Repeat)
             || IsvalidToken(Token_Class.Read) || IsvalidToken(Token_Class.Write))
            {
                Node node = new Node("Statement_List");
                node.Children.Add(Statement());
                node.Children.Add(Statement_List());
                return node;
            }
            return null;
        }

        private Node Function_Body()
        {
            Node node = new Node("Function_Body");
            node.Children.Add(Match(Token_Class.LCurlyBraces));
            node.Children.Add(Statement_List());
            node.Children.Add(Return_Statement());
            node.Children.Add(Match(Token_Class.RCurlyBraces));
            return node;
        }

        private Node Function_Statement()
        {
            Node node = new Node("Function_Statement");
            node.Children.Add(Function_Declaration());
            node.Children.Add(Function_Body());
            return node;
        }

        private Node Function_List_Opt()
        {
            // Skip comments
            while (IsvalidToken(Token_Class.Comment))
            {
                Match(Token_Class.Comment); // Skip the comment token
            }

            // Check for the main function first
            if (IsvalidToken(Token_Class.Main) ||
                (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String)) &&
                LookAhead(1) == Token_Class.Main)
            {
                Node node = new Node("Function_List_Opt");
                node.Children.Add(Main_Function());
                return node;
            }
            // Check for other functions with a datatype
            else if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                Node node = new Node("Function_List_Opt");
                node.Children.Add(Function_Statement());
                node.Children.Add(Function_List_Opt());
                return node;
            }
            return null;
        }



        private Node Factor()
        {
            Node node = new Node("Factor");
            if (IsvalidToken(Token_Class.LParanthesis))
            {
                node.Children.Add(Match(Token_Class.LParanthesis));
                node.Children.Add(Expression());
                node.Children.Add(Match(Token_Class.RParanthesis));
            }
            else if (IsvalidToken(Token_Class.constant))
            {
                node.Children.Add(Match(Token_Class.constant));
            }
            else if (IsvalidToken(Token_Class.StringLiteral))
            {
                node.Children.Add(Match(Token_Class.StringLiteral));
            }
            else
            {
                node.Children.Add(Identifier_Factor());
            }
            return node;
        }

        private Node Identifier_Factor()
        {
            Node node = new Node("Identifier_Factor");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(Factor_Tail());
            return node;
        }

        private Node Factor_Tail()
        {
            if (IsvalidToken(Token_Class.LParanthesis))
            {
                Node node = new Node("Factor_Tail");
                node.Children.Add(Argument_List_Opt());
                return node;
            }
            return null;
        }

        private Node Term_Tail()
        {
            if (IsvalidToken(Token_Class.MultiplyOp) || IsvalidToken(Token_Class.DivideOp))
            {
                Node node = new Node("Term_Tail");
                node.Children.Add(MultOp());
                node.Children.Add(Factor());
                node.Children.Add(Term_Tail());
                return node;
            }
            return null;
        }

        private Node Term()
        {
            Node node = new Node("Term");
            node.Children.Add(Factor());
            node.Children.Add(Term_Tail());
            return node;
        }

        private Node Expr_Tail()
        {
            if (IsvalidToken(Token_Class.PlusOp) || IsvalidToken(Token_Class.MinusOp))
            {
                Node node = new Node("Expr_Tail");
                node.Children.Add(AddOp());
                node.Children.Add(Term());
                node.Children.Add(Expr_Tail());
                return node;
            }
            return null;
        }

        private Node Expression()
        {
            Node node = new Node("Expression");
            node.Children.Add(Term());
            node.Children.Add(Expr_Tail());
            return node;
        }

        private bool IsvalidToken(Token_Class token)
        {
            return (TokenIndex < TokenStream.Count && TokenStream[TokenIndex].token_type == token);
        }

        private Token_Class LookAhead(int offset)
        {
            if (TokenIndex + offset < TokenStream.Count)
                return TokenStream[TokenIndex + offset].token_type;
            return 0;
        }

        private Node Match(Token_Class ExpectedToken)
        {
            if (TokenIndex < TokenStream.Count && ExpectedToken == TokenStream[TokenIndex].token_type)
            {
                TokenIndex++;
                Node newNode = new Node(ExpectedToken.ToString());
                return newNode;
            }
            else
            {
                if (TokenIndex < TokenStream.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[TokenIndex].token_type.ToString() +
                        " found\r\n"
                        + " at " + TokenStream[TokenIndex].token_type.ToString() + "\n");
                    TokenIndex++;
                }
                else
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and nothing was found\r\n");

                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }

        public static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}