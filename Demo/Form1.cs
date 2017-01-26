using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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

            var total= (Expression.Lambda(
                Expression.Add(x, y), x, y)
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

            var argument = Expression.Parameter(typeof(Person));
            var valueProperty = Expression.Property(argument, "Name");
            var containsCall = Expression.Call(valueProperty,
                typeof(string).GetMethod(
                    "Contains", new Type[] {typeof(string)}),
                Expression.Constant("a", typeof(string)));
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
    }

    public sealed class Person
    {
        public string Name { get; set; }
    }
}
