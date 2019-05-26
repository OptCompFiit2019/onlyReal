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
            "Полученный трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Запуск",*/
        enum Modes { Text, BeforeThreeCode, BeforeBlocks,  BeforeGraph, BeforeRun,
            AfterTbreeCode, AfterBlocks, AfterGraph, AfterRun}

        public Form1()
        {
            InitializeComponent();
        }
        Modes DetectMode(ComboBox box) {
            /*
             *             "Исходный код",
            "Трехадресный код",
            "Блоки трехадресного кода",
            "Граф потока управления",
            "Запуск",
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
                case 5: return Modes.AfterTbreeCode;
                case 6: return Modes.AfterBlocks;
                case 7: return Modes.AfterGraph;
                case 8: return Modes.AfterRun;
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
                SimpleLang.Visitors.ThreeAddressCodeVisitor threeCodeVisitor = new SimpleLang.Visitors.ThreeAddressCodeVisitor();
                r.Visit(threeCodeVisitor);
                if (m == Modes.BeforeThreeCode) {
                    txt.Text = SimpleLang.Visitors.ThreeAddressCodeVisitor.ToString(threeCodeVisitor.GetCode());
                    return;
                }
                    
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Modes m1 = DetectMode(comboBox1);
            UpdateTab(m1, textBox1);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Modes m1 = DetectMode(comboBox2);
            UpdateTab(m1, textBox2);
        }
    }
}
