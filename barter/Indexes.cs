using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barter
{
    class Indexes : IIndexerInterface
    {
        private class DoubleList<T>
        {
            private List<T> wish = new List<T>();
            private List<T> give = new List<T>();
            public DoubleList()
            { }
            public void AddToWish(T item)
            {
                wish.Add(item);
            }
            public void AddToGive(T item)
            {
                give.Add(item);
            }
            public List<T> GetWishList()
            {
                return wish;
            }
            public List<T> GetGiveList()
            {
                return give;
            }
        }

        private Dictionary<Person, DoubleList<Book>> personIndex;
        private Dictionary<Book, DoubleList<Person>> bookIndex;
        public Indexes()
        {
            personIndex = new Dictionary<Person, DoubleList<Book>>();
            bookIndex = new Dictionary<Book, DoubleList<Person>>(); 
        }
        public void Build(Participants participants)
        {
            foreach(var participant in participants)
            {
                AddToPersonIndex(participant);
                AddToBookIndex(participant);
            }
        }
        private void AddToPersonIndex(Participant participant)
        {
            DoubleList<Book> bookDoubleList = new DoubleList<Book>();
            participant.WishList.ForEach(b => bookDoubleList.AddToWish(b));
            participant.GiveList.ForEach(b => bookDoubleList.AddToGive(b));
            personIndex.Add(participant.Person, bookDoubleList);
        }
        private void AddToBookIndex(Participant participant)
        {
            foreach(Book b in participant.WishList)
            {
                if (bookIndex.ContainsKey(b))
                    bookIndex[b].AddToWish(participant.Person);
                else
                {
                    bookIndex.Add(b, new DoubleList<Person>());
                    bookIndex[b].AddToWish(participant.Person);
                }
            }
            foreach(Book b in participant.GiveList)
            {
                if (bookIndex.ContainsKey(b))
                    bookIndex[b].AddToGive(participant.Person);
                else
                {
                    bookIndex.Add(b, new DoubleList<Person>());
                    bookIndex[b].AddToGive(participant.Person);
                }
            }
        }

        public IEnumerable<Person> AllPeople()
        {
            return personIndex.Keys;
        }
        public IEnumerable<Book> AllBooks()
        {
            return bookIndex.Keys;
        }
        /// <summary>
        /// Public method for getting the list of Books in the Person's Wish List
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public IEnumerable<Book> BookWishList(Person person)
        {
            return personIndex.First(p => p.Key.Equals(person)).Value.GetWishList();
        }
        /// <summary>
        /// Public method for getting the list of Books in the Person's Give List
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public IEnumerable<Book> BookGiveList(Person person)
        {
            return personIndex.First(p => p.Key.Equals(person)).Value.GetGiveList();
        }
        /// <summary>
        /// Public method for getting the list of People who has this Book in their Wish List
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public IEnumerable<Person> PersonWishList(Book book)
        {
            return bookIndex.First(p => p.Key.Equals(book)).Value.GetWishList();
        }
        /// <summary>
        /// Public method for getting the list of People who has this Book in their Give List
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public IEnumerable<Person> PersonGiveList(Book book)
        {
            return bookIndex.First(p => p.Key.Equals(book)).Value.GetGiveList();
        }
        
        /// <summary>
        /// Prints Index
        /// </summary>
        public void PrintIndexes()
        {
            PrintPersonIndex();
            PrintBookIndex();
        }

        /// <summary>
        /// Prints Person Index
        /// </summary>
        private void PrintPersonIndex()
        {
            Console.WriteLine();
            Console.WriteLine("** PersonIndex **");
            foreach (var entry in personIndex)
            {
                Console.Write("Person -> ");
                entry.Key.Print();
                Console.WriteLine("Books::WishList");
                foreach (var book in entry.Value.GetWishList())
                {
                    Console.Write(" -> ");
                    book.Print();
                }
                Console.WriteLine("Books::GiveList");
                foreach (var book in entry.Value.GetGiveList())
                {
                    Console.Write(" -> ");
                    book.Print();
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints Book Index
        /// </summary>
        private void PrintBookIndex()
        {
            Console.WriteLine();
            Console.WriteLine("** BookIndex **");
            foreach (var entry in bookIndex)
            {
                Console.Write("Book -> ");
                entry.Key.Print();
                Console.WriteLine("Persons::WishList");
                foreach (var book in entry.Value.GetWishList())
                {
                    Console.Write(" -> ");
                    book.Print();
                }
                Console.WriteLine("Persons::GiveList");
                foreach (var book in entry.Value.GetGiveList())
                {
                    Console.Write(" -> ");
                    book.Print();
                }
            }
            Console.WriteLine();
        }
    }
}
