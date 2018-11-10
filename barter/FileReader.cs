using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Barter
{
    class FileReader
    {
        string Path;
        Participants Participants;

        enum Mode : Byte
        {
            participant,
            wishlist,
            givelist,
            undefined
        };
        Mode LineMode = Mode.undefined;

        private readonly string ParticipantLine = "#Participant";
        private readonly string WishListLine = "#WishList";
        private readonly string GiveListLine = "#GiveList";
        private readonly string EndOfParticipantMarket = "##";

        public FileReader(string path, Participants participants)
        {
            Path = path;
            Participants = participants;
        }
        public void Read()
        {
            string line;
            using (StreamReader reader = new StreamReader(Path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Process(line);
                }
            }
        }

        Participant p;
        private void Process(string line)
        {
            line = line.Trim();
            if (String.IsNullOrWhiteSpace(line))
                return;
            else if (line.Equals(ParticipantLine))
                LineMode = Mode.participant;
            else if (line.Equals(WishListLine))
                LineMode = Mode.wishlist;
            else if (line.Equals(GiveListLine))
                LineMode = Mode.givelist;
            else if (line.Equals(EndOfParticipantMarket))
                Participants.Add(p);
            else if (line.First() == '#')
                throw new InvalidDataException("Invalid tagging in the Input file");
            else
                ProcessData(line);
        }

        private void ProcessData(string line)
        {
            switch (LineMode)
            {
                case Mode.participant:
                    p = new Participant(new Person(line));
                    break;
                case Mode.wishlist:
                    List<Book> wishlist = line.Split(',').Select(b => new Book(b.Trim())).ToList();
                    p.SetWishList(wishlist);
                    break;
                case Mode.givelist:
                    List<Book> givelist = line.Split(',').Select(b => new Book(b.Trim())).ToList();
                    p.SetGiveList(givelist);
                    break;
                default:
                    break;
            }
        }
    }
}