using System.Text;
using Task1OIP;

namespace EnglishTextCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string outputDirectory = "downloaded_english_pages";

            // Массив из 100+ ссылок Project Gutenberg
            string[] urlsToDownload = new string[]
            {
                // Топ-100 книг Project Gutenberg
                "https://www.gutenberg.org/cache/epub/1342/pg1342.txt",   // Pride and Prejudice
                "https://www.gutenberg.org/cache/epub/84/pg84.txt",       // Frankenstein
                "https://www.gutenberg.org/cache/epub/11/pg11.txt",       // Alice's Adventures in Wonderland
                "https://www.gutenberg.org/cache/epub/2701/pg2701.txt",   // Moby Dick
                "https://www.gutenberg.org/cache/epub/74/pg74.txt",       // The Adventures of Tom Sawyer
                "https://www.gutenberg.org/cache/epub/76/pg76.txt",       // Adventures of Huckleberry Finn
                "https://www.gutenberg.org/cache/epub/98/pg98.txt",       // A Tale of Two Cities
                "https://www.gutenberg.org/cache/epub/1400/pg1400.txt",   // Great Expectations
                "https://www.gutenberg.org/cache/epub/730/pg730.txt",     // Oliver Twist
                "https://www.gutenberg.org/cache/epub/1260/pg1260.txt",   // Jane Eyre
                "https://www.gutenberg.org/cache/epub/768/pg768.txt",     // Wuthering Heights
                "https://www.gutenberg.org/cache/epub/158/pg158.txt",     // Emma
                "https://www.gutenberg.org/cache/epub/161/pg161.txt",     // Sense and Sensibility
                "https://www.gutenberg.org/cache/epub/345/pg345.txt",     // Dracula
                "https://www.gutenberg.org/cache/epub/174/pg174.txt",     // The Picture of Dorian Gray
                "https://www.gutenberg.org/cache/epub/1661/pg1661.txt",   // The Adventures of Sherlock Holmes
                "https://www.gutenberg.org/cache/epub/244/pg244.txt",     // A Study in Scarlet
                "https://www.gutenberg.org/cache/epub/2852/pg2852.txt",   // The Hound of the Baskervilles
                "https://www.gutenberg.org/cache/epub/1232/pg1232.txt",   // The Prince
                "https://www.gutenberg.org/cache/epub/996/pg996.txt",     // Don Quixote
                "https://www.gutenberg.org/cache/epub/6130/pg6130.txt",   // The Iliad
                "https://www.gutenberg.org/cache/epub/1727/pg1727.txt",   // The Odyssey
                "https://www.gutenberg.org/cache/epub/2600/pg2600.txt",   // War and Peace
                "https://www.gutenberg.org/cache/epub/28054/pg28054.txt", // The Brothers Karamazov
                "https://www.gutenberg.org/cache/epub/2554/pg2554.txt",   // Crime and Punishment
                "https://www.gutenberg.org/cache/epub/43/pg43.txt",       // Dr. Jekyll and Mr. Hyde
                "https://www.gutenberg.org/cache/epub/219/pg219.txt",     // Heart of Darkness
                "https://www.gutenberg.org/cache/epub/119/pg119.txt",     // The Time Machine
                "https://www.gutenberg.org/cache/epub/35/pg35.txt",       // The Time Machine (alt)
                "https://www.gutenberg.org/cache/epub/36/pg36.txt",       // The War of the Worlds
                "https://www.gutenberg.org/cache/epub/101/pg101.txt",     // The Invisible Man
                "https://www.gutenberg.org/cache/epub/5230/pg5230.txt",   // The Island of Doctor Moreau
                "https://www.gutenberg.org/cache/epub/27591/pg27591.txt", // The Lost World
                "https://www.gutenberg.org/cache/epub/829/pg829.txt",     // Gulliver's Travels
                "https://www.gutenberg.org/cache/epub/17192/pg17192.txt", // Beowulf
                "https://www.gutenberg.org/cache/epub/13846/pg13846.txt", // Sir Gawain
                "https://www.gutenberg.org/cache/epub/30254/pg30254.txt", // Faust
                "https://www.gutenberg.org/cache/epub/19942/pg19942.txt", // Candide
                "https://www.gutenberg.org/cache/epub/2130/pg2130.txt",   // Utopia
                "https://www.gutenberg.org/cache/epub/3200/pg3200.txt",   // The Republic
                "https://www.gutenberg.org/cache/epub/1497/pg1497.txt",   // The Republic (alt)
                "https://www.gutenberg.org/cache/epub/1656/pg1656.txt",   // Meditations
                "https://www.gutenberg.org/cache/epub/3296/pg3296.txt",   // The Art of War
                "https://www.gutenberg.org/cache/epub/1080/pg1080.txt",   // A Modest Proposal
                "https://www.gutenberg.org/cache/epub/8800/pg8800.txt",   // The Divine Comedy
                "https://www.gutenberg.org/cache/epub/100/pg100.txt",     // Complete Works of Shakespeare
                "https://www.gutenberg.org/cache/epub/1513/pg1513.txt",   // Romeo and Juliet
                "https://www.gutenberg.org/cache/epub/1524/pg1524.txt",   // Hamlet
                "https://www.gutenberg.org/cache/epub/1533/pg1533.txt",   // Macbeth
                "https://www.gutenberg.org/cache/epub/2263/pg2263.txt",   // King Lear
                "https://www.gutenberg.org/cache/epub/1795/pg1795.txt",   // Othello
                "https://www.gutenberg.org/cache/epub/2243/pg2243.txt",   // The Tempest
                "https://www.gutenberg.org/cache/epub/2242/pg2242.txt",   // A Midsummer Night's Dream
                "https://www.gutenberg.org/cache/epub/1785/pg1785.txt",   // The Winter's Tale
                "https://www.gutenberg.org/cache/epub/1531/pg1531.txt",   // Julius Caesar
                "https://www.gutenberg.org/cache/epub/120/pg120.txt",     // A Christmas Carol
                "https://www.gutenberg.org/cache/epub/46/pg46.txt",       // A Christmas Carol (alt)
                "https://www.gutenberg.org/cache/epub/19337/pg19337.txt", // The Raven
                "https://www.gutenberg.org/cache/epub/2147/pg2147.txt",   // The Raven (alt)
                "https://www.gutenberg.org/cache/epub/1065/pg1065.txt",   // The Raven (poetry)
                "https://www.gutenberg.org/cache/epub/14082/pg14082.txt", // The Call of the Wild
                "https://www.gutenberg.org/cache/epub/215/pg215.txt",     // The Call of the Wild (alt)
                "https://www.gutenberg.org/cache/epub/910/pg910.txt",     // The Jungle Book
                "https://www.gutenberg.org/cache/epub/236/pg236.txt",     // The Jungle Book (alt)
                "https://www.gutenberg.org/cache/epub/27200/pg27200.txt", // Just So Stories
                "https://www.gutenberg.org/cache/epub/2781/pg2781.txt",   // Kim
                "https://www.gutenberg.org/cache/epub/233/pg233.txt",     // Kim (alt)
                "https://www.gutenberg.org/cache/epub/1322/pg1322.txt",   // Leaves of Grass
                "https://www.gutenberg.org/cache/epub/27473/pg27473.txt", // Walden
                "https://www.gutenberg.org/cache/epub/205/pg205.txt",     // Les Misérables
                "https://www.gutenberg.org/cache/epub/135/pg135.txt",     // Les Misérables (alt)
                "https://www.gutenberg.org/cache/epub/17483/pg17483.txt", // The Count of Monte Cristo
                "https://www.gutenberg.org/cache/epub/1184/pg1184.txt",   // The Count of Monte Cristo (alt)
                "https://www.gutenberg.org/cache/epub/1257/pg1257.txt",   // The Three Musketeers
                "https://www.gutenberg.org/cache/epub/2814/pg2814.txt",   // The Three Musketeers (alt)
                "https://www.gutenberg.org/cache/epub/2641/pg2641.txt",   // The Hunchback of Notre-Dame
                "https://www.gutenberg.org/cache/epub/2610/pg2610.txt",   // The Hunchback of Notre-Dame (alt)
                "https://www.gutenberg.org/cache/epub/786/pg786.txt",     // 20,000 Leagues Under the Sea
                "https://www.gutenberg.org/cache/epub/164/pg164.txt",     // 20,000 Leagues Under the Sea (alt)
                "https://www.gutenberg.org/cache/epub/103/pg103.txt",     // Around the World in 80 Days
                "https://www.gutenberg.org/cache/epub/8001/pg8001.txt",   // Around the World in 80 Days (alt)
                "https://www.gutenberg.org/cache/epub/1263/pg1263.txt",   // The Secret Garden
                "https://www.gutenberg.org/cache/epub/17396/pg17396.txt", // The Secret Garden (alt)
                "https://www.gutenberg.org/cache/epub/542/pg542.txt",     // The Princess and the Goblin
                "https://www.gutenberg.org/cache/epub/34345/pg34345.txt", // The Princess and the Goblin (alt)
                "https://www.gutenberg.org/cache/epub/55/pg55.txt",       // The Wonderful Wizard of Oz
                "https://www.gutenberg.org/cache/epub/43936/pg43936.txt", // The Wonderful Wizard of Oz (alt)
                "https://www.gutenberg.org/cache/epub/30568/pg30568.txt", // The Marvelous Land of Oz
                "https://www.gutenberg.org/cache/epub/23581/pg23581.txt", // Ozma of Oz
                "https://www.gutenberg.org/cache/epub/5197/pg5197.txt",   // Peter Pan
                "https://www.gutenberg.org/cache/epub/16/pg16.txt",       // Peter Pan (alt)
                "https://www.gutenberg.org/cache/epub/17378/pg17378.txt", // The Wind in the Willows
                "https://www.gutenberg.org/cache/epub/2899/pg2899.txt",   // The Wind in the Willows (alt)
                "https://www.gutenberg.org/cache/epub/1155/pg1155.txt",   // The Importance of Being Earnest
                "https://www.gutenberg.org/cache/epub/844/pg844.txt",     // The Importance of Being Earnest (alt)
                "https://www.gutenberg.org/cache/epub/1458/pg1458.txt",   // The Picture of Dorian Gray (alt)
                "https://www.gutenberg.org/cache/epub/3070/pg3070.txt",   // Fathers and Sons
                "https://www.gutenberg.org/cache/epub/13214/pg13214.txt", // Fathers and Sons (alt)
                "https://www.gutenberg.org/cache/epub/15859/pg15859.txt", // Anna Karenina
                "https://www.gutenberg.org/cache/epub/1399/pg1399.txt",   // Anna Karenina (alt)
                "https://www.gutenberg.org/cache/epub/883/pg883.txt",     // The Red Badge of Courage
                "https://www.gutenberg.org/cache/epub/32242/pg32242.txt", // The Red Badge of Courage (alt)
                "https://www.gutenberg.org/cache/epub/37106/pg37106.txt", // The Awakening
                "https://www.gutenberg.org/cache/epub/160/pg160.txt",     // The Awakening (alt)
                "https://www.gutenberg.org/cache/epub/4085/pg4085.txt",   // The Scarlet Letter
                "https://www.gutenberg.org/cache/epub/33/pg33.txt",       // The Scarlet Letter (alt)
                "https://www.gutenberg.org/cache/epub/2192/pg2192.txt",   // The Social Contract
            };

            var crawler = new GutenbergCrawler(outputDirectory);
            await crawler.CrawlAsync(urlsToDownload);
        }
    }

    
    
}