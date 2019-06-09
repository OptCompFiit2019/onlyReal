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
        enum Modes { Text, BeforeThreeCode, BeforeBlocks,  BeforeGraph, BeforeRun, BeforeMass,
            AfterMass, ASTOpt, AfterTbreeCode, AfterBlocks, AfterGraph, AfterRun}

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
            if (checkBox53.Checked)
                res.Add(new SimpleLang.Visitors.OptVisitor_8());
            if (checkBox54.Checked)
                res.Add(new SimpleLang.Visitors.OptVisitor_13());
            if (checkBox55.Checked)
                res.Add(new SimpleLang.Optimisations.OptSimilarDifference());
            if (checkBox56.Checked)
                res.Add(new SimpleLang.Optimisations.OptSimilarAssignment());
            if (checkBox57.Checked)
                res.Add(new SimpleLang.Visitors.OptMulDivOneVisitor());
            if (checkBox58.Checked)
                res.Add(new SimpleLang.Visitors.OptWhileVisitor());
            if (checkBox59.Checked)
                res.Add(new SimpleLang.Visitors.PlusNonZero());
            if (checkBox60.Checked)
                res.Add(new SimpleLang.Visitors.ElseStVisitor());
            if (checkBox61.Checked)
                res.Add(new SimpleLang.Visitors.LessOptVisitor());
            if (checkBox62.Checked)
                res.Add(new SimpleLang.Visitors.MultiplicationComputeVisitor());
            if (checkBox63.Checked)
                res.Add(new SimpleLang.Visitors.Opt7Visitor());
            if (checkBox64.Checked)
                res.Add(new SimpleLang.AstOptimisations.LinearizeBlocks());
            if (checkBox65.Checked)
                res.Add(new SimpleLang.AstOptimisations.FalseExprMoreAndNonEqualVisitor());
            if (checkBox66.Checked)
                res.Add(new SimpleLang.Visitors.DeleteNullVisitor());
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
                res.Add(new SimpleLang.ThreeCodeOptimisations.NonZero_JTJOpt());
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
            if (checkBox11.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.LVNOptimization());
            if (checkBox12.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.DAGOpt());
            if (checkBox13.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.CommonExprOpt());
            if (checkBox14.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.EliminationTranToTranOpt());
            if (checkBox15.Checked)
                res.Add(new SimpleLang.ThreeCodeOptimisations.PullCopiesOpt());
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
            "Исходный код",
            "Трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Множества",
            "Запуск",
            "",
            "Оптимизации по дереву",
            "Полученный трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Множества",
            "Запуск"});*/
            switch (box.SelectedIndex)
            {
                case 1: return Modes.BeforeThreeCode;
                case 2: return Modes.BeforeBlocks;
                case 3: return Modes.BeforeGraph;
                case 4: return Modes.BeforeMass;
                case 5: return Modes.BeforeRun;
                case 6: return Modes.ASTOpt;
                case 7: return Modes.ASTOpt;
                case 8: return Modes.AfterTbreeCode;
                case 9: return Modes.AfterBlocks;
                case 10: return Modes.AfterGraph;
                case 11: return Modes.AfterMass;
                case 12: return Modes.AfterRun;
                default: return Modes.Text;
            }
        }
        private void DrawCFG(PictureBox box, SimpleLang.ControlFlowGraph.Graph graph, List<LinkedList<SimpleLang.Visitors.ThreeCode>> code, Panel panel)
        {
            List<List<int>> matr = graph.GetAdjacencyList();
            int depth = matr.Count;
            List<Point> points = new List<Point>(matr.Count);
            List<Point> pointsEnd = new List<Point>(matr.Count);
            int new_width = code[0].Count * 13;

            const int WEIGHT = 180;
            const int DELIMER = 30;

            List<Point> weigh_count = new List<Point>();
            Dictionary<int, int> levels = new Dictionary<int, int>();
            levels[0] = 0;
            weigh_count.Add(new Point(0, 1));
            weigh_count.Add(new Point(code[0].Count * 13 + DELIMER, 1));

            for (int i = 0; i < matr.Count; i++)
            {
                int tmp = matr[i].Count - 1;
                depth -= tmp < 0 ? 0 : tmp;
                points.Add(new Point(0,0));
                pointsEnd.Add(new Point(0, 0));

                int max = 0;

                int cc = 0;

                for (int j = 0; j < matr[i].Count; j++) {
                    max = Math.Max(max, code[matr[i][j]].Count);
                    if (!levels.ContainsKey(matr[i][j]))
                    {
                        cc++;
                        levels.Add(matr[i][j], i+1);
                    }
                }
                new_width += max * 13 + DELIMER;
                weigh_count.Add(new Point(weigh_count[levels[i] + 1].X +  max * 13 + DELIMER, cc));
            }


            Bitmap bitmap = new Bitmap(panel.Width, new_width);
            Graphics grap = Graphics.FromImage(bitmap);

            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            Pen pen = new Pen(drawBrush);

            //grap.DrawString("sssss", drawFont, drawBrush, new PointF(450, 100));

            
            {
                Point first =  new Point(panel.Height / 2 - WEIGHT / 2, 0);
                points[0] = first;
                string text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(code[0]);
                grap.DrawString(text, drawFont, drawBrush, first);
                grap.DrawRectangle(pen, first.X, first.Y, WEIGHT, code[0].Count * 13);
                pointsEnd[0] = new Point(first.X + WEIGHT, first.Y + code[0].Count * 13);
            }
            

            for (int i = 0; i < matr.Count; i++) {
                Point parent = pointsEnd[i];
                for (int j = 0; j < matr[i].Count; j++) {
                    if (points[matr[i][j]].X != 0 && points[matr[i][j]].Y != 0) {
                        //ToDo can be 2 out in blocks
                        Point cur = points[matr[i][j]];
                        Point c = new Point(cur.X + WEIGHT / 2, cur.Y);

                        grap.DrawLines(pen, new Point[] { parent, c });

                    } else {
                        int x = (parent.X - WEIGHT / 2) / matr[i].Count;

                        //ToDo can be 2 out in blocks
                        Point cur;
                        if (matr[i].Count > 1)
                            cur = new Point(parent.X - x + (j * 2) * x - WEIGHT, weigh_count[levels[matr[i][j]]].X);//parent.Y + DELIMER);
                        else {
                            if (weigh_count[levels[matr[i][j]] + 1].Y == 1)
                                cur = new Point(panel.Height / 2 - WEIGHT / 2, weigh_count[levels[matr[i][j]]].X);//parent.Y + DELIMER);
                            else
                                cur = new Point(parent.X - WEIGHT, weigh_count[levels[matr[i][j]]].X);// parent.Y + DELIMER);
                        }

                        points[matr[i][j]] = cur;
                        string text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(code[matr[i][j]]);
                        grap.DrawString(text, drawFont, drawBrush, cur);
                        grap.DrawRectangle(pen, cur.X, cur.Y, WEIGHT, code[matr[i][j]].Count * 13);

                        Point c = new Point(cur.X + WEIGHT / 2, cur.Y);
                        grap.DrawLines(pen, new Point[]{parent, c});

                        pointsEnd[matr[i][j]] = new Point(cur.X + WEIGHT, cur.Y + code[matr[i][j]].Count * 13);
                        
                    }
                }
            }

            box.Image = bitmap;
            box.Invalidate();
        }

        private void DrawMass(List<LinkedList<SimpleLang.Visitors.ThreeCode>> code, TextBox txt) {
            if (radioButton1.Checked) {
                SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(code);
                SimpleLang.Dominators.DominatorsFinder finder = new SimpleLang.Dominators.DominatorsFinder(gra);
                finder.Find();
                txt.Text = finder.ToString();
            }
            if (radioButton2.Checked)
            {
                SimpleLang.ThreeCodeOptimisations.ReachingDefsAnalysis defs = new SimpleLang.ThreeCodeOptimisations.ReachingDefsAnalysis();
                defs.IterativeAlgorithm(code);
                txt.Text = defs.GetOutput();
            }
            if (radioButton3.Checked)
            {
                var tmp = System.Console.Out;
                System.IO.MemoryStream stre = new System.IO.MemoryStream();
                System.IO.TextWriter wr = new System.IO.StreamWriter(stre);
                Console.SetOut(wr);

                SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(code);
                SimpleLang.DetectReversibleEdges find = new SimpleLang.DetectReversibleEdges(gra);
                find.PrintIsReverseDic();
                

                Console.SetOut(tmp);
                wr.Flush();
                stre.Flush();
                string res = Encoding.UTF8.GetString(stre.ToArray());
                txt.Text = res;
            }
            if (radioButton4.Checked)
            {
                var tmp = System.Console.Out;
                System.IO.MemoryStream stre = new System.IO.MemoryStream();
                System.IO.TextWriter wr = new System.IO.StreamWriter(stre);
                Console.SetOut(wr);

                SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(code);
                SimpleLang.DetectReversibleEdges find = new SimpleLang.DetectReversibleEdges(gra);
                find.PrintisReducible();


                Console.SetOut(tmp);
                wr.Flush();
                stre.Flush();
                string res = Encoding.UTF8.GetString(stre.ToArray());
                txt.Text = res;
            }
            if (radioButton5.Checked)
            {
                SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(code);
                SimpleLang.ThreeCodeOptimisations.DefUseBlocks def = new SimpleLang.ThreeCodeOptimisations.DefUseBlocks(gra);
                var d1 = def.DefBs;
                var d2 = def.UseBs;

                string res = "";

                if (d1.Count == d2.Count) {
                    for (int i = 0; i < d1.Count; i++) {
                        res = res + "----------------------" + Environment.NewLine + "Block " + i.ToString() + Environment.NewLine;
                        var a1 = d1[i];
                        var a2 = d2[i];
                        res = res +  "\tDefB:" + Environment.NewLine;
                        foreach (string t in a1)
                        {
                            res = res + "\t\t" + t + Environment.NewLine;
                        }
                        res = res + "\tUseB:" + Environment.NewLine;
                        foreach (string t in a2)
                        {
                            res = res + "\t\t" + t + Environment.NewLine;
                        }
                    }
                }
                txt.Text = res;
            }
            if (radioButton6.Checked)
            {
                var tmp = System.Console.Out;
                System.IO.MemoryStream stre = new System.IO.MemoryStream();
                System.IO.TextWriter wr = new System.IO.StreamWriter(stre);
                Console.SetOut(wr);


                SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(code);
                SpanTree span = new SpanTree(gra);
                span.buildSpanTree();
                span.writeAllSpanTreeEdges();
                Console.WriteLine();
                Console.WriteLine("\t\tTypes:");
                span.writeAllEdgesWithTypes();

                Console.SetOut(tmp);
                wr.Flush();
                stre.Flush();
                string res = Encoding.UTF8.GetString(stre.ToArray());
                txt.Text = res;
            }
            if (radioButton7.Checked) {
                SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(code);
                SimpleLang.ThreeCodeOptimisations.DefUseBlocks def = new SimpleLang.ThreeCodeOptimisations.DefUseBlocks(gra);
                var ou = new SimpleLang.ThreeCodeOptimisations.InOutActiveVariables(def, gra);
                List<HashSet<string>> _in = ou.InBlocks;
                List<HashSet<string>> _out = ou.OutBlocks;
                string res = "";
                for (int i = 0; i < _in.Count; i++) {
                    res = res + "----------------------" + Environment.NewLine + "Block " + i.ToString() + Environment.NewLine;
                    res = res + "\tIN:" + Environment.NewLine;
                    foreach (string t in _in[i])
                        res = res + "\t\t" + t + Environment.NewLine;
                    res = res + "\tOUT:" + Environment.NewLine;
                    foreach (string t in _out[i])
                        res = res + "\t\t" + t + Environment.NewLine;
                }
                txt.Text = res;
            }
        }
        private void UpdateTab(Modes m, TextBox txt, PictureBox box, Panel panel) {
            if (FileName.Length == 0) {
                txt.Text = "File is not set";
                return;
            }
            try {
                if (m == Modes.AfterGraph || m == Modes.BeforeGraph) {
                    panel.Visible = true;
                    txt.Visible = false;
                } else {
                    panel.Visible = false;
                    txt.Visible = true;
                }
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

                    List<long> datas = new List<long>();
                    List<long> datas2 = new List<long>();
                    int count_run = Int32.Parse(textBox3.Text);
                    string res = "";
                    for (int i = 0; i < count_run; i++)
                    {
                        var timer = System.Diagnostics.Stopwatch.StartNew();
                        res = gen.Execute();
                        timer.Stop();

                        datas.Add(timer.ElapsedMilliseconds);
                        datas2.Add(timer.ElapsedTicks);
                    }

                    res = res + Environment.NewLine + Environment.NewLine + "Executed avg: " + (datas.Min()).ToString() + " ms"
                            + " or " + (datas2.Min()).ToString() + " ticks" + Environment.NewLine;
                    for (int i = 0; i < datas.Count; i++) {
                        res = res + i.ToString() + ": " + datas[i].ToString() + " ms or " + datas2[i].ToString() + " ticks" + Environment.NewLine;
                    }


                    txt.Text = res;
                    return;
                }
                
                if (m == Modes.BeforeBlocks) {
                    var blocks = new SimpleLang.Block.Block(threeCodeVisitor).GenerateBlocks();
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(blocks);
                    return;
                }
                if (m == Modes.BeforeMass) {
                    var blocks = new SimpleLang.Block.Block(threeCodeVisitor).GenerateBlocks();
                    DrawMass(blocks, txt);
                    return;
                }
                if (m == Modes.BeforeGraph)
                {
                    var blocks = new SimpleLang.Block.Block(threeCodeVisitor).GenerateBlocks();
                    SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(blocks);
                    DrawCFG(box, gra.cfg, blocks, panel);
                    return;
                }

                SimpleLang.Visitors.AutoApplyVisitor optAst = GetASTOptimizer();
                optAst.Apply(r);
                if (m == Modes.ASTOpt)
                {
                    SimpleLang.Visitors.PrettyPrintVisitor vis = new SimpleLang.Visitors.PrettyPrintVisitor();
                    r.Visit(vis);
                    txt.Text = vis.Text;
                    return;
                }

                threeCodeVisitor = new SimpleLang.Visitors.ThreeAddressCodeVisitor();
                r.Visit(threeCodeVisitor);
                var opt = GetOptimiser();
                var outcode = opt.Apply(threeCodeVisitor);

                if (m == Modes.AfterMass) {
                    DrawMass(outcode, txt);
                    return;
                }

                if (m == Modes.AfterBlocks) {
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(outcode);
                    return;
                }
                if (m == Modes.AfterGraph) {
                    SimpleLang.ControlFlowGraph.ControlFlowGraph gra = new SimpleLang.ControlFlowGraph.ControlFlowGraph(outcode);
                    DrawCFG(box, gra.cfg, outcode, panel);
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

                    List<long> datas = new List<long>();
                    List<long> datas2 = new List<long>();
                    int count_run = Int32.Parse(textBox3.Text);
                    string re2 = "";
                    for (int i = 0; i < count_run; i++)
                    {
                        var timer = System.Diagnostics.Stopwatch.StartNew();
                        re2 = gen.Execute();
                        timer.Stop();

                        datas.Add(timer.ElapsedMilliseconds);
                        datas2.Add(timer.ElapsedTicks);
                    }

                    re2 = re2 + Environment.NewLine + Environment.NewLine +"Executed avg: " + (datas.Min()).ToString() + " ms"
                            + " or " + (datas2.Min()).ToString() + " ticks" + Environment.NewLine;
                    for (int i = 0; i < datas.Count; i++)
                    {
                        re2 = re2 + i.ToString() + ": " + datas[i].ToString() + " ms or " + datas2[i].ToString() + " ticks" + Environment.NewLine;
                    }
                    txt.Text = re2;
                    return;
                }

                txt.Text = "Is not implemented";

            } catch (Exception e) {
                panel.Visible = false;
                txt.Visible = true;
                txt.Text = e.ToString();
            }
        }
        private void UpdateMode() {
            Modes m1 = DetectMode(comboBox1);
            Modes m2 = DetectMode(comboBox2);

            UpdateTab(m1, textBox1, pictureBox1, panel1);
            UpdateTab(m2, textBox2, pictureBox2, panel2);

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
        private void Setting_count_run_textChange(object sender, EventArgs even) {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < textBox3.Text.Length; i++)
            {
                if (Char.IsDigit(textBox3.Text[i]))
                    b.Append(textBox3.Text[i]);
            }
            textBox3.Text = b.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Modes m1 = DetectMode(comboBox1);
            UpdateTab(m1, textBox1, pictureBox1, panel1);
            UpdateForms();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Modes m1 = DetectMode(comboBox2);
            UpdateTab(m1, textBox2, pictureBox2, panel2);
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
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            checkBox51.Checked = checkBox67.Checked;
            checkBox52.Checked = checkBox67.Checked;
            checkBox53.Checked = checkBox67.Checked;
            checkBox54.Checked = checkBox67.Checked;
            checkBox55.Checked = checkBox67.Checked;
            checkBox56.Checked = checkBox67.Checked;
            checkBox57.Checked = checkBox67.Checked;
            checkBox58.Checked = checkBox67.Checked;
            checkBox59.Checked = checkBox67.Checked;
            checkBox60.Checked = checkBox67.Checked;
            checkBox61.Checked = checkBox67.Checked;
            checkBox62.Checked = checkBox67.Checked;
            checkBox63.Checked = checkBox67.Checked;
            checkBox64.Checked = checkBox67.Checked;
            checkBox65.Checked = checkBox67.Checked;
            checkBox66.Checked = checkBox67.Checked;
            UpdateMode();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = checkBox16.Checked;
            checkBox2.Checked = checkBox16.Checked;
            checkBox3.Checked = checkBox16.Checked;
            checkBox4.Checked = checkBox16.Checked;
            checkBox5.Checked = checkBox16.Checked;
            checkBox6.Checked = checkBox16.Checked;
            checkBox7.Checked = checkBox16.Checked;
            checkBox8.Checked = checkBox16.Checked;
            checkBox9.Checked = checkBox16.Checked;
            checkBox10.Checked = checkBox16.Checked;
            checkBox11.Checked = checkBox16.Checked;
            checkBox12.Checked = checkBox16.Checked;
            checkBox13.Checked = checkBox16.Checked;
            checkBox14.Checked = checkBox16.Checked;
            checkBox15.Checked = checkBox16.Checked;
            UpdateMode();
        }
    }
}
