using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ClassGenerator
{
    public partial class Form1 : Form
    {
        private string[] variablesTypes = new string [20];
        private int sizeVariablesTypes = 0;
        private string[] variables = new string [20];
        private int sizeVariables = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string className = txtClassName.Text;
            string folderPath = "";

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "";
            if (fbd.ShowDialog() == DialogResult.OK)
                folderPath = fbd.SelectedPath;

            FileStream fileH = new FileStream(folderPath + "\\" + className + ".h", FileMode.Create, FileAccess.Write);
            FileStream fileCpp = new FileStream(folderPath + "\\" + className + ".cpp", FileMode.Create, FileAccess.Write);

            StreamWriter writeCpp = new StreamWriter(fileCpp);
            writeCpp.Write( lib() + include() + " \"" + className + ".h\"" + "\n\n" + 
                className + "::" +  constructor(className) + "\n" + open() + close() + "\n\n    " + 
                className + "::" + destructor(className) + "\n" + open() + close() + "\n\n" + 
                className + "::" + className + "(" );
            // constructor with arg
            for(int i = 0; i < sizeVariablesTypes; i++)
            {
                writeCpp.Write( " " + variablesTypes[i] + " " + variables[i] );
                if (i + 1 != sizeVariablesTypes)
                    writeCpp.Write(", ");
            }
            writeCpp.Write(" )\n{\n");
            for(int i = 0; i < sizeVariablesTypes; i++)
            {
                writeCpp.Write("    _" + variables[i] + " = " + variables[i] + ";\n");
            }
            writeCpp.Write("}\n");
            for (int i = 0; i < sizeVariablesTypes; i++)
            {
                // getter
                writeCpp.Write("\nconst " + variablesTypes[i] + " " + className + "::get" + 
                    variables[i].First().ToString().ToUpper() + String.Join("", variables[i].Skip(1))
                    + "(){\n    return _" + variables[i] + semicolon() + "}\n");
                // setter
                writeCpp.Write("\nvoid " + className + "::set" + variables[i].First().ToString().ToUpper() 
                    + String.Join("", variables[i].Skip(1)) + "(" + variablesTypes[i] + " " + variables[i] +
                    ")" + open() + "    _" + variables[i] + " = " + variables[i] + ";\n" + close() + "\n");
            }

            StreamWriter writeH = new StreamWriter(fileH);
            writeH.Write( pragma() + cname(className) + open() + priv() );
            for(int i = 0; i < sizeVariablesTypes; i++)
            {
                writeH.Write("    " + variablesTypes[i] + " _" + variables[i] + semicolon());
            }
            writeH.Write("\n" + publ() + "    " + constructor(className) + semicolon() +
                "    " + destructor(className) + semicolon() + "    "  + className + "(");
            // constructor with args
            for (int i = 0; i < sizeVariablesTypes; i++)
            {
                writeH.Write(" " + variablesTypes[i] + " " + variables[i] );
                if (i + 1 != sizeVariablesTypes)
                    writeH.Write(", ");
            }
            writeH.Write(" );\n");
            for (int i = 0; i < sizeVariablesTypes; i++)
            {
                // getter
                writeH.Write("    const " + variablesTypes[i] + " get" + variables[i].First().ToString().ToUpper() + 
                    String.Join("",variables[i].Skip(1)) + "();\n");
                // setter
                writeH.Write("    void set" + variables[i].First().ToString().ToUpper() + 
                    String.Join("", variables[i].Skip(1)) + "(" + variablesTypes[i] + " " + 
                    variables[i] + ");\n");
            }
            writeH.Write("\n" + close() + semicolon());
            writeH.Close();
            writeCpp.Close();
            txtClassName.Clear();
        }

        private void btnAddVariable_Click(object sender, EventArgs e)
        {
            string type = txtType.Text;
            string name = txtName.Text;
            variablesTypes[sizeVariablesTypes] = type;
            variables[sizeVariables] = name;
            sizeVariables += 1;
            sizeVariablesTypes += 1;
            txtType.Clear();
            txtName.Clear();
        }

        string lib()
        {
            return "#include \"stdafx.h\"\n";
        }

        string include()
        {
            return "#include";
        }

        string pragma()
        {
            return "#pragma once\n\n";
        }

        string cname(string className)
        {
            return "class " + className + "\n";
        }

        string publ()
        {
            return "public:\n";
        }

        string priv()
        {
            return "protected:\n";
        }

        string constructor(string className)
        {
            return className + "()";
        }

        string destructor(string className)
        {
            return "~" + className + "()";
        }

        string open()
        {
            return "{\n";
        }

        string close()
        {
            return "}";
        }

        string semicolon()
        {
            return ";\n";
        }
    }
}
