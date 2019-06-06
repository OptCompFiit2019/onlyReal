using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleLang;

namespace OptGui
{
    public partial class Form1 : Form
    {
        private string FileName = "";

        /*
             *             "Исходный код",
            "Трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Запуск",
            "Оптимизации по дереву",           
            "Полученный трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Запуск",*/
        enum Modes { Text, BeforeThreeCode, BeforeBlocks,  BeforeGraph, BeforeRun,
            ASTOpt, AfterTbreeCode, AfterBlocks, AfterGraph, AfterRun}

        public Form1()
        {
            InitializeComponent();
            UpdateForms();
        }
        public SimpleLang.Visitors.AutoApplyVisitor GetASTOptimizer()
        {
            SimpleLang.Visitors.AutoApplyVisitor res = new SimpleLang.Visitors.AutoApplyVisitor();
            if (checkBox51.Checked)
                res.Add(new SimpleLang.Visitors.Opt2Visitor());
            if (checkBox52.Checked)
                res.Add(new SimpleLang.Visitors.Opt11Visitor());
            return res;
        }
        public SimpleLang.ThreeCodeOptimisations.AutoThreeCodeOptimiser GetOptimiser() {
            SimpleLang.ThreeCodeOptimisations.AutoThreeCodeOptimiser res = new SimpleLang.ThreeCodeOptimisations.AutoThreeCodeOptimiser();
            if (checkBox1.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.DistributionOfConstants());
            if (checkBox2.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.EvalConstExpr());
            if (checkBox3.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.ApplyAlgebraicIdentities());
            if (checkBox4.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.DeadOrAliveOptimizationAdapter());
            if (checkBox5.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.NonZero_JTJ());
            if (checkBox6.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.DefUseConstOpt());
            if (checkBox7.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.DefUseDeadCodeOpt());
            if (checkBox8.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.DeleteOfDeadCodeOpt());
            if (checkBox9.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.PullOfCopiesOpt());
            if (checkBox10.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.UnreachableCodeOpt());
            return res;
        }
        void UpdateForms() {
            Modes m1 = DetectMode(comboBox1);
            Modes m2 = DetectMode(comboBox2);

            button2.Enabled = true;
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;

            bool enable_save = true;
            enable_save = !(m1 == Modes.Text && m2 == Modes.Text);

            enable_save = enable_save && ((m1 == Modes.Text) || (m2 == Modes.Text)) && FileName.Length > 0;

            button2.Enabled = enable_save;

            textBox1.ReadOnly = m1 != Modes.Text;
            textBox2.ReadOnly = m2 != Modes.Text;
            this.Text = "FIIT";
            if (FileName.Length > 0)
                this.Text = this.Text + ": " + FileName;
        }
        Modes DetectMode(ComboBox box) {
            /*
             *             "Исходный код",
            "Трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Запуск",
            "Оптимизации по дереву",            
            "Полученный трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Запуск",*/
            switch (box.SelectedIndex)
            {
                case 1: return Modes.BeforeThreeCode;
                case 2: return Modes.BeforeBlocks;
                case 3: return Modes.BeforeGraph;
                case 4: return Modes.BeforeRun;
                case 5: return Modes.ASTOpt;
                case 6: return Modes.AfterTbreeCode;
                case 7: return Modes.AfterBlocks;
                case 8: return Modes.AfterGraph;
                case 9: return Modes.AfterRun;
                default: return Modes.Text;
            }
        }
        private void UpdateTab(Modes m, TextBox txt) {
            if (FileName.Length == 0) {
                txt.Text = "File is not set";
                return;
            }
            try {
                string tt = System.IO.File.ReadAllText(FileName);
                if (m == Modes.Text) {
                    txt.Text = tt;
                    return;
                }
                SimpleScanner.Scanner scan = new SimpleScanner.Scanner();
                scan.SetSource(tt, 0);
                SimpleParser.Parser pars = new SimpleParser.Parser(scan);
                var b = pars.Parse();
                if (!b) {
                    txt.Text = "Ошибка парсинга";
                    return;
                }
                var r = pars.root;
                SimpleLang.Visitors.FillParentVisitor parVisitor = new SimpleLang.Visitors.FillParentVisitor();
                r.Visit(parVisitor);

                SimpleLang.Visitors.AutoApplyVisitor optAst = GetASTOptimizer();
                optAst.Apply(r);
                if (m == Modes.ASTOpt)
                {
                    SimpleLang.Visitors.PrettyPrintVisitor vis = new SimpleLang.Visitors.PrettyPrintVisitor();
                    r.Visit(vis);
                    txt.Text = vis.Text;
                    return;
                }

                SimpleLang.Visitors.ThreeAddressCodeVisitor threeCodeVisitor = new SimpleLang.Visitors.ThreeAddressCodeVisitor();
                r.Visit(threeCodeVisitor);
                if (m == Modes.BeforeThreeCode) {
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(threeCodeVisitor.GetCode());
                    Console.WriteLine(txt.Text);
                    return;
                }
                if (m == Modes.BeforeRun) {
                    SimpleLang.Compiler.ILCodeGenerator gen = new SimpleLang.Compiler.ILCodeGenerator();
                    gen.Generate(threeCodeVisitor.GetCode());
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    string res = gen.Execute();
                    timer.Stop();
                    res = res + "\n\n\nExecuted: " + timer.ElapsedMilliseconds.ToString() + " ms"
                        + " or " + timer.ElapsedTicks.ToString() + " ticks";
                    txt.Text = res;
                    return;
                }
                
                if (m == Modes.BeforeBlocks) {
                    var blocks = new SimpleLang.Block.Block(threeCodeVisitor).GenerateBlocks();
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(blocks);
                    return;
                }
                var opt = GetOptimiser();
                var outcode = opt.Apply(threeCodeVisitor);

                if (m == Modes.AfterBlocks) {
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(outcode);
                    return;
                }
                if (m == Modes.AfterTbreeCode) {
                    System.Collections.Generic.LinkedList<SimpleLang.Visitors.ThreeCode> res = new System.Collections.Generic.LinkedList<SimpleLang.Visitors.ThreeCode>();
                    foreach (var block in outcode)
                        foreach (SimpleLang.Visitors.ThreeCode code in block)
                            res.AddLast(code);
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(res);
                    Console.WriteLine(txt.Text);
                    return;
                }
                if (m == Modes.AfterRun) {
                    System.Collections.Generic.LinkedList<SimpleLang.Visitors.ThreeCode> res = new System.Collections.Generic.LinkedList<SimpleLang.Visitors.ThreeCode>();
                    foreach (var block in outcode)
                        foreach (SimpleLang.Visitors.ThreeCode code in block)
                            res.AddLast(code);
                    SimpleLang.Compiler.ILCodeGenerator gen = new SimpleLang.Compiler.ILCodeGenerator();
                    gen.Generate(res);
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    string ooo = gen.Execute();
                    timer.Stop();
                    ooo = ooo + "\n\n\nExecuted: " + timer.ElapsedMilliseconds.ToString() + " ms"
                        + " or " + timer.ElapsedTicks.ToString() + " ticks";
                    txt.Text = ooo;
                    return;
                }

                txt.Text = "Is not implemented";

            } catch (Exception e) {
                txt.Text = e.ToString();
            }
        }
        private void UpdateMode() {
            Modes m1 = DetectMode(comboBox1);
            Modes m2 = DetectMode(comboBox2);

            UpdateTab(m1, textBox1);
            UpdateTab(m2, textBox2);

        }

        private void button1_Click(object sender, EventArgs e) {
            FileName = "";
            var res = openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length > 0)
            {
                FileName = openFileDialog1.FileName;
                UpdateMode();
            }
            UpdateForms();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Modes m1 = DetectMode(comboBox1);
            UpdateTab(m1, textBox1);
            UpdateForms();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Modes m1 = DetectMode(comboBox2);
            UpdateTab(m1, textBox2);
            UpdateForms();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FileName.Length == 0)
                return;
            Modes m1 = DetectMode(comboBox1);
            Modes m2 = DetectMode(comboBox2);

            string newtxt = "";
            if (m1 == Modes.Text)
                newtxt = textBox1.Text;
            if (m2 == Modes.Text)
                newtxt = textBox2.Text;
            if (newtxt.Length > 0) {
                System.IO.File.WriteAllText(FileName, newtxt);
                UpdateMode();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }
    }
}
