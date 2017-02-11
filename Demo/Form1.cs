using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Expression<Func<int, int, int>> demo = (x, y) => x + y;
            var result = demo.Compile()(1, 2);
            MessageBox.Show(result.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));

            var total = (Expression.Lambda(Expression.Add(x, y), x, y)
                .Compile() as Func<int, int, int>)(2, 3);

            MessageBox.Show(total.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Expression sumExpr = Expression.Add(
                Expression.Constant(2),
                Expression.Constant(3)
            );

            MessageBox.Show((Expression.Lambda<Func<int>>(sumExpr).Compile()()).ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var people = new List<Person>()
            {
                new Person {Name = "John"},
                new Person {Name = "Mike"},
                new Person {Name = "James"},
                new Person {Name = "Phil"}
            };

            // var results = people.Where(x => x.Name.Contains("a"));
            var inputText = "a";

            var argument = Expression.Parameter(typeof(Person));
            var valueProperty = Expression.Property(argument, "Name");
            var containsCall = Expression.Call(valueProperty,
                typeof(string).GetMethod(
                    "Contains", new Type[] {typeof(string)}),
                Expression.Constant(inputText, typeof(string)));
            var wherePredicate = Expression.Lambda<Func<Person, bool>>(
                containsCall, argument);
            var whereCall = Expression.Call(typeof(Queryable), "Where",
                new Type[] {typeof(Person)},
                people.AsQueryable().Expression, wherePredicate);

            var expressionResults = people.AsQueryable()
                .Provider.CreateQuery<Person>(whereCall);
            MessageBox.Show(string.Join(", ",
                expressionResults.ToList().Select(x => x.Name)));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CodeNamespace prgNamespace = HelloWorldCodeDOM.BuildProgram();
            var compilerOptions = new CodeGeneratorOptions()
            {
                IndentString = " ",
                BracingStyle = "C",
                BlankLinesBetweenMembers = false
            };
            var codeText = new StringBuilder();
            using (var codeWriter = new StringWriter(codeText))
            {
                CodeDomProvider.CreateProvider("vb")
                .GenerateCodeFromNamespace(
                prgNamespace, codeWriter, compilerOptions);
            }
            var script = codeText.ToString();
            MessageBox.Show(script);
        }

        partial class HelloWorldCodeDOM
        {
            public static CodeNamespace BuildProgram()
            {
                var ns = new CodeNamespace("MetaWorld");
                var systemImport = new CodeNamespaceImport("System");
                ns.Imports.Add(systemImport);
                var programClass = new CodeTypeDeclaration("Program");
                ns.Types.Add(programClass);
                var methodMain = new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Static,
                    Name = "Main"
                };
                methodMain.Statements.Add(new CodeMethodInvokeExpression(new CodeSnippetExpression("Console"), "WriteLine",
                    new CodePrimitiveExpression("Hello, world!")));
                programClass.Members.Add(methodMain);
                return ns;
            }
        }

        private static int a = 10, b = 20;
        private void button5_Click(object sender, EventArgs e)
        {
            Type t = typeof(Form1);
            string varName = "a";
            FieldInfo fieldInfo = t.GetField(varName, BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo != null)
            {
                int newInt = -10;
                fieldInfo.SetValue(null, newInt);
                MessageBox.Show("a * b = " + (a * b));
            }
        }
    }
    
    public sealed class Person
    {
        public string Name { get; set; }
    }
}
