using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barter
{
    /// <summary>
    /// Performs search on double index
    /// </summary>
    class Search
    {
        private Participant FirstParticipant;
        private IIndexerInterface Indexer;
        private FullCircle<Book, Person> Transfers;
        public Search(IIndexerInterface indexer, Participant firstParticipant)
        {
            Indexer = indexer;
            FirstParticipant = firstParticipant;
            Transfers = new FullCircle<Book, Person>();
        }

        /// <summary>
        /// Performs search for one person based on his/her wishlist & givelist
        /// </summary>
        public void Execute()
        {

            // Once we find a Book in the First Participants Wish List from Someone elses
            // Give list then mark that Book as Found
            // Set all Books found to False initially
            List<Searched<Book>> fpBookFoundList = new List<Searched<Book>>();
            List<Book> fpBookWishList = Indexer.BookWishList(FirstParticipant.Person) as List<Book>;
            fpBookWishList.ForEach(b => fpBookFoundList.Add(new Searched<Book>(b)));
            // Reset
            fpBookFoundList.ForEach(bf => bf.Found = false);

            // Queue of people whose Give list we are going to search next
            // Start with the first participant
            Queue<Person> peopleQueue = new Queue<Person>();
            peopleQueue.Enqueue(FirstParticipant.Person);

            // Capture the trade trail
            ExecuteAux(FirstParticipant.Person, peopleQueue, fpBookFoundList);

            // There needs to be a Pruning from Back to Front depending on how many books
            // FirstParticipant received at the end. There might be an algorithm solutions in 
            // the recursion step, if not, Prune
            PrintTransferTrack();
            Transfers.Prune();
        }

        /// <summary>
        /// Recursively propagate search fanning out trying to build a chain of interchangeable items
        /// To do: The recursive look up can be performed in O(n) using Dynammic Programming with the 
        /// use of a single look up table
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="peopleQueue"></param>
        /// <param name="fpBookFoundList"></param>
        private void ExecuteAux(Person origin, 
                                Queue<Person> peopleQueue, 
                                List<Searched<Book>> fpBookFoundList)
        {
            // Base Case

            // No more people to search
            if (!peopleQueue.Any())
                return;

            // Current Give List to process
            IEnumerable<Book> giveList = Indexer.BookGiveList(peopleQueue.Peek());

            // If we didn't add any one new Person then we cannot go any further
            // Nobody wants the books in the current Give List
            bool peopleToAdd = false;
            foreach(Book book in giveList)
            {
                IEnumerable<Person> wantingPeople = Indexer.PersonWishList(book);
                if (wantingPeople.Any())
                {
                    peopleToAdd = true;
                    break;
                }
            }
            if (!peopleToAdd)
                return;

            // Recursive Case

            Person currentPerson = peopleQueue.Dequeue();
            IEnumerable<Book> currentGiveList = Indexer.BookGiveList(currentPerson);
            foreach(Book b in currentGiveList)
            {
                IEnumerable<Person> wantingPeople = Indexer.PersonWishList(b);
                foreach (Person p in wantingPeople)
                {
                    PrintTrack(currentPerson, b, p);
                    Transfers.Add(currentPerson, b, p);
                    if (!p.Equals(origin))
                        peopleQueue.Enqueue(p);
                    else
                        fpBookFoundList.Find(searchedBook => searchedBook.Item.Equals(b)).Found = true;
                }
            }

            ExecuteAux(origin, peopleQueue, fpBookFoundList);
        }

        /// <summary>
        /// Prints all candidates for transfer based on wishlist/givelist match
        /// </summary>
        /// <param name="currentPerson"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        private static void PrintTrack(Person currentPerson, Book b, Person p)
        {
            Console.Write("Candidate Book Transfer -> ");
            b.Print(false);
            Console.Write(", " + "Give from: ");
            currentPerson.Print(false);
            Console.Write(" to ");
            p.Print();
        }

        private void PrintTransferTrack()
        {
            foreach(var transfer in Transfers)
            {
                transfer.Print();
            }
        }
    }

    /// <summary>
    /// Tracks which item is getting transferred to who
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="S"></typeparam>
    class TransferTrack<T, S> where T: Printable where S : Printable
    {
        public T Item { get; set; }
        public S From { get; set; }
        public S To { get; set; }
        public TransferTrack(T item, S from, S to)
        {
            Item = item;
            From = from;
            To = to;
        }
        public void Print()
        {
            Console.Write("Sending Book ");
            Item.Print(false);
            Console.Write(" From ");
            From.Print(false);
            Console.Write(" To ");
            To.Print(false);
            Console.WriteLine();
        }
    }

    class Searched<T>
    {
        public T Item { get; set; }
        public bool Found { get; set; }
        public Searched(T item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// To do: Kinda cheated here. There should be no reference of Books and Persons
    /// in any of the classes in Search file. Book and Person should really be an instance
    /// of abstract class and interfaces implementing the necessary behavior
    /// That's why we see the casting because the design is loose - we really
    /// shouldn't know if T and S are Book or Person here
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="S"></typeparam>
    class FullCircle<T, S> : IEnumerable<TransferTrack<T, S>> where T : Printable
                                                              where S : Printable
    {
        private List<TransferTrack<T, S>> DeliveryCircle;
        public FullCircle()
        {
            DeliveryCircle = new List<TransferTrack<T, S>>();
        }

        public void Add(Person currentPerson, Book b, Person p)
        {
            bool found = DeliveryCircle.Exists(t => t.Item.Equals(b) &&
                                                    t.From.Equals(currentPerson) &&
                                                    t.To.Equals(p));
            if (!found)
                DeliveryCircle.Add(new TransferTrack<T, S>(b as T, currentPerson as S, p as S));
        }

        /// <summary>
        /// You cannot give more than what you get at any transfer
        /// </summary>
        public void Prune()
        {
            // Need to maintain the cardinal equality between wish and give
            // No one can get more than what they give
            List<int> giveCounts = new List<int>(); // wishCount will work equally
            var groups = DeliveryCircle.GroupBy(
                p => p.From,
                p => p.Item,
                (key, g) => new { From = key, Items = g.ToList() }
                );
            var min = groups.Select(w => w.Items.Count).Min();

            Console.WriteLine();
            Console.WriteLine("Transfer Cardinality -> " + min);

            // Print Pruned Transfer Track - Ideally you want to store 
            // this in a data structure
            Console.WriteLine();
            Console.WriteLine("Final Transfer List..");
            foreach(var transfer in groups)
            {
                int i = 0;
                while (i < min)
                {
                    transfer.From.Print(false);
                    Console.Write(" gives ");
                    transfer.Items[i].Print();
                    i++;
                }
            }
        }

        IEnumerator<TransferTrack<T, S>> IEnumerable<TransferTrack<T, S>>.GetEnumerator()
        {
            for (int i = 0; i < DeliveryCircle.Count; i++)
            {
                yield return DeliveryCircle[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < DeliveryCircle.Count; i++)
            {
                yield return DeliveryCircle[i];
            }
        }
    }
}
