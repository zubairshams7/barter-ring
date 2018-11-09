using System;
using System.Collections;
using System.Collections.Generic;

namespace Barter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello!!");
            Console.WriteLine("-------");

            Participants participants = new Participants();
            DataGen.CreateSeedData(participants);

            foreach (Participant participant in participants)
                participant.Print();

            Indexes indexers = new Indexes();
            indexers.Build(participants);
            indexers.PrintIndexes();

            Participant firstParticipant = participants.Find("Noman");
            Search search = new Search(indexers, firstParticipant);
            search.Execute();

            Console.ReadKey();
        }
    }

    class DataGen
    {
        public static void CreateSeedData(Participants users)
        {
            Participant noman = new Participant(new Person("Noman"));
            Participant heather = new Participant(new Person("Heather"));
            Participant sraboni = new Participant(new Person("Sraboni"));

            noman.SetWishList(new List<Book>() { new Book("Book1"),
                                                 new Book("Book2"),
                                                 new Book("Book3") });
            noman.SetGiveList(new List<Book>() { new Book("Book4"),
                                                 new Book("Book5"),
                                                 new Book("Book6") });
            heather.SetWishList(new List<Book>() { new Book("Book5"),
                                                   new Book("Book4"),
                                                   new Book("Book70") });
            heather.SetGiveList(new List<Book>() { new Book("Book8"),
                                                   new Book("Book9"),
                                                   new Book("Book10") });
            sraboni.SetWishList(new List<Book>() { new Book("Book9"),
                                                   new Book("Book8"),
                                                   new Book("Book110") });
            sraboni.SetGiveList(new List<Book>() { new Book("Book2"),
                                                   new Book("Book30"),
                                                   new Book("Book120") });

            users.Add(noman);
            users.Add(heather);
            users.Add(sraboni);

        }
    }

    class Participants : IEnumerable<Participant>
    {
        public List<Participant> UserList { get; set; }
        public Participants()
        {
            UserList = new List<Participant>();
        }
        public void Add(Participant participant)
        {
            UserList.Add(participant);
        }
        public Participant Find(Person person)
        {
            return UserList.Find(user => user.Person.Equals(person));
        }
        public Participant Find(String personName)
        {
            return UserList.Find(user => user.Person.Name.Equals(personName));
        }

        public IEnumerator<Participant> GetEnumerator()
        {
            for (int i = 0; i < UserList.Count; i++)
            {
                yield return UserList[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for(int i = 0; i < UserList.Count; i++)
            {
                yield return UserList[i];
            }
        }
    }

    class Participant
    {
        public Person Person { get; set; }
        public List<Book> WishList { get; set; }
        public List<Book> GiveList { get; set; }
        public Participant(Person person)
        {
            Person = person;
            WishList = new List<Book>();
            GiveList = new List<Book>();
        }
        public void SetWishList(List<Book> books)
        {
            WishList = books;
        }
        public void SetGiveList(List<Book> books)
        {
            GiveList = books;
        }
        public void Print()
        {
            Person.Print();
            Console.WriteLine("Wishlist::");
            foreach (Book b in WishList)
            {
                Console.Write(" -> ");
                b.Print();
            }
            Console.WriteLine("Givelist::");
            foreach (Book b in GiveList)
            {
                Console.Write(" -> ");
                b.Print();
            }
        }
    }

    class Book : Printable, IEquatable<Book>
    {
        public string Name { get; set; }
        public Book(string name)
        {
            Name = name;
        }
        public override void Print(bool newLine = true)
        {
            if (newLine)
                Console.WriteLine(Name);
            else
                Console.Write(Name);
        }

        bool IEquatable<Book>.Equals(Book other)
        {
            return Name == other.Name;
        }
        public override bool Equals(object obj)
        {
            return Name == ((Book)obj).Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    class Person : Printable, IEquatable<Person>
    {
        public string Name { get; set; }
        public Person(string name)
        {
            Name = name;
        }
        public override void Print(bool newLine = true)
        {
            if (newLine)
                Console.WriteLine(Name);
            else
                Console.Write(Name);
        }
        bool IEquatable<Person>.Equals(Person other)
        {
            return Name == other.Name;
        }
        public override bool Equals(object obj)
        {
            return Name == ((Person)obj).Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
