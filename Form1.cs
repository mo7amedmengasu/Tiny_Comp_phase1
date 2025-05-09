using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Tiny_Comp_phase1;
namespace Tiny_Comp_phase1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            //string Code=textBox1.Text.ToLower();
            string Code = textBox1.Text;
            Tiny_Comp_phase1.Start_Compiling(Code);
            PrintTokens();
            //   PrintLexemes();
            PrintTokens();
            //PrintLexemes();
            treeView2.Nodes.Add(Parser.PrintParseTree(Tiny_Comp_phase1.treeroot));

            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < Tiny_Comp_phase1.Tiny_Scanner.Tokens.Count; i++)
            {
                dataGridView1.Rows.Add(Tiny_Comp_phase1.Tiny_Scanner.Tokens.ElementAt(i).lex, Tiny_Comp_phase1.Tiny_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for (int i = 0; i < Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Tiny_Comp_phase1.TokenStream.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        /*  void PrintLexemes()
{
for (int i = 0; i < JASON_Compiler.Lexemes.Count; i++)
{
textBox2.Text += JASON_Compiler.Lexemes.ElementAt(i);
textBox2.Text += Environment.NewLine;
}
}*/
    }
}
