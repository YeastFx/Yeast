using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;
using Yeast.WebApi.Tests.TestObjects;

namespace Yeast.WebApi.Tests
{
    public class PartialBuilderTests
    {
        [Fact]
        public void SupportsNullSource()
        {
            // Act
            var result = PartialBuilder.ToPartial(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ProjectsOnlyRequestedFields()
        {
            // Arrange
            var person = new Person
            {
                Firstname = "John",
                Lastname = "Doe"
            };

            var fields = new[] { nameof(Person.Firstname) };

            // Act
            dynamic result = PartialBuilder.ToPartial(person, fields);

            // Assert
            Assert.Throws<RuntimeBinderException>(() => result.Id);
            Assert.Equal("John", result.Firstname);
            Assert.Throws<RuntimeBinderException>(() => result.Lastname);
        }

        [Fact]
        public void ProjectsNullValues()
        {
            // Arrange
            var person = new Person
            {
                Firstname = "John",
                Lastname = "Doe",
                BirthDate = null
            };

            var fields = new[] { nameof(Person.BirthDate) };

            // Act
            dynamic result = PartialBuilder.ToPartial(person, fields);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.BirthDate);
        }

        [Fact]
        public void DontProjectsMissingFields()
        {
            // Arrange
            var person = new Person
            {
                Firstname = "John",
                Lastname = "Doe"
            };

            var fields = new[] { nameof(Person.Firstname), "foo" };

            // Act
            dynamic result = PartialBuilder.ToPartial(person, fields);

            // Assert
            Assert.Throws<RuntimeBinderException>(() => result.foo);
            Assert.Equal("John", result.Firstname);
        }

        [Fact]
        public void ProjectsEnumerables()
        {
            // Arrange
            var persons = new List<Person> {
                new Person
                {
                    Firstname = "John"
                },
                new Person
                {
                    Firstname = "Jane"
                },
                new Person
                {
                    Firstname = "Jack"
                },
                new Person
                {
                    Firstname = "Sue"
                }
            };

            var fields = new[] { nameof(Person.Firstname) };

            // Act
            dynamic result = PartialBuilder.ToPartial(persons, fields);

            // Assert
            Assert.Equal("John", result[0].Firstname);
            Assert.Equal("Jane", result[1].Firstname);
            Assert.Equal("Jack", result[2].Firstname);
            Assert.Equal("Sue", result[3].Firstname);
        }

        [Fact]
        public void ProjectsDictionaries()
        {
            // Arrange
            var persons = new Dictionary<string, Person> {
                {
                    "john",
                    new Person
                    {
                        Firstname = "John"
                    }
                },
                {
                    "jane",
                    new Person
                    {
                        Firstname = "Jane"
                    }
                },
            };

            var fields = new[] { nameof(Person.Firstname) };

            // Act
            dynamic result = PartialBuilder.ToPartial(persons, fields);

            // Assert
            Assert.Equal("John", result["john"].Firstname);
            Assert.Equal("Jane", result["jane"].Firstname);
        }

        [Fact]
        public void ProjectsArrays()
        {
            // Arrange
            var persons = new[,] {
                {
                    new Person
                    {
                        Firstname = "John"
                    },
                    new Person
                    {
                        Firstname = "Jane"
                    }
                },
                {
                    new Person
                    {
                        Firstname = "Jack"
                    },
                    new Person
                    {
                        Firstname = "Sue"
                    }
                }
            };

            var fields = new[] { nameof(Person.Firstname) };

            // Act
            dynamic result = PartialBuilder.ToPartial(persons, fields);

            // Assert
            Assert.Equal("John", result[0, 0].Firstname);
            Assert.Equal("Jane", result[0, 1].Firstname);
            Assert.Equal("Jack", result[1, 0].Firstname);
            Assert.Equal("Sue", result[1, 1].Firstname);
        }

        [Fact]
        public void ProjectsDynamics()
        {
            // Arrange
            dynamic foo = new ExpandoObject();
            foo.bar = "baz";
            foo.sub = new
            {
                val = 1
            };

            var fields = new[] { "bar", "sub.val" };

            // Act
            dynamic result = PartialBuilder.ToPartial(foo, fields);

            // Assert
            Assert.Equal("baz", result.bar);
            Assert.Equal(1, result.sub.val);
        }

    }
}
