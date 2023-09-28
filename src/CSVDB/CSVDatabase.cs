using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;

namespace SimpleDB
{
    //sealed ensures that this class can't be inherited in any other class.
    public sealed class CSVDatabase<T> : IDatabaseRepository<T>
    {



        //I think is instance is only made when called which is the meaning of "lazy"
        private static readonly Lazy<CSVDatabase<T>> lazy = new Lazy<CSVDatabase<T>>(() =>
        new CSVDatabase<T>());
        public static CSVDatabase<T> Instance { get { return lazy.Value; } }


        //relative path
        private readonly string database;
        private static string csvPath = "chirp_cli_db.csv";


        //** THIS  CONSTRUCTOR IS SUPPOSED TO BE PRIVATE
        private CSVDatabase()
        {
            this.database = Path.Combine(Path.GetTempPath(), csvPath);
            Console.WriteLine($"CSV Database Path: {this.database}");

            if (!(File.Exists(this.database)))
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(this.database))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteHeader<T>();
                        csv.NextRecord();
                    }
                    Console.WriteLine("CSV Database file created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating CSV Database file: {ex.Message}");
                }
            }else{
                    Console.WriteLine("CSV Database file exists.");
                }
}

public IEnumerable<T> Read(int? limit = null)
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture);
    using (var reader = new StreamReader(this.database))
    using (var csv = new CsvReader(reader, config))
    {
        var records = csv.GetRecords<T>().ToList();
        return records;
    }
}

public void Store(T record)
{
    Console.WriteLine("Your cheep has been stored.");
    using (var writer = new StreamWriter(this.database, true))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecord(record);
        writer.Write("\n");
    }
}
    };
}