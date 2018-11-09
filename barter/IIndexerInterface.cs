using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barter
{
    interface IIndexerInterface
    {
        IEnumerable<Person> AllPeople();
        IEnumerable<Book> AllBooks();
        IEnumerable<Book> BookWishList(Person person);
        IEnumerable<Book> BookGiveList(Person person);
        IEnumerable<Person> PersonWishList(Book book);
        IEnumerable<Person> PersonGiveList(Book book);
    }

    abstract class Printable
    {
        abstract public void Print(bool newLine = true);
    }

}
