using System;
using System.Collections.Generic;
using System.Text;

namespace Yeast.WebApi.Tests.TestObjects
{
    public class Person
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public Person Conjoint { get; set; }

        public DateTime? BirthDate { get; set; }

        public IEnumerable<Person> Children { get; set; }

        public Person[] Parents { get; set; }

        public Array Ascendants { get; set; }
    }
}
